using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Services;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------
// Load RSA PUBLIC KEY for verifying JWT
// -----------------------------------------
var publicKeyText = File.ReadAllText("keys/auth_public_key.pem");
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyText);

var rsaSecurityKey = new RsaSecurityKey(rsa);

// -----------------------------------------
// JWT Auth
// -----------------------------------------
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
            IssuerSigningKey = rsaSecurityKey
        };
    });

builder.Services.AddAuthorization();

// -----------------------------------------
// DB + Services
// -----------------------------------------
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITransactionService, TransactionService.Services.TransactionService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
