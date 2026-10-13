# FactoryPulse — Öğrenerek Geliştirme Planı

Amaç: Projeyi yalnızca çalıştırmak değil, **her sınıfın, endpoint'in ve veritabanı ilişkisinin neden var olduğunu anlayarak** geliştirmek. CV'de ancak kendi test edip açıklayabildiğin işi yaz.

## Başlangıç: doğrula ve kendi çalıştır

1. .NET 10 SDK, Node.js (Angular 21 ile uyumlu), Git ve isteğe bağlı Docker kur.
2. README'deki backend/Angular komutlarını uygula. Başlangıç paketi çevrimdışı hazırlandı; ilk kurulumda sürüm sorunları çıkarsa çöz ve commit et.
3. `/health`, `/api/machines`, `/api/overview` yanıtlarını Postman veya `curl` ile kontrol et.
4. Angular ekranda sıcaklık grafiğini ve makine listesini aç. Bir incident oluştur, acknowledge et, resolve et.
5. `npm install` sonrasında üretilen `package-lock.json` dosyasını commit et. CI ve Dockerfile'da `npm install` yerine `npm ci` kullan.
6. Çalışan sistemin ekran görüntüsünü `docs/screenshots/` içine ekle. Başarılı test/CI olmadan rozet kullanma.

## Modül 1 — C# ve OOP

**Oku:** `Models/FactoryEntities.cs`, `Services/IncidentWorkflow.cs`.

**Öğren:** class, property, enum, generic List, switch expression, encapsulation, interface vs class, nullability.

**Kendin yap:** `Machine` modeline `Location` alanı ekle; seed verisini ve API çıktısını güncelle. Test yaz.

**Mülakat:** "A class represents a machine; its readings and incidents are related records. I used enums to constrain statuses."

## Modül 2 — REST API

**Oku:** `Endpoints/FactoryEndpoints.cs`, `Program.cs`.

**Öğren:** GET/POST/PATCH, JSON, 200/201/400/404, DTO, async/await, dependency injection, request validation.

**Kendin yap:** `GET /api/machines/{id}` ekle. Olmayan id için 404 dön. Yanıtı Postman ile kanıtla.

**Mülakat:** "I built an ASP.NET Core REST API with typed request models, validation and status codes."

## Modül 3 — SQL ve EF Core

**Oku:** `Data/FactoryDbContext.cs`, `DemoDataSeeder.cs`.

**Öğren:** primary key, foreign key, 1:N relationships, index, LINQ, AsNoTracking, database migrations, SQLite.

**Kendin yap:** `GET /api/machines?line=Assembly A` filtrelemesi ekle ve iki veriyle test et.

**Mülakat:** "I modeled machines, incident records and time-series readings with EF Core relationships."

## Modül 4 — Angular / TypeScript

**Oku:** `frontend/src/app/models.ts`, `api.service.ts`, `app.ts`, `app.html`.

**Öğren:** component, dependency injection, signals, service, HttpClient, promises/observables, template binding, forms.

**Kendin yap:** Makine adına göre filtreleme kutusu ekle. Angular'da tip hatası almadan build et.

**Mülakat:** "I used Angular with a typed HttpClient service to consume the REST API and render telemetry."

## Modül 5 — Docker ve CI/CD

**Oku:** `compose.yaml`, Dockerfile'lar, `.github/workflows/ci.yml`.

**Öğren:** image/container, build stages, volume, reverse proxy, restore/build/test pipeline, pull requests.

**Kendin yap:** CI job'larını yeşil yap, ekran görüntüsü al ve ilk pull request'ini aç.

**Mülakat:** "I configured separate container images and a CI workflow with build and test checks."

## Modül 6 — Mühendislik kalitesi

**Yap:** auth + role-based authorization; pagination; telemetry retention; structured logging; e2e/integration tests; database migrations; API error handling.

**Mülakat:** "I can explain what the demo does, its limitations and how I would harden it for production."

## Günlük çalışma rutini (60–90 dakika)

20 dk kavramı anla → 40 dk kendin kod yaz → 15 dk test et → 10 dk README'ye öğrendiğini yaz ve commit et. İşlevi anlatamıyorsan henüz öğrenilmiş sayma.

## Önerilen commit geçmişi

- `chore: bootstrap monorepo and docs`
- `feat(api): add machine lookup endpoint`
- `feat(web): filter machines by name`
- `test(api): cover incident state transitions`
- `ci: verify frontend and backend builds`

Commits yaptığın gerçek değişiklikleri yansıtmalı. Hazır kodu tek seferde kendi yazmışsın gibi anlatmak yerine, üzerinde yaptığın geliştirmeleri ve öğrendiklerini göstermek daha inandırıcıdır.
