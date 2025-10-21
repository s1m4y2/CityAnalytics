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
- 🌐 [LinkedIn](https://www.linkedin.com/in/simay-ayanoğlu-0b02a8255)  
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

## 🖼️ CityAnalytics Dashboard Görselleri

### 1️⃣ Genel Görünüm (Metro)
<img width="1187" height="785" alt="Dashboard 1" src="https://github.com/user-attachments/assets/2b4f1003-4038-4501-82f0-249ee981914b" />

### 2️⃣ Günlük & Aylık Kullanım
<img width="1151" height="867" alt="Dashboard 2" src="https://github.com/user-attachments/assets/8f7e2851-a63a-4075-b1bc-d075c5042485" />

### 3️⃣ Tahmin, En Yoğun Kurum, Kümeleme 
<img width="1213" height="922" alt="Dashboard 3" src="https://github.com/user-attachments/assets/82bc04cc-86e2-4c7a-898f-dea20431dfb0" />

### 4️⃣ Anomaliler ve Korelasyon Isı Haritası
<img width="1166" height="922" alt="Dashboard 4" src="https://github.com/user-attachments/assets/6dd14ee0-5515-40bb-b1c9-78be3bb55df4" />

> Tüm görseller canlı analizlerden elde edilmiştir.  
> Veriler SQLite üzerinden okunur, ML.NET ile işlenir ve web arayüzünde dinamik olarak gösterilir.

