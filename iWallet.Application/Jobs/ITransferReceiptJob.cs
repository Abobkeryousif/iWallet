
namespace iWallet.Application.Jobs
{
    public interface ITransferReceiptJob
    {
        Task GenerateReceiptAsync(string reference);
    }
}
