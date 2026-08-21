namespace iWallet.Application.Validator
{
    public class MakeDepositValidator : AbstractValidator<DepositDto>
    {
        public MakeDepositValidator()
        {
            RuleFor(a => a.amount)
                .ValidAmount();
        }
    }
}
