namespace iWallet.Application.Errors
{
    public class TransactionSharedErrors
    {
        public static ServiceError InValidUserWallet() =>
          new(
              "Transaction.InValidUserWallet",
              "Invalid Sender Wallet",
              ErrorType.Validation
              );

        public static ServiceError InsufficientBalance() =>
            new(
                "Transaction.InsufficientBalance",
                "Insufficient Balance from user wallet",
                ErrorType.Conflict
                );
    }
}
