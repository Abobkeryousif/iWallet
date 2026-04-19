

namespace iWallet.Infrastructure.Injection
{
    public static class InfrastructureRegisteriation
    {
        public static IServiceCollection InfrastructureReigster(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitofwork, Unitofwork>();
            services.AddTransient(typeof(IIsExistMethod<>),typeof(IsExistMethod<>));
            services.AddTransient<ISendEmailService, SendEmailService>();
            services.AddTransient<IOtpRepository, OtpRepository>();
            services.AddScoped<ILimitService, LimitSerivce>();
            services.AddMemoryCache();




            return services;
        }
    }
}