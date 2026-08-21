namespace iWallet.Application.Validator
{
    public class TransferValidator : AbstractValidator<TransferTransactionDto>
    {
        public TransferValidator()
        {
            RuleFor(a=> a.amount)
                .ValidAmount();

            RuleFor(a => a.amount)
                .LessThan(5000)
                .WithMessage("Exceeded per transaction Daily limit 5000");
        }
    }
}
