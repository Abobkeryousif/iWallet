
namespace iWallet.Infrastructure.Implemention
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;


        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAsync(int userId,WalletType walletType,string pin)
        {

            Log.Information("Start create wallet process for UserId: {UserId}",userId);

            //var userChack = await _context.Wallets.FirstOrDefaultAsync(u=> u.Id == userId);
            //if (userChack == null)
            //{
            //    Log.Warning("Invalid UserId: {UserId}",userId);
            //    throw new Exception("Invalid user id");
            //}

            if (!Enum.IsDefined(typeof(WalletType), walletType))
                throw new Exception("Invalid wallet type");

            //ensure any user just have 2 wallet
            var walletCount = await _context.Wallets
                .CountAsync(w=> w.UserId == userId);

            if (walletCount >= 2)
                throw new Exception("User can not have more then 2 wallet");


            CreatePinHash(pin, out byte[] pinHash, out byte[] pinSalt);
            
            var wallet = new Wallet
            {
                UserId = userId,
                WalletNumber = GenerateWalletNumber(),
                WalletType = walletType,
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
            pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
        }

        public async Task<GetWalletDto> GetWalletById(int walletId, int userId, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .Where(wi=> wi.Id == walletId && wi.UserId == userId)
                .Select(w=> new GetWalletDto
                {
                    WalletNumber = w.WalletNumber,
                    Balance = w.Balance,
                    Status = w.Status,
                    WalletType = w.WalletType
                })
                .FirstOrDefaultAsync(cancellationToken);
                
   
            if (wallet == null)
                throw new Exception($"not found wallet with id: {walletId}");

            return wallet;
        }

        public async Task<string> PatchWalletBalance(int walletId, int userId, decimal balance)
        {
            var wallet = await _context.Wallets
                .Where(w => w.Id == walletId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (wallet == null)
                 throw new Exception($"not found wallet with id: {walletId}");

            if (balance <= 0)
                throw new Exception("invalid balance!");

            wallet.Balance += balance;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();  

            return $"your updated balance is {wallet.Balance}";
        }

        public async Task<List<GetWalletDto>> GetWalletsAsync(CancellationToken cancellationToken)
        {
            //fix performance to filter in database level and just get filterd data to local

            var wallets = await _context.Wallets
                .AsNoTracking()
                .Select(w => new GetWalletDto
                {
                    WalletNumber = w.WalletNumber,
                    Balance = w.Balance,
                    WalletType = w.WalletType,
                    Status = w.Status
                })
                .ToListAsync(cancellationToken);

            return wallets;
        }

        public async Task<GetWalletDto> GetByWalletNumber(string walletNumber, int userId, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .Where(wn => wn.WalletNumber == walletNumber && wn.Status == WalletStatus.Active && wn.UserId == userId)
                .Select(w => new GetWalletDto
                {
                    WalletNumber = w.WalletNumber,
                    Balance = w.Balance,
                    Status = w.Status,
                    WalletType = w.WalletType
                }).FirstOrDefaultAsync(cancellationToken);

            if (wallet == null)
                throw new Exception("invalid wallet number or InActive Wallet");

            return wallet;
        }

        public async Task<List<GetWalletDto>> GetUserWalletsAsync(int userId, CancellationToken cancellationToken)
        {
            var result = await _context.Wallets
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .Select(w => new GetWalletDto
                {
                    WalletNumber = w.WalletNumber,
                    Balance = w.Balance,
                    WalletType = w.WalletType,
                    Status = w.Status
                }).ToListAsync(cancellationToken);

            if (result.Count == 0)
                throw new ArgumentNullException("user does't have any wallet!");

            return result;
        }
    }
    }
