using System.Text.Json.Serialization;
using FactoryPulse.Api.Data;
using FactoryPulse.Api.Endpoints;
using FactoryPulse.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var connectionString = builder.Configuration.GetConnectionString("FactoryPulse")
    ?? "Data Source=factorypulse.db";
builder.Services.AddDbContext<FactoryDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddOpenApi();
builder.Services.AddHostedService<TelemetrySimulator>();

var app = builder.Build();

// Explicitly marked as a demo. For production use reviewed EF Core migrations.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FactoryDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DemoDataSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", mode = "simulated" }));
app.MapFactoryEndpoints();

app.Run();

public partial class Program { }
