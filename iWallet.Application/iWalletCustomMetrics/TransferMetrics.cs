namespace iWallet.Application.iWalletCustomMetrics
{
    public static class TransferMetrics
    {
        public static readonly Counter TransferTotal =
            Metrics.CreateCounter("iwallet_transactions_total",
                "total numbers of success wallet transactions");

        public static readonly Counter TransferFailureTotal =
            Metrics.CreateCounter("iwallet_transactions_failures_total",
                "total numbers of failuers wallet transaction");
    }
}
