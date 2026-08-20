namespace iWallet.Application.iWalletCustomMetrics
{
    public static class WithdrawalMetrics
    {
        public static readonly Counter WithdrawalTotal =
            Metrics.CreateCounter("iwallet_withdrawal_total",
                "total number of success wallet withdrawal");

        public static readonly Counter WithdrawalFailuresTotal =
            Metrics.CreateCounter("iwallet_withdrawal_failures_total",
                "total number of failures wallet withdrawal");
    }
}
