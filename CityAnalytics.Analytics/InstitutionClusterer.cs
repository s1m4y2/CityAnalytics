using CityAnalytics.Shared;
using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Linq;

namespace CityAnalytics.Analytics;

public class InstitutionClusterer
{
    private readonly AppDbContext _db;
    private readonly MLContext _ml;

    public InstitutionClusterer(AppDbContext db)
    {
        _db = db;
        _ml = new MLContext(seed: 1);
    }

    // input feature model
    public class InstitutionStats
    {
        public string Institution { get; set; } = "";
        public float FullFare { get; set; }
        public float Student { get; set; }
        public float Teacher { get; set; }
        public float SixtyYearsOld { get; set; }
        public float Ticket { get; set; }
        public float Child { get; set; }
        public float Personnel { get; set; }
        public float Free { get; set; }
        public float BankCard { get; set; }
    }

    public class InstitutionClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint ClusterId { get; set; }

        [ColumnName("Score")]
        public float[]? Distances { get; set; }
    }

    public async Task<IEnumerable<object>> ClusterInstitutionsAsync(int k = 3)
    {
        var stats = _db.DailyInstitutionUsages
            .AsEnumerable()
            .GroupBy(x => x.Institution)
            .Select(g => new InstitutionStats
            {
                Institution = g.Key,
                FullFare = (float)g.Average(x => x.FullFare),
                Student = (float)g.Average(x => x.Student),
                Teacher = (float)g.Average(x => x.Teacher),
                SixtyYearsOld = (float)g.Average(x => x.SixtyYearsOld),
                Ticket = (float)g.Average(x => x.Ticket),
                Child = (float)g.Average(x => x.Child),
                Personnel = (float)g.Average(x => x.Personnel),
                Free = (float)g.Average(x => x.Free),
                BankCard = (float)g.Average(x => x.BankCard)
            })
            .ToList();

        var data = _ml.Data.LoadFromEnumerable(stats);
        var pipeline = _ml.Transforms.Concatenate("Features",
            nameof(InstitutionStats.FullFare),
            nameof(InstitutionStats.Student),
            nameof(InstitutionStats.Teacher),
            nameof(InstitutionStats.SixtyYearsOld),
            nameof(InstitutionStats.Ticket),
            nameof(InstitutionStats.Child),
            nameof(InstitutionStats.Personnel),
            nameof(InstitutionStats.Free),
            nameof(InstitutionStats.BankCard))
            .Append(_ml.Clustering.Trainers.KMeans("Features", numberOfClusters: k));

        var model = pipeline.Fit(data);
        var predEngine = _ml.Model.CreatePredictionEngine<InstitutionStats, InstitutionClusterPrediction>(model);

        var results = stats.Select(s =>
        {
            var p = predEngine.Predict(s);
            return new
            {
                s.Institution,
                Cluster = p.ClusterId,
                // 👇 3D scatter için kullanacağız
                s.FullFare,
                s.Student,
                s.Free,
                s.BankCard
            };
        })
        .OrderBy(r => r.Cluster)
        .ToList();

        return results;
    }

}
