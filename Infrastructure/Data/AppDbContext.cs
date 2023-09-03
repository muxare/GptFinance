using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<EodData> EodData { get; set; }
        public DbSet<EmaData> EmaData { get; set; }
        public DbSet<MacdData> MacdData { get; set; }
        //public DbSet<StockExchange> StockExchange { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EodData>()
                .HasKey(o => new { o.Id, o.CompanyId, o.Date });

            modelBuilder.Entity<EmaData>()
                .HasKey(o => new { o.Id, o.CompanyId, o.Date });
            modelBuilder.Entity<MacdData>()
                .HasKey(o => new { o.Id, o.CompanyId, o.Date });
            //modelBuilder.Entity<StockExchange>()
            //    .HasKey(o => new { o.Id });

            //modelBuilder.Entity<StockExchange>().OwnsOne<TradingHours>(o => o.TradingHours);
            //modelBuilder.Entity<StockExchange>().OwnsOne<LunchBreak>(o => o.LunchBreak);

            base.OnModelCreating(modelBuilder);
        }
    }
}
