
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.InfrastructureReigster(builder.Configuration);

//registered and signing serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IGetUserIdFromToken, GetUserIdFromToken>();

//apply rate limiting per user ip address to provide more advance security layer for our APIs
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10)
            }));
});
builder.Services.AddDbContext<ApplicationDbContext>
    (option=> option.UseSqlServer(builder.Configuration.GetConnectionString("default")));

builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
}).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
                };

                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["ACCESS_TOKEN"];
                        return Task.CompletedTask;
                    }
                };
            });


builder.Services.AddControllers()
    .AddFluentValidation(validation => validation.RegisterValidatorsFromAssemblyContaining<UserRegisterValidator>());


// to convert from enum to string, instead of make it inside db query, i make it globally form program.cs to save performance
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddSingleton<IConnectionMultiplexer>(connection=>
{
    var config = builder.Configuration.GetConnectionString("redis");
    return ConnectionMultiplexer.Connect(config!);
});

var app = builder.Build();

//await app.Services.ApplyMigrationAsync<ApplicationDbContext>();


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.MapMetrics();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
