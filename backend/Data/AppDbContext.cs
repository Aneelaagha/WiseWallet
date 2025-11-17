using Microsoft.EntityFrameworkCore;
using WiseWallet.Models;

namespace WiseWallet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions => Set<Subscription>();
    }
}
