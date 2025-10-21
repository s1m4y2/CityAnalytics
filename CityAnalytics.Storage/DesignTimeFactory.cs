using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CityAnalytics.Storage;

public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=cityanalytics.db")
            .Options;
        return new AppDbContext(opt);
    }
}
