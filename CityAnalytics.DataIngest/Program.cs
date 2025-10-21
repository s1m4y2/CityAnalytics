using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CityAnalytics.Shared;
using CityAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// --- 1. Config dosyasını oku ---
var cfg = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var csvPath = cfg["Data:CsvPath"]!;
var delimiter = cfg["Data:Delimiter"] ?? ",";
var dateFormat = cfg["Data:DateFormat"] ?? "d.MM.yyyy";
var conn = cfg.GetConnectionString("Default")!;

// --- 2. DbContext hazırla ---
var dbOpts = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(conn)
    .Options;

using var db = new AppDbContext(dbOpts);
await db.Database.EnsureCreatedAsync();

// --- 3. CSV dosyasını oku ---
using var reader = new StreamReader(csvPath, Encoding.UTF8);
var csvCfg = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    HasHeaderRecord = true,
    Delimiter = delimiter,
    MissingFieldFound = null,
    TrimOptions = TrimOptions.Trim
};

using var csv = new CsvReader(reader, csvCfg);

// ✅ ÖNEMLİ: Başlık satırını oku
await csv.ReadAsync();
csv.ReadHeader();

// --- 4. Yardımcı fonksiyonlar ---
static int SafeInt(string? t)
{
    var val = (t ?? "0").Replace(".", "").Replace(",", "");
    return int.TryParse(val, out var v) ? v : 0;
}

static string FixTr(string s) => s
    .Replace("Ý", "İ").Replace("ý", "ı")
    .Replace("þ", "ş").Replace("Þ", "Ş")
    .Replace("ð", "ğ").Replace("Ð", "Ğ")
    .Replace("Ä°", "İ").Replace("ÅŸ", "ş").Replace("Åž", "Ş")
    .Replace("Ã¶", "ö").Replace("Ã¼", "ü").Replace("Ã‡", "Ç");

// --- 5. Veriyi oku ve dönüştür ---
var list = new List<DailyInstitutionUsage>();

while (await csv.ReadAsync())
{
    if (!DateTime.TryParseExact(csv.GetField("DATE"), dateFormat, CultureInfo.InvariantCulture,
        DateTimeStyles.None, out var date))
        continue;

    var inst = FixTr(csv.GetField("INSTITUTION") ?? "");

    list.Add(new DailyInstitutionUsage
    {
        Date = date,
        Institution = inst,
        FullFare = SafeInt(csv.GetField("FULL_FARE")),
        Student = SafeInt(csv.GetField("STUDENT")),
        Teacher = SafeInt(csv.GetField("TEACHER")),
        SixtyYearsOld = SafeInt(csv.GetField("SIXTY_YEARS_OLD")),
        Ticket = SafeInt(csv.GetField("TICKET")),
        Child = SafeInt(csv.GetField("CHILD")),
        Personnel = SafeInt(csv.GetField("PERSONNEL")),
        Free = SafeInt(csv.GetField("FREE")),
        BankCard = SafeInt(csv.GetField("BANK CARD"))
    });
}

// --- 6. Veritabanına kaydet ---
db.DailyInstitutionUsages.AddRange(list);
await db.SaveChangesAsync();

Console.WriteLine($"✅ Inserted {list.Count} rows into database.");
