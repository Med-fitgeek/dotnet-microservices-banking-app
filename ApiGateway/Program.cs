using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;


var builder = WebApplication.CreateBuilder(args);

// config
var config = builder.Configuration;
var jwtPubPath = config["Jwt:PublicKeyPath"];
var issuer = config["Jwt:Issuer"];
var audience = config["Jwt:Audience"];
var publicKeyText = File.ReadAllText(jwtPubPath);
var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyText);
var rsaKey = new RsaSecurityKey(rsa);

// Auth: validate JWT at gateway (RS256)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = rsaKey,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(config.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        // Always remove hop-by-hop headers that break proxies
        builderContext.AddRequestTransform(context =>
        {
            context.ProxyRequest.Headers.Remove("Cookie");
            return default;
        });

        // Inject common forwarded headers from user claims (if present)
        builderContext.AddRequestTransform(context =>
        {
            var httpContext = context.HttpContext;
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var sub = httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.FindFirst("userId")?.Value;
                var fullName = httpContext.User.FindFirst("fullName")?.Value;
                var role = httpContext.User.FindFirst("role")?.Value ?? httpContext.User.FindFirst("roles")?.Value;

                if (!string.IsNullOrEmpty(sub))
                {
                    context.ProxyRequest.Headers.Remove("X-User-Id");
                    context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", sub);
                }

                if (!string.IsNullOrEmpty(fullName))
                {
                    context.ProxyRequest.Headers.Remove("X-User-FullName");
                    context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-FullName", fullName);
                }

                if (!string.IsNullOrEmpty(role))
                {
                    context.ProxyRequest.Headers.Remove("X-User-Role");
                    context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Role", role);
                }
            }

            // pas de return
            return ValueTask.CompletedTask;
        });

    });

// Health checks & routing
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");

// Proxy endpoints are handled by YARP middleware
app.MapReverseProxy();

app.Run();
