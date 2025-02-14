using Microsoft.EntityFrameworkCore;
using MunicipalTaxManager.Models;

namespace MunicipalTaxManager.DataLayer
{
    public class TaxDbContext : DbContext
    {
        public TaxDbContext(DbContextOptions<TaxDbContext> options)
            : base(options) { }

        public DbSet<TaxRecord> TaxRecords => Set<TaxRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexing municipality + date range
            modelBuilder.Entity<TaxRecord>()
                .HasIndex(x => new { x.Municipality, x.StartDate, x.EndDate });
        }
    }
}
