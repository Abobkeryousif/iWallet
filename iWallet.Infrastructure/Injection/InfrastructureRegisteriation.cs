
namespace iWallet.Infrastructure.Injection
{
    public static class InfrastructureRegisteriation
    {
        public static IServiceCollection InfrastructureReigster(this IServiceCollection services)
        {
            services.AddScoped<IUnitofwork, Unitofwork>();
            services.AddTransient(typeof(IIsExistMethod<>),typeof(IsExistMethod<>));


            return services;
        }
    }
}