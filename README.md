# 🏙️ CityAnalytics – Akıllı Ulaşım Verileri Analiz Platformu

CityAnalytics, şehir içi toplu taşıma kurumlarının günlük kullanım verilerini analiz eden, öngörüde bulunan ve anomalileri tespit eden **.NET 8 + ML.NET** tabanlı bir veri analitiği platformudur.  
Bu proje; **veri madenciliği**, **makine öğrenimi** ve **şehir analitiği** konularını bir araya getirir.

---

## 🚀 Özellikler

| Modül | Açıklama | Teknoloji |
|-------|-----------|------------|
| 📊 **InstitutionAnalyzer** | Günlük ve aylık toplam biletleme istatistiklerini çıkarır | Entity Framework Core |
| 🧮 **CorrelationAnalyzer** | Bilet tipleri arasındaki korelasyon matrisini hesaplar | Pearson Correlation |
| 🤖 **InstitutionClusterer** | Kurumları benzer bilet kullanımlarına göre gruplar | ML.NET K-Means |
| 🔮 **Forecaster** | Gelecekteki yolcu sayısını tahmin eder | ML.NET SSA Forecasting |
| ⚠️ **AnomalyDetector** | Z-score tabanlı anomali (uç değer) tespiti yapar | İstatistiksel Analiz |

---

## 🧱 Mimari Yapı

```
CityAnalytics/
├── CityAnalytics.Analytics/        # Analiz ve ML modelleri
│   ├── InstitutionAnalyzer.cs
│   ├── CorrelationAnalyzer.cs
│   ├── InstitutionClusterer.cs
│   ├── Forecaster.cs
│   └── AnomalyDetector.cs
├── CityAnalytics.Storage/          # EF Core DbContext + Entities
│   ├── AppDbContext.cs
│   ├── DesignTimeFactory.cs
│   └── DailyInstitutionUsage.cs
├── Program.cs                      # CSV → SQLite yükleme girişi
├── appsettings.example.json        # Örnek ayar dosyası (gizli bilgiler yok)
├── .gitignore                      # Gizli ve build dosyalarını dışlar
└── README.md                       # Bu dosya 😄
```

---

## ⚙️ Kurulum ve Çalıştırma

### 1️⃣ Depoyu klonla
```bash
git clone https://github.com/<kullanici-adin>/CityAnalytics.git
cd CityAnalytics
```

### 2️⃣ Ayar dosyasını oluştur
Örnek dosyadan kopya çıkar:
```bash
copy appsettings.example.json appsettings.json
```

### 3️⃣ `appsettings.json`’u düzenle
```json
{
  "Data": {
    "CsvPath": "C:\\Users\\Simay\\Desktop\\data\\izmirdata.csv",
    "Delimiter": ",",
    "DateFormat": "d.MM.yyyy"
  },
  "ConnectionStrings": {
    "Default": "Data Source=cityanalytics.db"
  }
}
```
> ⚠️ `appsettings.json`, `.db` ve `.csv` dosyaları `.gitignore` ile korunur ve GitHub’a gönderilmez.

### 4️⃣ Çalıştır
```bash
dotnet run
```

### 5️⃣ Beklenen çıktı
```bash
✅ Inserted 1250 rows into database.
```

---

## 🧩 Örnek Kullanımlar

### 📈 7 Günlük Tahmin
```csharp
var fc = new Forecaster(db);
var results = await fc.ForecastInstitutionAsync("ESHOT", 7);
```

### 🧮 Kurum Kümeleme (K-Means)
```csharp
var cl = new InstitutionClusterer(db);
var clusters = await cl.ClusterInstitutionsAsync(4);
```

### ⚠️ Günlük Anomali Tespiti
```csharp
var ad = new AnomalyDetector(db);
var anomalies = await ad.DetectDailyAnomaliesAsync("Metro A.Ş.");
```

---

## 🧠 Kullanılan Teknolojiler

| Teknoloji | Kullanım Alanı |
|------------|----------------|
| 🟦 **.NET 8** | Ana uygulama çatısı |
| 🧠 **ML.NET** | Kümeleme, tahminleme |
| 🗃️ **Entity Framework Core (SQLite)** | Veritabanı erişimi |
| 📂 **CsvHelper** | CSV veri okuma/parsing |
| ⚙️ **Dependency Injection** | Servis yönetimi |
| 📈 **Z-Score, Pearson Correlation** | İstatistiksel analiz yöntemleri |

---

## 🧑‍💻 Geliştirici Hakkında

**👩‍💻 Simay Ayanoğlu**  
- Yazılım Mühendisliği, Manisa Celal Bayar Üniversitesi  
- Alanlar: Backend, Veri Madenciliği, Dağıtık Sistemler  
- Proje: *CityAnalytics – Akıllı Şehir Ulaşım Analitiği*  
- 🌐 [LinkedIn](www.linkedin.com/in/simay-ayanoğlu-0b02a8255)  
- 📧 simaynglu@gmail.com  

---

## 🛡️ Güvenlik Notu
Bu proje yalnızca **örnek veri** ile geliştirilmiştir.  
Gerçek kişisel veya kurumsal veri içermez.  
`appsettings.json` dosyası gizli bilgileri (veri yolu, bağlantı dizesi vb.) barındırabilir — **bu dosya GitHub’a yüklenmemelidir.**

---

## 🧾 Lisans
**MIT License**  
Bu proje açık kaynaklıdır ve araştırma / eğitim amaçlı kullanılabilir.

---

## 🌟 Ekran Görseli 
```
![forecast-example](https://github.com/<kullanici-adin>/CityAnalytics/assets/.../forecast.png)
```
