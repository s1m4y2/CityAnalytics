using CityAnalytics.Shared;
using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

namespace CityAnalytics.Analytics;

public class Forecaster
{
    private readonly AppDbContext _db;
    private readonly MLContext _ml = new(0);

    public Forecaster(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ForecastResult>> ForecastInstitutionAsync(
    string institution, int horizonDays = 7)
    {
        try
        {
            var series = await _db.DailyInstitutionUsages.AsNoTracking()
                .Where(x => x.Institution.ToLower().Contains(institution.ToLower()))
                .OrderBy(x => x.Date)
                .Select(x => new TimePoint
                {
                    Value = (float)(x.FullFare + x.Student + x.Teacher + x.SixtyYearsOld +
                                    x.Ticket + x.Child + x.Personnel + x.Free + x.BankCard),
                    Timestamp = x.Date
                })
                .ToListAsync();

            if (series.Count < 30)
                return Array.Empty<ForecastResult>();

            var data = _ml.Data.LoadFromEnumerable(series);

            var pipeline = _ml.Forecasting.ForecastBySsa(
                outputColumnName: nameof(ForecastPoint.ForecastedValues),
                inputColumnName: nameof(TimePoint.Value),
                windowSize: 7,
                seriesLength: series.Count,
                trainSize: series.Count - 7,
                horizon: horizonDays);

            var model = pipeline.Fit(data);
            var engine = model.CreateTimeSeriesEngine<TimePoint, ForecastPoint>(_ml);

            var result = engine.Predict();

            var startDate = series.Last().Timestamp.AddDays(1);
            var list = new List<ForecastResult>();
            for (int i = 0; i < horizonDays; i++)
            {
                list.Add(new ForecastResult
                {
                    Date = startDate.AddDays(i),
                    Forecast = result.ForecastedValues[i]
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Forecast error: {ex.Message}");
            return Array.Empty<ForecastResult>();
        }
    }

    public class ForecastResult
    {
        public DateTime Date { get; set; }
        public float Forecast { get; set; }
    }

    public class TimePoint
    {
        public DateTime Timestamp { get; set; }
        public float Value { get; set; }
    }

    public class ForecastPoint
    {
        [VectorType]
        public float[] ForecastedValues { get; set; } = default!;
    }

}
