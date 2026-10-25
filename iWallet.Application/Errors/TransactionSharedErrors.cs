namespace iWallet.Application.Errors
{
    public class TransactionSharedErrors
    {
        public static ServiceError InValidUserWallet() =>
          new(
              "Transaction.InValidUserWallet",
              "Invalid User Wallet",
              ErrorType.Validation
              );

        public static ServiceError InsufficientBalance() =>
            new(
                "Transaction.InsufficientBalance",
                "Insufficient Balance from user wallet",
                ErrorType.Conflict
                );

        public static ServiceError InvalidBeneficieryName() =>
            new(
                "Transaction.InvalidBeneficieryName",
                "invalid beneficiery name",
                ErrorType.NotFound
                );
    }
}
