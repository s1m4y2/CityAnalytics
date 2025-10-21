using CityAnalytics.Analytics;
using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor + DbContext
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Servisler
builder.Services.AddScoped<InstitutionAnalyzer>();
builder.Services.AddScoped<Forecaster>();
builder.Services.AddScoped<InstitutionClusterer>();
builder.Services.AddScoped<AnomalyDetector>();
builder.Services.AddScoped<CorrelationAnalyzer>();

var app = builder.Build();

// ---- API uçları ----
app.MapGet("/api/clusters", async (InstitutionClusterer c, int? k) =>
{
    var clusters = await c.ClusterInstitutionsAsync(k ?? 3);
    return Results.Ok(clusters);
});

app.MapGet("/api/anomalies", async (AnomalyDetector detector, string institution) =>
{
    var result = await detector.DetectDailyAnomaliesAsync(institution);
    return Results.Ok(result);
});

app.MapGet("/api/correlation", async (CorrelationAnalyzer a, string? institution, DateTime? from, DateTime? to) =>
{
    var result = await a.GetCorrelationAsync(institution, from, to);
    return Results.Ok(result);
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Günlük toplam veriler
app.MapGet("/api/daily", async (InstitutionAnalyzer a, string? institution, DateTime? from, DateTime? to) =>
{
    var data = await a.GetDailyTotalsAsync(institution, from, to);
    return Results.Ok(data);
});

// Aylık toplam veriler
app.MapGet("/api/monthly", async (InstitutionAnalyzer a, string? institution, DateTime? from, DateTime? to) =>
{
    var data = await a.GetMonthlyTotalsAsync(institution, from, to);
    return Results.Ok(data);
});

// En çok kullanılan kurumlar
app.MapGet("/api/top", async (InstitutionAnalyzer a, DateTime? from, DateTime? to) =>
{
    var data = await a.GetTopInstitutionsAsync(from, to);
    return Results.Ok(data);
});

// Basit tahmin
app.MapGet("/api/forecast", async (Forecaster f, string institution, int? horizon) =>
{
    var data = await f.ForecastInstitutionAsync(institution, horizon ?? 7);
    return Results.Ok(data);
});

// Kurum listesi
app.MapGet("/api/institutions", async (AppDbContext db) =>
{
    var list = await db.DailyInstitutionUsages
        .Select(x => x.Institution)
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync();
    return Results.Ok(list);
});

app.Run();
