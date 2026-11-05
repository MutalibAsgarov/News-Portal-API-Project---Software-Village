# News Portal API

## Layihə haqqında

News Portal API — Software Village proqramı çərçivəsində hazırlanmış xəbər portalının backend layihəsidir.

Layihənin əsas məqsədi frontend tətbiqinin istifadə edə biləcəyi RESTful API vasitəsilə xəbər məlumatlarını təqdim etməkdir.

API vasitəsilə xəbərlərin məzmunu, kateqoriyaları, müəllif məlumatları, statistik göstəriciləri və əlaqəli xəbərlər əldə edilə bilər.

Layihədə ana səhifə və xəbər detalları üçün ayrı-ayrı endpoint-lər hazırlanmışdır.

## İstifadə olunan texnologiyalar

- C#
- ASP.NET Core Web API
- Entity Framework Core
- Microsoft SQL Server
- LINQ
- RESTful API
- Swagger / OpenAPI

## Layihənin strukturu

```text
News_Portal_API/
│
├── Controllers/
│   └── NewsController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── DTOs/
│   ├── News/
│   └── ...
│
├── Entities/
│   └── ...
│
├── Database/
│   └── NewsPortalDb.sql
│
├── Properties/
│   └── launchSettings.json
│
├── Program.cs
├── appsettings.json
└── News_Portal_API.csproj
```

## Verilənlər bazası

Layihədə verilənlərin saxlanılması və idarə olunması üçün Microsoft SQL Server istifadə olunur.

Verilənlər bazasının əsas cədvəlləri:

| Cədvəl | Təyinatı |
|---|---|
| articles | Xəbərlərin məlumatlarını və məzmununu saxlayır |
| categories | Xəbər kateqoriyalarını saxlayır |
| authors | Müəllif məlumatlarını saxlayır |
| article_stats | Xəbərlərin statistik göstəricilərini saxlayır |

### Cədvəllər arasındakı əlaqələr

- Hər xəbər müəyyən bir kateqoriyaya aiddir.
- Hər xəbər bir müəlliflə əlaqələndirilir.
- Hər xəbərin statistik məlumatları mövcuddur.

Entity Framework Core verilənlər bazası ilə əlaqəni və məlumatların idarə olunmasını təmin edir.

## API Endpoint-ləri

**Base URL:**

```text
https://localhost:7025
```

Bütün endpoint-lər `/api/news` marşrutu altında yerləşir.

### 1. Ana səhifə endpoint-ləri

| HTTP metodu | Endpoint | Təyinatı |
|---|---|---|
| GET | `/api/news/featured` | Seçilmiş əsas xəbəri qaytarır |
| GET | `/api/news/breaking` | Son dəqiqə xəbərlərini qaytarır |
| GET | `/api/news/articles` | Xəbər siyahısını və kateqoriyalara görə filtrləməni təmin edir |
| GET | `/api/news/live` | Canlı xəbəri qaytarır |
| GET | `/api/news/editors-picks` | Redaktor tərəfindən seçilmiş xəbərləri qaytarır |

**Articles endpoint-inin Query parametrləri:**

| Parametr | Standart dəyər | Təyinatı |
|---|---|---|
| tab | latest | Xəbər bölməsi: latest, think, health |
| page | 1 | Cari səhifə |
| limit | 4 | Bir səhifədəki xəbər sayı |

**Sorğu nümunəsi:**

```http
GET /api/news/articles?tab=latest&page=1&limit=4
```

### 2. Xəbər detalları endpoint-ləri

| HTTP metodu | Endpoint | Təyinatı |
|---|---|---|
| GET | `/api/news/{id}` | Xəbərin tam məlumatlarını qaytarır |
| GET | `/api/news/{id}/related` | Eyni kateqoriyaya aid əlaqəli xəbərləri qaytarır |

Xəbər detalı endpoint-i aşağıdakı məlumatları təqdim edir:

- Xəbərin tam mətni
- Xəbərin əsas məlumatları və dərc olunma tarixi
- Kateqoriya məlumatları
- Müəllif məlumatları
- Bəyənmə, paylaşım, yadda saxlama və şərh statistikası

**Related News query parametri:**

| Parametr | Standart dəyər | Təyinatı |
|---|---|---|
| limit | 5 | Qaytarılacaq əlaqəli xəbərlərin maksimum sayı |

**Sorğu nümunələri:**

```http
GET /api/news/1
```

```http
GET /api/news/1/related?limit=5
```

## API cavab formatı

API sorğularında standart cavab strukturu istifadə olunur.

**Uğurlu sorğu nümunəsi:**

```json
{
  "success": true,
  "message": "OK",
  "data": {}
}
```

**Xəta cavabı nümunəsi:**

```json
{
  "success": false,
  "message": "News not found.",
  "data": null
}
```

**Pagination nümunəsi:**

Səhifələmə tətbiq olunan endpoint-lərdə əlavə pagination məlumatları təqdim olunur.

```json
{
  "success": true,
  "message": "OK",
  "data": [],
  "pagination": {
    "currentPage": 1,
    "totalPages": 5,
    "totalItems": 20,
    "limit": 4
  }
}
```

## Xəta idarəetməsi

API sorğular zamanı yarana biləcək xətaları uyğun HTTP status kodları ilə idarə edir.

| Status kodu | İzahı |
|---|---|
| 200 | Sorğu uğurla tamamlandı |
| 400 | Yanlış sorğu parametrləri |
| 404 | Axtarılan məlumat tapılmadı |
| 500 | Server daxilində xəta baş verdi |

## Swagger sənədləşməsi

API endpoint-lərinin sənədləşdirilməsi və test edilməsi üçün Swagger / OpenAPI istifadə olunur.

Layihəni işə saldıqdan sonra Swagger interfeysinə aşağıdakı ünvandan daxil olmaq mümkündür:

```text
https://localhost:7025/swagger
```

Swagger vasitəsilə mövcud endpoint-ləri, sorğu parametrlərini və cavab strukturlarını araşdırmaq və test etmək mümkündür.

## Layihənin işə salınması

### Tələblər

- .NET SDK
- Microsoft SQL Server
- Visual Studio və ya uyğun IDE

### Quraşdırılma

**1. Repository-ni klonlayın**

```bash
git clone https://github.com/MutalibAsgarov/News-Portal-API-Project---Software-Village.git
```

**2. Layihə qovluğuna daxil olun**

```bash
cd News-Portal-API-Project---Software-Village
```

**3. Lazımi paketləri bərpa edin**

```bash
dotnet restore
```

**4. Verilənlər bazası bağlantısını konfiqurasiya edin**

`appsettings.json` faylında yerləşən `DefaultConnection` connection string parametrini öz SQL Server konfiqurasiyanıza uyğunlaşdırın.

**5. Verilənlər bazasını yaradın**

`Database` qovluğunda yerləşən SQL skriptini SQL Server üzərində icra edərək verilənlər bazasını yaradın və ilkin məlumatları daxil edin.

**6. Layihəni işə salın**

```bash
dotnet run
```

**7. Swagger interfeysini açın**

```text
https://localhost:7025/swagger
```

## Müəllif

**Mütalib Əsgərov**

[GitHub](https://github.com/MutalibAsgarov)
