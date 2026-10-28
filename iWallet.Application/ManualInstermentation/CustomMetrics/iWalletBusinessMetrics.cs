
namespace iWallet.Application.ManualInstermentation.iWalletBusinessMetrics
{
    public static class iWalletBusinessMetrics
    {
        public const string meterName = "iWallet.Business";

       private readonly static Meter Meter = new Meter(meterName);

        public static readonly Counter<long> TotalOperations = Meter.CreateCounter<long>(
            "iwallet.business.operations",
            unit: "{operation}",
            description: "total number of business operation executed in iwallet app"
            );

        public static readonly Histogram<double> OperationDuration = Meter.CreateHistogram<double>(
            "iwallet.business.operation.duration",
            unit: "{s}",
            description: "duration of business operation"
            );
    }
}
