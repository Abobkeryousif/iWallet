namespace iWallet.Application.iWalletMetrics
{
    public static class DepositMetrics
    {
        
        public static readonly Counter DepositsTotal =
            Metrics.CreateCounter("iwallet_deposit_total",
              "Total number of successful wallet deposits");

        public static readonly Counter DepositsFailuresTotal =
            Metrics.CreateCounter("iwallet_deposit_failures_total",
                "Total number of failures wallet deposits");

            
    }
}
