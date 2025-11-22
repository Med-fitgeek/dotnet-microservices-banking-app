using AccountService.Services;
using AccountService.Services.Impl;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAccountService, AccountServiceImpl>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();