using AccountService.Data;
using AccountService.Messaging;
using AccountService.Services;
using AccountService.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Database
// ------------------------------
builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAccountService, AccountServiceImpl>();

// ------------------------------
// RabbitMQ
// ------------------------------
builder.Services.AddSingleton<IConnection>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var factory = new ConnectionFactory
    {
        HostName = cfg["RabbitMq:Host"],
        Port = int.Parse(cfg["RabbitMq:Port"]),
        UserName = cfg["RabbitMq:User"],
        Password = cfg["RabbitMq:Password"]
    };
    return factory.CreateConnection();
});

// Le consumer va créer les queues/exchanges
builder.Services.AddHostedService<TransactionConsumer>();

// ------------------------------
// JWT Auth
// ------------------------------
var publicKeyPath = builder.Configuration["Jwt:PublicKeyPath"];
var publicKeyText = File.ReadAllText(publicKeyPath);
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyText);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "DineroBank.AuthService",
            ValidAudience = "DineroBank",
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
