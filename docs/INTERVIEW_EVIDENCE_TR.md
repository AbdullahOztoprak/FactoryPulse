# Teknik Mülakat — Soru / Kod Kanıtı

Buradaki dosyalar proje kaynaklarında vardır; fakat **çalıştırılmadan veya sen tarafından öğrenilmeden deneyim iddiası oluşturmaz**. Her maddeyi kendi bilgisayarında doğrula.

| Görüşmede soru | Öğrenilecek ve açıklanacak alan | Dosyadaki kanıt |
| --- | --- | --- |
| C# biliyor musun? | Class, enum, LINQ, async/await | `backend/FactoryPulse.Api/Models/FactoryEntities.cs`, `Endpoints/FactoryEndpoints.cs` |
| .NET ile API geliştirdin mi? | Minimal API, routing, dependency injection | `backend/FactoryPulse.Api/Program.cs`, `Endpoints/FactoryEndpoints.cs` |
| REST / JSON nasıl çalışır? | GET/POST/PATCH, status codes | `docs/API.md` ve `api.service.ts` |
| SQL kullandın mı? | İlişkisel şema, indeksler, EF Core | `Data/FactoryDbContext.cs` |
| Angular biliyor musun? | Standalone component, signals, HttpClient, template | `frontend/src/app/` |
| Background worker yazdın mı? | Hosted service ve timer | `Services/TelemetrySimulator.cs` |
| Docker kullandın mı? | İki aşamalı build, compose volume ve proxy | `compose.yaml`, iki `Dockerfile` |
| CI/CD kurdun mu? | Build ve test workflow | `.github/workflows/ci.yml` |
| Unit test yazdın mı? | Arrange/act/assert ve Theory | `backend/FactoryPulse.Tests/IncidentWorkflowTests.cs` |
| Agile ve gereksinim analizi? | Backlog, user story ve acceptance criteria | `docs/ROADMAP.md` |

## Dürüst bir İngilizce anlatım taslağı — ancak gerçekten doğruladıktan sonra

"I built an industrial monitoring demo called FactoryPulse to practice the full-stack technologies used in enterprise digitalization. I developed an ASP.NET Core API and an Angular dashboard, persisted machine and incident data with EF Core and SQLite, and modeled incident status transitions with unit tests. The telemetry is simulated, so it is a learning project rather than a real industrial integration. I also documented the architecture and set up a CI workflow. The project helped me understand how UI, API, database, and deployment components work together."

## Mülakatçı derinleştirirse

- `Why SQLite?` → Local, frictionless demo; enterprise SQL Server/PostgreSQL possible later.
- `Why polling instead of WebSockets?` → Small demo, simple implementation; real-time push optional later.
- `How did you validate API input?` → Manual basic checks; production: stronger validation, auth and observability.
- `How are machine signals collected?` → Synthetic `BackgroundService`, **not** OPC UA, PLC, or real factory sensors.
- `How do you store secrets?` → Demo has none; production: env vars/secret manager, never commit passwords.
- `How do you prevent unauthorized updates?` → Not implemented; must add authorization before public deployment.
- `How do you measure availability?` → Running share, *not* proper availability/OEE; production requires time-weighted modeling.
