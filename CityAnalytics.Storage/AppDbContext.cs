using CityAnalytics.Shared;
using Microsoft.EntityFrameworkCore;

namespace CityAnalytics.Storage
{
    public class AppDbContext : DbContext
    {
        public DbSet<DailyInstitutionUsage> DailyInstitutionUsages => Set<DailyInstitutionUsage>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyInstitutionUsage>()
                .HasIndex(x => new { x.Institution, x.Date });
        }
    }
}
