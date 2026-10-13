# FactoryPulse — Başlangıç Rehberi

Bu proje **v0.1.0 başlangıç kod tabanıdır**. Kodlar henüz gerçek .NET/Angular derlemesiyle doğrulanmadı. İlk aşamada yerel kurulumda doğrulayacağız.

## 1. Sistem kontrolü (Mac Terminal)

```bash
dotnet --version
node --version
npm --version
git --version
docker --version
```

.NET 10 SDK, Angular 21 ile uyumlu Node ve Git gerekir; Docker ilk aşamada isteğe bağlı.

## 2. .NET API'yi başlat

Proje klasöründe:

```bash
dotnet restore backend/FactoryPulse.slnx
dotnet test backend/FactoryPulse.Tests/FactoryPulse.Tests.csproj
dotnet run --project backend/FactoryPulse.Api --launch-profile http
```

Başka Terminal sekmesinde:

```bash
curl http://localhost:5080/health
curl http://localhost:5080/api/machines
curl http://localhost:5080/api/overview
```

## 3. Angular'ı başlat

```bash
cd frontend
npm install
npm run typecheck
npm start
```

Tarayıcı: http://localhost:4200 — Makine seç, ölçümleri incele, incident oluştur ve çöz.

## 4. GitHub'a yükle

Önce .NET ve Angular'ın çalıştığını doğrula. Proje kökünde:

```bash
git init -b main
git add .
git commit -m "feat: initialize FactoryPulse demo"
```

GitHub'da `factorypulse` isimli boş public repository oluştur (yeni README oluşturma). Ardından kendi kullanıcı adınla:

```bash
git remote add origin https://github.com/KULLANICI_ADIN/factorypulse.git
git push -u origin main
```

**Önemli:** `npm install` sonrasında oluşan `package-lock.json` dosyasını Git'e ekle, CI ve frontend Dockerfile'da `npm install` yerine `npm ci` kullan. CI yeşil olmadan başarı iddiası ekleme.

## 5. Birlikte öğreneceğimiz ilk özellik

`GET /api/machines/{id}` endpoint'ini sen geliştireceksin. Geçerli ID için 200 ve olmayan ID için 404 dönüşlerini test edeceğiz. Böylece C#, async/await, EF Core, REST ve Git konularını gerçekten kavrayacaksın.

Diğer dersler: `docs/LEARNING_GUIDE_TR.md`.
