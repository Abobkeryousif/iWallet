
namespace iWallet.Infrastructure.Implemention
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILimitService _limitService;
        private readonly ILogger<TransactionRepository> _logger;

        const string module = "transaction";
        public TransactionRepository(ILogger<TransactionRepository> logger ,ApplicationDbContext context, ILimitService limitService)
        {
            _context = context;
            _limitService = limitService;
            _logger = logger;
        }


        public async Task<Result<string>> MakeDepositAsync(DepositDto depositDto, int userId)
        {
            const string depositOperation = "deposit";
            using var activity = TransactionActivitySource.Source.StartActivity("transaction.deposit");

            activity?.SetTag("transaction.type", "deposit");
            activity?.SetTag("wallet.id", depositDto.walletId);

            Stopwatch stopwatch = Stopwatch.StartNew();
             

            var wallet = await _context.Wallets
                .Where(d=> d.Id == depositDto.walletId && d.UserId == userId && d.Status == WalletStatus.Active)
                .FirstOrDefaultAsync();


            if (wallet == null)
            {
                _logger.LogWarning("deposit rejected because wallet was not found or is inactive. WalletId: {WalletId}", depositDto.walletId);
                activity?.SetTag("transaction.outcome", "failure");
                BusinessMetricsHelper.RecordBusinessOperation(module, depositOperation, "failure");

                return Result<string>.Failure(TransactionSharedErrors.InValidUserWallet());
            }

            var reference = GenerateReference(TransactionType.Deposit);

            using var depositTransaction = await _context.Database.BeginTransactionAsync();
            activity?.AddEvent(new ActivityEvent("deposit.db_transaction.started"));

            try
            {

                var transaction = new Transaction
                {
                    FromWalletId = depositDto.walletId,
                    Reference = reference,
                    Amount = depositDto.amount,
                    TransactionType = TransactionType.Deposit,
                    Status = TransactionStatus.Success,
                };

                wallet.Balance += depositDto.amount;

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

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

                await depositTransaction.CommitAsync();

                
                _logger.LogInformation("deposit amount: {DepositAmount} to wallet: {WalletNumber}", depositDto.amount, depositDto.walletId);
                activity?.SetTag("transaction.outcome", "success");

                BusinessMetricsHelper.RecordBusinessOperation(module, depositOperation, "success");
                BusinessMetricsHelper.RecordBusinessOperationDuration(module, depositOperation, stopwatch.ElapsedMilliseconds);

                return Result<string>.Success($"Successfly Deposit with Ammount {transaction.Amount} and total wallet balance = {wallet.Balance}");
            }


            catch (Exception ex) {
                await depositTransaction.RollbackAsync();

                _logger.LogError(ex ,"deposit failed walletId: {WalletId}, amount: {Amount}, reference: {Reference}", 
                        depositDto.walletId, depositDto.amount, reference);

                activity?.SetTag("transaction.outcome", "failure");
                activity?.SetStatus(ActivityStatusCode.Error, "deposit failed");

                BusinessMetricsHelper.RecordBusinessOperation(module, depositOperation, "failure");
                BusinessMetricsHelper.RecordBusinessOperationDuration(module, depositOperation, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }


        public async Task<Result<string>> TransferAsync( string toAccountNumber ,decimal amount, int userId)
        {
            const string transferOperation = "transfer";

            using var activity = TransactionActivitySource.Source
                .StartActivity("transaction.transfer");

            activity?.AddEvent(new ActivityEvent("start.transfer.process"));

            Stopwatch st = Stopwatch.StartNew();

            _logger.LogInformation("Starting Transfer Process. UserId: {UserId}, ToAccount: {ToAccount}, Amount: {Amount}" ,userId, toAccountNumber, amount);

            if (amount <= 0)
                return Result<string>.Failure(TransferErrors.Conflict());

            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

            if (senderWallet == null || senderWallet.Status != WalletStatus.Active)
            {
                Log.Warning("Sender wallet not found or not active. UserId: {UserId}", userId);
                activity?.SetTag("transaction.outcome", "failure");
                BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

                return Result<string>.Failure(TransactionSharedErrors.InValidUserWallet());
            }

            var receiverWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.WalletNumber == toAccountNumber);

            if (receiverWallet == null || receiverWallet.Status != WalletStatus.Active)
            {
                Log.Warning("Receiver wallet not found or not active. UserId: {UserId}", userId);
                activity?.SetTag("transaction.outcome", "failure");

                BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

                return Result<string>.Failure(TransferErrors.InValidReceiverWallet());
            }

            if (senderWallet.Id == receiverWallet.Id)
                return Result<string>.Failure(TransferErrors.Conflict());

            if (senderWallet.Balance < amount)
            {
                _logger.LogWarning("InSufficient Balance to transfer");
                activity?.SetTag("transaction.outcome", "failure");

                BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

                return Result<string>.Failure(TransactionSharedErrors.InsufficientBalance());
            }

            activity?.SetTag("sender_wallet.id", senderWallet.Id);
            activity?.SetTag("receiver_wallet.id", receiverWallet.Id);

            var limit = await _limitService.GetUserLimitAsync(senderWallet.UserId);

            var todayTotal = await GetTransactionsTodayTotalAsync(senderWallet.UserId);

            if (todayTotal + amount > limit.DailyLimit)
            {
                _logger.LogWarning("user {UserId} is Exceeded Daily limit", userId);
                activity?.SetTag("transaction.outcome", "failure");

                BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

                return Result<string>.Failure(TransferErrors.ExceededDailylimit());
            }

            var reference = GenerateReference(TransactionType.Transfer);

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            activity?.AddEvent(new ActivityEvent("transfer.db_transaction.started"));

            try
            {
                
                senderWallet.Balance -= amount;
                receiverWallet.Balance += amount;

                senderWallet.UpdatedAt = DateTime.UtcNow;
                receiverWallet.UpdatedAt = DateTime.UtcNow;

                activity?.AddEvent(new ActivityEvent("transfer.balances.updated"));

                
                var transaction = new Transaction
                {
                    FromWalletId = senderWallet.Id,
                    ToWalletId = receiverWallet.Id,
                    Amount = amount,
                    TransactionType = TransactionType.Transfer,
                    Reference = reference,
                    Status = TransactionStatus.Success
                };

                
                var senderLedger = new LedgerEntry
                {
                    WalletId = senderWallet.Id,
                    Transaction = transaction,
                    Debit = amount,
                    Credit = 0,
                    Particulars = $"Transfer to {receiverWallet.WalletNumber}" //edit message
                };

                var receiverLedger = new LedgerEntry
                {
                    WalletId = receiverWallet.Id,
                    Transaction = transaction,
                    Debit = 0,
                    Credit = amount,
                    Particulars = $"Successfully received transfer from {senderWallet.WalletNumber}"
                };

               
                _context.Transactions.Add(transaction);

                // one add for both ledgers
                _context.LedgerEntries.AddRange(senderLedger, receiverLedger);

               // Save all change in one call 
                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();

                activity?.AddEvent(new ActivityEvent("transfer.db_transaction.completed"));

                try
                {
                    BackgroundJob.Enqueue<ITransferReceiptJob>(
                        job => job.GenerateReceiptAsync(reference));
                }
                catch (Exception jobException)
                {
                    _logger.LogError(jobException, "Failed to enqueue transfer receipt. Reference: {Reference}" ,reference);
                }

                _logger.LogInformation("Transfer completed successfully. Reference: {Reference}, Amount: {Amount}" ,reference ,amount);

                activity?.SetTag("transaction.outcome", "success");

                activity?.AddEvent(new ActivityEvent("end.transfer.process.successfully"));

                BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "success");

                BusinessMetricsHelper.RecordBusinessOperationDuration(module, transferOperation, st.ElapsedMilliseconds);

                return Result<string>.Success($"Transfer Completed Successfully with Transaction Reference {reference}");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();

                _logger.LogError(ex, "Transfer failed. UserId: {UserId}, Amount: {Amount}, Reference: {Reference}" ,userId, amount, reference);

                activity?.SetTag("transaction.outcome", "failure");
                activity?.SetStatus(ActivityStatusCode.Error, "Transfer failed");

                BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

                throw;
            }
        }

        public async Task<Result<string>> MakeWithdrawal(int walletId, int userId, decimal amount)
        {
            const string withdrawalOperation = "withdrawal";
            using var activity = TransactionActivitySource.Source.StartActivity("transaction.withdrawal");
            activity?.SetTag("wallet.id", walletId);

            Stopwatch sw = Stopwatch.StartNew();
            

            var wallet = await _context
                .Wallets
                .Where(w=> w.Id == walletId &&
                    w.UserId == userId &&
                    w.Status == WalletStatus.Active)
                .FirstOrDefaultAsync();


            if (wallet == null || wallet.Status != WalletStatus.Active)
            {
                _logger.LogWarning("Withdrawal rejected because wallet was not found or is inactive. WalletId: {WalletId}", walletId);
                activity?.SetTag("transaction.outcome", "failure");
                BusinessMetricsHelper.RecordBusinessOperation(module, withdrawalOperation, "failure");

                return Result<string>.Failure(TransactionSharedErrors.InValidUserWallet());
            }

            if (wallet.Balance < amount)
            {
                _logger.LogWarning("Withdrawal rejected due to insufficient balance. WalletId: {WalletId}",walletId);
                activity?.SetTag("transaction.outcome", "failure");

                BusinessMetricsHelper.RecordBusinessOperation(module, withdrawalOperation, "failure");

                return Result<string>.Failure(TransactionSharedErrors.InsufficientBalance());
            }

            var reference = GenerateReference(TransactionType.Withdrawal);

            var withdrawalTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                wallet.Balance -= amount;
                wallet.UpdatedAt = DateTime.UtcNow;

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

                await withdrawalTransaction.CommitAsync();

                _logger.LogInformation("Withdrawal completed successfully. TransactionReference: {TransactionReference}, Amount: {Amount}", reference, amount);
                activity?.SetTag("transaction.outcome", "success");

                BusinessMetricsHelper.RecordBusinessOperation(module, withdrawalOperation, "success");
                BusinessMetricsHelper.RecordBusinessOperationDuration(module, withdrawalOperation, sw.ElapsedMilliseconds);
                return Result<string>.Success($"withdrawal completed successfly with Transaction Reference {reference}");
            }

            
            catch (Exception ex) {
                await withdrawalTransaction.RollbackAsync();

                _logger.LogError(ex, "Withdrawal operation failed. WalletId: {WalletId}, Amount: {Amount}, Reference: {Reference}",
                    walletId, amount, reference);

                activity?.SetTag("transaction.outcome", "failure");

                BusinessMetricsHelper.RecordBusinessOperation(module, withdrawalOperation, "failure");
                BusinessMetricsHelper.RecordBusinessOperationDuration(module, withdrawalOperation, sw.ElapsedMilliseconds);

                throw;
            }}

        public async Task<List<TransactionDto>> TransactionHistory(int walletId, int userId)
        {
            var walletExists = await _context.Wallets
                 .AnyAsync(w => w.Id == walletId && w.UserId == userId);

            if (!walletExists)
                throw new Exception("Invalid wallet");

            var fromTransactions = _context.Transactions
                 .AsNoTracking()
                 .Where(t => t.FromWalletId == walletId);

            
            var toTransactions = _context.Transactions
                .AsNoTracking()
                .Where(t => t.ToWalletId == walletId);

            
            var history = await fromTransactions
                .Concat(toTransactions)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionDto
                {
                    Reference = t.Reference,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType.ToString(),
                    TransactionStatus = t.Status.ToString()
                })
                .ToListAsync();


            if (history.Count == 0)
                throw new Exception("No transactions found");

            return history;
        }

        public async Task<Result<string>> TransferToBeneficiery(string beneficieryName, decimal amount, int userId)
        {
            const string beneficeryOperation = "transfer_beneficery";
            
            Stopwatch sw = Stopwatch.StartNew();
            

            var checkBeneficiery = await _context.Beneficiaries.FirstOrDefaultAsync(n => n.Name.ToLower() == beneficieryName.ToLower());
            if (checkBeneficiery == null)
            {
                _logger.LogWarning("not found matched beneficery with name: {BeneficieryName}", beneficieryName);
                
                BusinessMetricsHelper.RecordBusinessOperation(module, beneficeryOperation, "failure");

                return Result<string>.Failure(TransactionSharedErrors.InvalidBeneficieryName());
            }

            await TransferAsync(checkBeneficiery.WalletNumber, amount, userId);

            _logger.LogInformation("Transfer Completed Successfly with amount: {Amount} to beneficiery {BeneficieryName}", amount, beneficieryName);
            

            BusinessMetricsHelper.RecordBusinessOperation(module, beneficeryOperation, "success");
            BusinessMetricsHelper.RecordBusinessOperationDuration(module, beneficeryOperation, sw.ElapsedMilliseconds);

            return Result<string>.Success($"Transfer Completed Successfly with amount {amount}");
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
