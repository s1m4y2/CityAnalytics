using CityAnalytics.Shared;
using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CityAnalytics.Analytics;

public class InstitutionAnalyzer
{
    private readonly AppDbContext _db;

    public InstitutionAnalyzer(AppDbContext db)
    {
        _db = db;
    }

    // 📅 Günlük toplamlar
    public async Task<IEnumerable<object>> GetDailyTotalsAsync(string? institution, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.DailyInstitutionUsages.AsQueryable();

        if (from.HasValue)
            query = query.Where(x => x.Date >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.Date <= to.Value);

        // 🔹 Verileri belleğe çekiyoruz
        var data = await query.AsNoTracking().ToListAsync();

        // 🔹 Türkçe küçük harf duyarlı filtreleme
        if (!string.IsNullOrEmpty(institution))
        {
            var tr = new CultureInfo("tr-TR");
            var lowerInst = institution.ToLower(tr);
            data = data.Where(x => x.Institution.ToLower(tr).Contains(lowerInst)).ToList();
        }

        return data
            .GroupBy(x => x.Date)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Sum(x => x.FullFare + x.Student + x.Teacher +
                                   x.SixtyYearsOld + x.Ticket + x.Child +
                                   x.Personnel + x.Free + x.BankCard)
            })
            .OrderBy(x => x.Date)
            .ToList();
    }

    // 🗓️ Aylık toplamlar
    public async Task<IEnumerable<object>> GetMonthlyTotalsAsync(string? institution, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.DailyInstitutionUsages.AsQueryable();

        if (from.HasValue)
            query = query.Where(x => x.Date >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.Date <= to.Value);

        var data = await query.AsNoTracking().ToListAsync();

        if (!string.IsNullOrEmpty(institution))
        {
            var tr = new CultureInfo("tr-TR");
            var lowerInst = institution.ToLower(tr);
            data = data.Where(x => x.Institution.ToLower(tr).Contains(lowerInst)).ToList();
        }

        return data
            .GroupBy(x => new { x.Date.Year, x.Date.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Total = g.Sum(x => x.FullFare + x.Student + x.Teacher +
                                   x.SixtyYearsOld + x.Ticket + x.Child +
                                   x.Personnel + x.Free + x.BankCard)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();
    }

    // 🏆 En çok kullanılan kurumlar
    public async Task<IEnumerable<object>> GetTopInstitutionsAsync(DateTime? from, DateTime? to)
    {
        var query = _db.DailyInstitutionUsages.AsQueryable();

        if (from.HasValue)
            query = query.Where(x => x.Date >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.Date <= to.Value);

        var data = await query.AsNoTracking().ToListAsync();

        return data
            .GroupBy(x => x.Institution)
            .Select(g => new
            {
                Institution = g.Key,
                Total = g.Sum(x => x.FullFare + x.Student + x.Teacher +
                                   x.SixtyYearsOld + x.Ticket + x.Child +
                                   x.Personnel + x.Free + x.BankCard)
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();
    }
}
