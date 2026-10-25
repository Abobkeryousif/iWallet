
namespace iWallet.Application.Helper
{
    public static class BusinessMetricsHelper
    {
        public static void RecordBusinessOperation(string module, string operation, string status)
        {
            var tags = new TagList
            {
                {"module", module },
                {"operation", operation },
                {"status", status }
            };

            iWalletBusinessMetrics.TotalOperations.Add(1, tags);
        }

        public static void RecordBusinessOperationDuration(string module, string operation, double durationInMs)
        {
            var tags = new TagList
            {
                {"module", module},
                {"operation", operation }
            };

            iWalletBusinessMetrics.OperationDuration.Record(durationInMs, tags);
        }

    }
}
