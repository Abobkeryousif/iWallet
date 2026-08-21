
namespace iWallet.Application.Validator
{
    public class WithdrawalValidator : AbstractValidator<WithdrawalDto>
    {
        public WithdrawalValidator()
        {
            RuleFor(a => a.amount)
                .ValidAmount();
        }
    }
}
