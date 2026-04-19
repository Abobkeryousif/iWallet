
namespace iWallet.Infrastructure.Implemention
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletRepository _walletRepository;
        private readonly ILimitService _limitService;
        public TransactionRepository(ApplicationDbContext context, IWalletRepository walletRepository, ILimitService limitService)
        {
            _context = context;
            _walletRepository = walletRepository;
            _limitService = limitService;
        }
        public async Task<string> MakeDepositAsync(int walletId, decimal ammount)
        {
            if (ammount <= 0)
                throw new Exception("invalid amount");

            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);
                if (wallet == null || wallet.Status != WalletStatus.Active)
                    throw new Exception("invalid wallet");

            var reference = GenerateReference(TransactionType.Deposit);

            var transaction = new Transaction
            {
                FromWalletId = walletId,
                Reference = reference,
                Amount = ammount,
                TransactionType = TransactionType.Deposit,
                Status = TransactionStatus.Success,
            };

            wallet.Balance += ammount;
            _context.Wallets.Update(wallet);
            await _context.Transactions.AddAsync(transaction);
            _context.SaveChanges();
            

            var ledger = new LedgerEntry
            {
                
                WalletId = walletId,
                TransactionId = transaction.Id,
                Debit = 0,
                Credit = ammount,
                Particulars = $"Deposit ammount = {ammount} to wallet {wallet.WalletNumber}"
                
            };

            _context.LedgerEntries.Add(ledger);
            await _context.SaveChangesAsync();

            return $"Successfly Deposit with Ammount {transaction.Amount} and total wallet balance = {wallet.Balance}";

        }

        public async Task<string> TransferAsync(string toAccountNumber, decimal amount, int userId)
        {
            if (amount <= 0)
                throw new Exception("Invalid Amount");

            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(u => u.UserId == userId);
            if (senderWallet == null || senderWallet.Status != WalletStatus.Active)
                throw new Exception("invalid sender wallet");

            var receiverWallet = await _context.Wallets.FirstOrDefaultAsync(an => an.WalletNumber == toAccountNumber);
            if (receiverWallet == null || receiverWallet.Status != WalletStatus.Active)
                throw new Exception("Invalid receiver wallet");

            if (senderWallet.Id == receiverWallet.Id)
                throw new Exception("you can't transfer to yourself");

            if (senderWallet.Balance < amount)
                throw new Exception("insufficient balance");

            // frud pervention and get value from cache

            var limit = await _limitService.GetUserLimitAsync(senderWallet.UserId);
            if (amount > limit.PerTransactionLimit)
                throw new Exception("Exceeded per transaction limit 5000");

            var todyTotal = await GetTransactionsTodayTotalAsync(senderWallet.UserId);
            if (todyTotal + amount > limit.DailyLimit)
                throw new Exception("You already Exceeded daily limit 200000");

            await _context.SaveChangesAsync();

            var reference = GenerateReference(TransactionType.Transfer);

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

                return $"Transfer Completed Successfly with Transaction Reference {reference}";
                }

            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> MakeWithdrawal(int walletId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Invalid amount");

            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null || wallet.Status != WalletStatus.Active)
                throw new Exception("Invalid wallet");

            if (wallet.Balance < amount)
                throw new Exception("Insufficient balance");

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
    }
}
