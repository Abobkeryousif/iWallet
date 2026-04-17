namespace iWallet.Application.Interface
{
    public interface IBeneficiaryRepository
    {
        Task<string> AddBeneficiary(string benficiaryName, string walletNumber, int userId);
    }
}
