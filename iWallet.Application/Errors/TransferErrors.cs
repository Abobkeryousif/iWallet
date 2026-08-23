
namespace iWallet.Application.Errors
{
    public static class TransferErrors 
    {
      
        public static ServiceError InValidReceiverWallet() =>
            new(
                "Transfer.InValidReceiverWallet",
                "Invalid Receiver Wallet",
                ErrorType.Validation
                );

        public static ServiceError Conflict() =>
            new(
                "Transfer.Conflict",
                "Sender and receiver wallets cannot be the same",
                ErrorType.Conflict
                );

        public static ServiceError ExceededDailylimit() =>
            new(
                "Transfer.ExceededDailylimit",
                "You already Exceeded daily limit 20000",
                ErrorType.Conflict
                );

    }
}
