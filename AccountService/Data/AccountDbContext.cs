using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Data
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<Account>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            return base.SaveChanges();
        }
    }
}
