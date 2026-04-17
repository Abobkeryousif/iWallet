
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
    }
}
