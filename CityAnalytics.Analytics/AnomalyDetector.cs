using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityAnalytics.Analytics
{
    public class AnomalyDetector
    {
        private readonly AppDbContext _db;
        public AnomalyDetector(AppDbContext db) => _db = db;

        // Z-score tabanlı basit anomaly detection
        public async Task<IEnumerable<object>> DetectDailyAnomaliesAsync(string institution)
        {
            var data = await _db.DailyInstitutionUsages
                .Where(x => x.Institution.Contains(institution))
                .OrderBy(x => x.Date)
                .Select(x => new
                {
                    x.Date,
                    Total = x.FullFare + x.Student + x.Teacher +
                            x.SixtyYearsOld + x.Ticket + x.Child +
                            x.Personnel + x.Free + x.BankCard
                })
                .ToListAsync();

            if (!data.Any()) return [];

            var totals = data.Select(d => d.Total).ToList();
            double mean = totals.Average();
            double std = Math.Sqrt(totals.Sum(x => Math.Pow(x - mean, 2)) / totals.Count);

            var anomalies = data
                .Select(d => new
                {
                    d.Date,
                    d.Total,
                    ZScore = std == 0 ? 0 : (d.Total - mean) / std,
                    IsAnomaly = Math.Abs(std == 0 ? 0 : (d.Total - mean) / std) > 2.0
                })
                .ToList();

            return anomalies;
        }
    }
}
