using AccountService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
        optionsBuilder.UseNpgsql("Server=localhost;Database=AccountServiceDB;User ID=postgres;Password=1331;TrustServerCertificate=True;");

        return new AccountDbContext(optionsBuilder.Options);
    }
}
