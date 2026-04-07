namespace iWallet.Infrastructure.Implemention
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task MakeDepositAsync(int walletId, decimal ammount)
        {
            if (ammount <= 0)
                throw new Exception("invalid amount");

            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == walletId);
                if (wallet == null || wallet.Status != WalletStatus.Active)
                    throw new Exception("invalid wallet");

            var transaction = new Transaction
            {
                FromWalletId = walletId,
                Reference = "Make Deposit to wallet",
                Amount = ammount,
                TransactionType = TransactionType.Deposit,
                Status = TransactionStatus.Success,
            };


        }
    }
}
