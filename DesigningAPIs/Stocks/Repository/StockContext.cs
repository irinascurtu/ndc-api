using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Stocks.Repository
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductStock>()
                .HasNoKey();
        }

        public DbSet<ProductStock> ProductStocks { get; set; }
    }
}
