

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


            services.AddAuthentication(option=>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie()
            .AddJwtBearer(options=>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
                };
            });


            return services;
        }
    }
}