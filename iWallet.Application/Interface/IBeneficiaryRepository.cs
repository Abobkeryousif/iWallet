namespace iWallet.Application.Interface
{
    public interface IBeneficiaryRepository
    {
        Task<string> AddBeneficiary(string benficiaryName, string walletNumber, int userId);
        string UpdateBeneficiaryName(int beneficierId, string updatedName);
        Task<List<BeneficieryDto>> GetBeneficiers(int userId);
        void DeleteBeneficiery(int beneficierId);
    }
}
