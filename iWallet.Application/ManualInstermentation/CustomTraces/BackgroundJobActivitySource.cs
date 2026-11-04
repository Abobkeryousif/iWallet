
namespace iWallet.Application.ManualInstermentation.CustomTraces
{
    public static class BackgroundJobActivitySource
    {
        public const string SourceName = "iwallet.background_job";

        public static readonly ActivitySource Source = new(SourceName);
    }
}
