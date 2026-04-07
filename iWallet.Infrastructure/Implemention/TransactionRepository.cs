namespace iWallet.Infrastructure.Implemention
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletRepository _walletRepository;
        public TransactionRepository(ApplicationDbContext context, IWalletRepository walletRepository)
        {
            _context = context;
            _walletRepository = walletRepository;
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

            await _walletRepository.PatchWalletBalance(walletId, transaction.Amount);


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
            _context.SaveChanges();

            return $"Successfly Deposit with Ammount {transaction.Amount} and total wallet balance = {wallet.Balance}";

        }
       
        
        private static string GenerateReference(TransactionType type)
        {
            var data = DateTime.UtcNow.ToString("yyMMdd");
            var random = Guid.NewGuid().ToString("N").Substring(0,6).ToUpper();

            var prefix = type switch
            {
                TransactionType.Deposit => "DEP",
                TransactionType.Transfer => "TRN",
                TransactionType.Withdrawal => "WDR",
                _=> "UNK"
            };

            return $"{prefix}-{data}-{random}";
        }
    }
}
