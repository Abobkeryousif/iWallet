
namespace iWallet.Infrastructure.Implemention
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        private readonly ApplicationDbContext _context;
        public BeneficiaryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<string> AddBeneficiary(string benficiaryName, string walletNumber, int userId)
        {
            if (string.IsNullOrWhiteSpace(benficiaryName))
                throw new Exception("please add beniciary name");

            var walletCheck = await _context.Wallets.FirstOrDefaultAsync(wn=> wn.WalletNumber == walletNumber);
            if (walletCheck == null || walletCheck.Status != WalletStatus.Active)
                throw new Exception("invalid beneficiary wallet");

            if (walletCheck.UserId == userId)
                throw new Exception("you can't add yourself as beneficiary");

            var beneficiary = new Beneficiary
            {
                Name = benficiaryName,
                WalletNumber = walletNumber,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Beneficiaries.Add(beneficiary);
            _context.SaveChanges();

            return $" Successfly Add beneficiery with name: {beneficiary.Name} and walletNumber {beneficiary.WalletNumber}";
        }

        public void DeleteBeneficiery(int beneficierId)
        {
           var deletedBeneficiery = _context.Beneficiaries.Find(beneficierId);
            if (deletedBeneficiery == null)
                throw new Exception($"not found any beneficiery");

            _context.Beneficiaries.Remove(deletedBeneficiery);
            _context.SaveChanges();
        }

        public async Task<List<BeneficieryDto>> GetBeneficiers(int userId)
        {
            var user = await _context.Beneficiaries.AnyAsync(u=> u.UserId == userId);
            if (!user)
                throw new Exception($"not found user with this id: {userId}");

            var userBeneficiery = await _context.Beneficiaries
                .Where(u=> u.UserId == userId)
                .OrderByDescending(u=>u.CreatedAt)
                .Select(b=> new BeneficieryDto
                {
                    Name=b.Name,
                    WalletNumber =b.WalletNumber,
                }).ToListAsync();

            if (userBeneficiery.Count == 0)
                throw new Exception("not added beneficiers yet");

            return userBeneficiery;
        }

        public string UpdateBeneficiaryName(int beneficierId, string updatedName)
        {
            var beneficiary =  _context.Beneficiaries.Find(beneficierId);
            if (beneficiary == null)
                throw new Exception("not found any beneficiery");

            if (string.IsNullOrWhiteSpace(updatedName))
                throw new Exception("invalid name");

            beneficiary.Name = updatedName;
            _context.Update(beneficiary);
            _context.SaveChanges();

            return $"updated beneficiery name to {beneficiary.Name}" ;
        }
    }
}
