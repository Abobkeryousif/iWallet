
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

            var userChack = await _context.Wallets.FirstOrDefaultAsync(u=> u.Id == walletDto.UserId);
            if (userChack == null)
                throw new Exception("Invalid user id");

            if (!Enum.IsDefined(typeof(WalletType), walletDto.WalletType))
                throw new Exception("Invalid wallet type");

            //ensure any user just have 2 wallet
            var walletCount = await _context.Wallets
                .CountAsync(w=> w.UserId == userChack.Id);

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

        public async Task<GetWalletDto> GetWalletById(int walletId)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
                throw new Exception($"not found wallet with id: {walletId}");

            var getWalletDto = new GetWalletDto
            {
                WalletNumber = wallet.WalletNumber,
                Balance = wallet.Balance,
                Status = wallet.Status.ToString(),
                WalletType = wallet.WalletType.ToString()
            };

            return getWalletDto;
        }

        public async Task<string> PatchWalletBalance(int walletId, decimal balance)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
                 throw new Exception($"not found wallet with id: {walletId}");

            if (balance <= 0)
                throw new Exception("invalid balance!");

            wallet.Balance += balance;
            wallet.UpdatedAt = DateTime.UtcNow;
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();  

            return $"your updated balance is {wallet.Balance}";
        }

        public async Task<List<GetWalletDto>> GetWalletsAsync()
        {
            //fix performance to filter in database level and just get filterd data to local

            var wallets = await _context.Wallets
                .Select(w => new GetWalletDto
                {
                    WalletNumber = w.WalletNumber,
                    Balance = w.Balance,
                    WalletType = w.WalletType.ToString(),
                    Status = w.Status.ToString()
                })
                .ToListAsync();

            return wallets;
        }
    }
    }

