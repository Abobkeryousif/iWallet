
namespace iWallet.Application.ManualInstermentation.CustomTraces
{
    public class TransactionActivitySource
    {
        public const string SourceName = "iWallet.Transaction";
        public static readonly ActivitySource Source = new ActivitySource(SourceName); 
    }
}
