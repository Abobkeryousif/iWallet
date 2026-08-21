
namespace iWallet.Infrastructure.Implemention
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILimitService _limitService;
        public TransactionRepository(ApplicationDbContext context, ILimitService limitService)
        {
            _context = context;
            _limitService = limitService;
        }
        public async Task<string> MakeDepositAsync(DepositDto depositDto)
            {

            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == depositDto.walletId);
            if (wallet == null || wallet.Status != WalletStatus.Active)
            {
                DepositMetrics.DepositsFailuresTotal.Inc();
                throw new Exception("invalid wallet");
                
            }

            var reference = GenerateReference(TransactionType.Deposit);

            var transaction = new Transaction
            {
                FromWalletId = depositDto.walletId,
                Reference = reference,
                Amount = depositDto.amount,
                TransactionType = TransactionType.Deposit,
                Status = TransactionStatus.Success,
            };

            wallet.Balance += depositDto.amount;
            _context.Wallets.Update(wallet);
            await _context.Transactions.AddAsync(transaction);
            _context.SaveChanges();

            DepositMetrics.DepositsTotal.Inc();

            var ledger = new LedgerEntry
            {
                
                WalletId = depositDto.walletId,
                TransactionId = transaction.Id,
                Debit = 0,
                Credit = depositDto.amount,
                Particulars = $"Deposit ammount = {depositDto.amount} to wallet {wallet.WalletNumber}"
                
            };

            _context.LedgerEntries.Add(ledger);
            await _context.SaveChangesAsync();

            return $"Successfly Deposit with Ammount {transaction.Amount} and total wallet balance = {wallet.Balance}";

        }

        public async Task<string> TransferAsync(string toAccountNumber, decimal amount, int userId)
        {

            Log.Information("Starring Transfer Process. UserId: {UserId}, ToAccount: {toAccount}, Amount: {Amount}",
                userId,
                toAccountNumber,
                amount);


            var senderWallet = await _context.Wallets.Include(w => w.User).FirstOrDefaultAsync(u => u.UserId == userId);
            if (senderWallet == null || senderWallet.Status != WalletStatus.Active)
            {
                Log.Warning("Sender wallet not found. UserId: {UserId}",userId);
                TransferMetrics.TransferFailureTotal.Inc();
                throw new Exception("invalid sender wallet");
            }

            var receiverWallet = await _context.Wallets.Include(w => w.User).FirstOrDefaultAsync(an => an.WalletNumber == toAccountNumber);
            if (receiverWallet == null || receiverWallet.Status != WalletStatus.Active)
            {
                TransferMetrics.TransferFailureTotal.Inc();
                throw new Exception("Invalid receiver wallet");
            }

            if (senderWallet.Id == receiverWallet.Id)
            {
                TransferMetrics.TransferFailureTotal.Inc();
                throw new Exception("you can't transfer to yourself");
            }

            if (senderWallet.Balance < amount)
            {
                TransferMetrics.TransferFailureTotal.Inc();
                throw new Exception("insufficient balance");
            }

            // frud pervention and get value from cache

            var limit = await _limitService.GetUserLimitAsync(senderWallet.UserId);

            var todyTotal = await GetTransactionsTodayTotalAsync(senderWallet.UserId);
            if (todyTotal + amount > limit.DailyLimit)
            {
                TransferMetrics.TransferFailureTotal.Inc();
                throw new Exception("You already Exceeded daily limit 200000");
            }

            await _context.SaveChangesAsync();

            var reference = GenerateReference(TransactionType.Transfer);


            Log.Information(
            "Database transaction start for transfer. FromWallet: {FromWallet}, ToWallet: {ToWallet}",
            senderWallet.WalletNumber,
            receiverWallet.WalletNumber);

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                senderWallet.Balance -= amount;
                receiverWallet.Balance += amount;

                var transaction = new Transaction
                {
                    FromWalletId = senderWallet.Id,
                    ToWalletId = receiverWallet.Id,
                    Amount = amount,
                    TransactionType = TransactionType.Transfer,
                    Reference = reference,
                    Status = TransactionStatus.Success
                };

                await _context.Transactions.AddAsync(transaction);
                _context.SaveChanges();

                TransferMetrics.TransferTotal.Inc();

                var senderLedger = new LedgerEntry
                {
                    WalletId = senderWallet.Id,
                    TransactionId = transaction.Id,
                    Debit = amount,
                    Credit = 0,
                    Particulars = $"Transfer to {senderWallet.WalletNumber}"
                };

                await _context.LedgerEntries.AddAsync(senderLedger);
                _context.SaveChanges();

                var receiverLedger = new LedgerEntry
                {
                    WalletId = receiverWallet.Id,
                    TransactionId = transaction.Id,
                    Debit = 0,
                    Credit = amount,
                    Particulars = $"Successfly receive transaction from {receiverWallet.WalletNumber}"
                };

                await _context.LedgerEntries.AddAsync(receiverLedger);
                _context.SaveChanges();

                senderLedger.UpdatedAt = DateTime.UtcNow;
                receiverWallet.UpdatedAt = DateTime.UtcNow;

                _context.Wallets.Update(senderWallet);
                _context.Wallets.Update(receiverWallet);

                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();


                Log.Information(
                "Transfer completed successfully. Reference: {Reference}, Amount: {Amount}, Sender: {Sender}, Receiver: {Receiver}",
                reference,
                amount,
                senderWallet.WalletNumber,
                receiverWallet.WalletNumber);


                var recepit = new TransferReceiptDto
                {
                    TransactionReference = reference,
                    SenderName = senderWallet.User.UserName,
                    SenderWalletNumber = senderWallet.WalletNumber,
                    ReceiverName = receiverWallet.User.UserName,
                    ReceiveWalletNumber = receiverWallet.WalletNumber,
                    Amount = amount,
                    Fees = 0,
                    Currency = "SAR",
                    Date = DateTime.UtcNow,
                    Status = transaction.Status.ToString(),
                };

                var path = Path.Combine("receipts/");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var logoPath = Path.Combine("wwwroot/assets", "iWallet-logo.png");

                var document = new TransferReceiptDocument(recepit, logoPath);

                document.GeneratePdf($"receipts/receipt-{recepit.TransactionReference}.pdf");


                return $"Transfer Completed Successfly with Transaction Reference {reference}";
                }

            catch(Exception ex)
            {
                await dbTransaction.RollbackAsync();

                Log.Error(
                ex.Message,
                "Transfer failed. UserId: {UserId}, Amount: {Amount}, Reference: {Reference}",
                userId,
                amount,
                reference);

                throw;
            }
        }

        public async Task<string> MakeWithdrawal(int walletId, decimal amount)
        {

            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null || wallet.Status != WalletStatus.Active)
            {
                WithdrawalMetrics.WithdrawalFailuresTotal.Inc();
                throw new Exception("Invalid wallet");
            }

            if (wallet.Balance < amount)
            {
                WithdrawalMetrics.WithdrawalFailuresTotal.Inc();
                throw new Exception("Insufficient balance");
            }

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            _context.Wallets.Update(wallet);
            _context.SaveChanges();

            var reference = GenerateReference(TransactionType.Withdrawal);

            var transaction = new Transaction
            {
                FromWalletId = walletId,
                Reference = reference,
                Amount = amount,
                Status = TransactionStatus.Success,
                TransactionType = TransactionType.Withdrawal,
                
            };

             await _context.Transactions.AddAsync(transaction);
            _context.SaveChanges();
            WithdrawalMetrics.WithdrawalTotal.Inc();

            var ledger = new LedgerEntry
            {
                TransactionId = transaction.Id,
                WalletId = wallet.Id,
                Debit = amount,
                Credit = 0,
                Particulars = $"successfly withdrawal from {wallet.WalletNumber} with balance {amount}"
            };

            _context.LedgerEntries.Add(ledger);
            _context.SaveChanges();

            return $"withdrawal completed successfly with Transaction Reference {reference}";
        }

        public async Task<List<TransactionDto>> TransactionHistory(int walletId)
        {
            var wallet = await _context.Wallets.AnyAsync(w=> w.Id == walletId);
            if (!wallet)
                throw new Exception("invalid wallet");

            var history = await _context.Transactions
                .Where(t => t.FromWalletId == walletId || t.ToWalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .AsNoTracking()
                .Select(t=> new TransactionDto
                {
                    Reference = t.Reference,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType.ToString(),
                    TransactionStatus = t.Status.ToString()
                })
                .ToListAsync();

            if (history.Count == 0)
                throw new Exception("not make any transactions yet");

            return history;
        }

        public async Task<string> TransferToBeneficiery(string beneficieryName, decimal amount, int userId)
        {
            var checkBeneficiery = await _context.Beneficiaries.FirstOrDefaultAsync(n=> n.Name.ToLower() == beneficieryName.ToLower());
            if (checkBeneficiery == null)
                throw new Exception("invalid beneficiery name");

            await TransferAsync(checkBeneficiery.WalletNumber, amount, userId);

            return $"Transfer Completed Successfly with amount {amount}";
        }


        public async Task<decimal> GetTransactionsTodayTotalAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Transactions
                .Where(t =>
                    t.FromWallet.UserId == userId &&
                    t.CreatedAt >= today &&
                    t.CreatedAt < tomorrow &&
                    t.Status == TransactionStatus.Success)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;
        }

        private static string GenerateReference(TransactionType type)
        {
            var data = DateTime.UtcNow.ToString("yyMMdd");
            var random = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            var prefix = type switch
            {
                TransactionType.Deposit => "DEP",
                TransactionType.Transfer => "TRN",
                TransactionType.Withdrawal => "WDR",
                _ => "UNK"
            };

            return $"{prefix}-{data}-{random}";
        }

   
    }
}
