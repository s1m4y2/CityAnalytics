using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;

namespace CityAnalytics.Analytics;

public class CorrelationAnalyzer
{
    private readonly AppDbContext _db;
    public CorrelationAnalyzer(AppDbContext db) => _db = db;

    // institution boş bırakılırsa tüm kurumlar bir arada ele alınır (şehir geneli)
    public async Task<object> GetCorrelationAsync(string? institution, DateTime? from, DateTime? to)
    {
        var q = _db.DailyInstitutionUsages.AsQueryable();

        if (!string.IsNullOrWhiteSpace(institution))
            q = q.Where(x => x.Institution.Contains(institution));
        if (from.HasValue) q = q.Where(x => x.Date >= from.Value);
        if (to.HasValue) q = q.Where(x => x.Date <= to.Value);

        // Gün bazında topla ki her gün bir gözlem (vektör) olsun
        var rows = await q
            .GroupBy(x => x.Date)
            .Select(g => new
            {
                Date = g.Key,
                FullFare = g.Sum(x => x.FullFare),
                Student = g.Sum(x => x.Student),
                Teacher = g.Sum(x => x.Teacher),
                Sixty = g.Sum(x => x.SixtyYearsOld),
                Ticket = g.Sum(x => x.Ticket),
                Child = g.Sum(x => x.Child),
                Personnel = g.Sum(x => x.Personnel),
                Free = g.Sum(x => x.Free),
                BankCard = g.Sum(x => x.BankCard)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var labels = new[] { "FullFare", "Student", "Teacher", "Sixty", "Ticket", "Child", "Personnel", "Free", "BankCard" };

        // Gözlem yoksa boş dön
        if (rows.Count == 0)
            return new { labels, matrix = Array.Empty<double[]>() };

        // Kolonları vektörlere aç
        var cols = new List<double[]>
        {
            rows.Select(r => (double)r.FullFare).ToArray(),
            rows.Select(r => (double)r.Student).ToArray(),
            rows.Select(r => (double)r.Teacher).ToArray(),
            rows.Select(r => (double)r.Sixty).ToArray(),
            rows.Select(r => (double)r.Ticket).ToArray(),
            rows.Select(r => (double)r.Child).ToArray(),
            rows.Select(r => (double)r.Personnel).ToArray(),
            rows.Select(r => (double)r.Free).ToArray(),
            rows.Select(r => (double)r.BankCard).ToArray()
        };

        // Pearson korelasyon matrisi
        int n = cols.Count;
        var M = new double[n][];
        for (int i = 0; i < n; i++)
        {
            M[i] = new double[n];
            for (int j = 0; j < n; j++)
                M[i][j] = Pearson(cols[i], cols[j]);
        }

        return new { labels, matrix = M };
    }

    private static double Pearson(double[] a, double[] b)
    {
        int n = Math.Min(a.Length, b.Length);
        if (n == 0) return 0;

        double meanA = a.Average();
        double meanB = b.Average();

        double num = 0, denA = 0, denB = 0;
        for (int i = 0; i < n; i++)
        {
            var da = a[i] - meanA;
            var db = b[i] - meanB;
            num += da * db;
            denA += da * da;
            denB += db * db;
        }
        var den = Math.Sqrt(denA) * Math.Sqrt(denB);
        if (den == 0) return 0; // varyansı sıfır kolonlar
        return num / den;       // -1 .. +1
    }
}
