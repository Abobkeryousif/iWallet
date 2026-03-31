
namespace iWallet.Infrastructure.Implemention
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;


        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAsync(CreateWalletDto walletDto)
        {

            if (!Enum.IsDefined(typeof(WalletType), walletDto.WalletType))
                throw new Exception("Invalid wallet type");

            //ensure any user just have 2 wallet
            var walletCount = await _context.Wallets
                .CountAsync(w=> w.UserId == walletDto.UserId);

            if (walletCount >= 2)
                throw new Exception("User can not have more then 2 wallet");


            CreatePinHash(walletDto.pin, out byte[] pinHash, out byte[] pinSalt);
            
            var wallet = new Wallet
            {
                UserId = walletDto.UserId,
                WalletNumber = GenerateWalletNumber(),
                WalletType = walletDto.WalletType,
                Balance = 0,
                PinHash = pinHash,
                PinSalt = pinSalt
            };
            wallet.Status = WalletStatus.Active;
            await _context.AddAsync(wallet);
            await _context.SaveChangesAsync();

            return ($"successfly created wallet! with wallet number {wallet.WalletNumber}");
            
        }

        // account number generation
        private static string GenerateWalletNumber()
        {
            var random = new Random();
            return "W" + random.Next(100000000, 999999999);
        }

        //create and has pin , pin salt
        private static void CreatePinHash(string pin , out byte[] pinHash , out byte[] pinSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            pinSalt = hmac.Key;
            pinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pin));
        }
    }
}
