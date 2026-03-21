

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.InfrastructureReigster();

builder.Services.AddDbContext<ApplicationDbContext>
    (option=> option.UseSqlServer(builder.Configuration.GetConnectionString("default")));

builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));


builder.Services.AddControllers()
    .AddFluentValidation(validation => validation.RegisterValidatorsFromAssemblyContaining<UserRegisterValidator>());

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
