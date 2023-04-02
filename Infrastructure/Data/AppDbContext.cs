using GptFinance.Domain.Entities;

namespace YahooFinanceAPI.Data
{
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<EodData> EodData { get; set; }
        public DbSet<EmaData> EmaData { get; set; }
        public DbSet<MacdData> MacdData { get; set; }
    }
}
