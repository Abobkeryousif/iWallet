namespace iWallet.Application.Validator
{
    public static class AmountValidationExtension
    {
        public static IRuleBuilder<T, decimal> ValidAmount<T>(
            this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0)
                .WithMessage("amount must be greater then 0");
        }
    }
}
