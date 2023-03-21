namespace YahooFinanceAPI.Data
{
    using Microsoft.EntityFrameworkCore;
    using YahooFinanceAPI.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<EODData> EODData { get; set; }
        public DbSet<EMAData> EMAData { get; set; }
        public DbSet<MACDData> MACDData { get; set; }
    }
}
