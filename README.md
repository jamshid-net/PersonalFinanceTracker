# 💸 FiTrack – Shaxsiy Moliyaviy Hisobot Tizimi

**FiTrack** — bu .NET 9 asosida ishlab chiqilgan, foydalanuvchining daromad va xarajatlarini boshqarish, tahlil qilish, statistik hisobotlar olish, va audit yuritishni qo‘llab-quvvatlaydigan moliyaviy kuzatuv tizimidir.

---

## ?? Texnologiyalar

| Yo‘nalish          | Texnologiya                  |
| ------------------ | ----------------------------|
| Backend            | ASP.NET Core 9               |
| Ma’lumotlar bazasi | PostgreSQL 16                |
| Kesh (Cache)       | Redis (localhost default)    |
| Audit & Log        | Serilog + Telegram bot       |
| Arxitektura        | Clean Architecture           |
| Auth               | JWT Authentication           |
| API Hujjatlari     | Swagger (prod'da auth bilan) |
| Docker             | Docker Compose               |

---

## ?? Muhim sozlamalar va Eslatmalar

### ?? Redis Cache sozlovi

`Program.cs` yoki `DependencyInjection.cs` faylida quyidagicha yozilgan bo‘lishi kerak:

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost"; // IP:Port default
    options.InstanceName = "FiTrack:";
});
```

> Redis port va IP – `localhost:6379` by default. Redis server ishlayotganiga ishonch hosil qiling.

---

### ?? Migratsiya ishlari uchun maxsus sozlamalar

#### `Add-Migration` yoki `Update-Database` bajarayotganda:

Quyidagi qatorni `Program.cs` faylida vaqtincha **commentga oling**:

```csharp
// await app.InitialiseDatabaseAsync();
```

#### Shuningdek, quyidagi qator ham **commentga olinadi**:

`FiTrack.Infrastructure` ? `DependencyInjection.cs` faylida:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention()
           // .AddAsyncSeeding(sp); ? Bu qator commentga olinadi
});
```

> Aks holda migratsiya va bazaga ulanishda xatolik yuz beradi (`Seeding` migrationda ishlatilmaydi).

---

### ?? `appsettings.json` sozlamalari

#### ?? Telegram loglar uchun konfiguratsiya

```json
"TelegramConfigure": {
  "Token": "",     // Bot tokenini kiriting
  "ChatId": ""     // Admin Chat ID kiriting
}
```

> Loglar Telegram orqali yuboriladi. Ushbu ma'lumotlar to'ldirilishi kerak.

---

#### ?? Ma’lumotlar bazasiga ulanish

```json
"ConnectionStrings": {
  "FiTrack": "Server=localhost;Port=5432;Database=FiTrackDb;Username=postgres;Password=Jam568$;"
}
```

> PostgreSQL ulanish satri — sozlab qo‘ying.

---

## ? Umumiy imkoniyatlar

* [x] Foydalanuvchi daromad/xarajatlarini CRUD qilish
* [x] Har bir transactionga audit log yuritish
* [x] Kategoriya bo‘yicha sarf tahlili
* [x] Oylik balans: umumiy daromad/xarajat va farq
* [x] Soft delete (IsActive orqali yashirin o‘chirish)
* [x] Filtrlash, sortlash, pagination

---

## ?? Ishga tushirish

1. **PostgreSQL** va **Redis** ishga tushirilganiga ishonch hosil qiling.
2. `dotnet ef database update` bilan bazani yarating.
3. `dotnet run` orqali dasturga start bering.

---

