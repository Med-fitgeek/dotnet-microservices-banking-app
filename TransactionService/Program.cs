using TransactionService.Data;
using TransactionService.Messaging;
using TransactionService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Database
// ------------------------------
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITransactionService, TransactionServiceImpl>();

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

builder.Services.AddSingleton<IMessagePublisher, RabbitPublisher>();

// ------------------------------
// JWT Auth
// ------------------------------
var publicKeyPath = builder.Configuration["Jwt:PublicKeyPath"];
var publicKeyText = File.ReadAllText(publicKeyPath);
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyText);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
