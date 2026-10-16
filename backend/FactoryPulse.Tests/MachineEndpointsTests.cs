using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FactoryPulse.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace FactoryPulse.Tests;

public sealed class MachineEndpointsTests(FactoryPulseApiFactory factory)
    : IClassFixture<FactoryPulseApiFactory>
{
    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Existing_machine_returns_ok()
    {
        var machines = await client.GetFromJsonAsync<List<MachineSummary>>("/api/machines");
        var machineId = Assert.Single(machines!, x => x.Code == "CNC-01").Id;

        var response = await client.GetAsync($"/api/machines/{machineId}");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(machineId, body.GetProperty("id").GetGuid());
        Assert.Equal("CNC-01", body.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Missing_machine_returns_not_found()
    {
        var response = await client.GetAsync($"/api/machines/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record MachineSummary(Guid Id, string Code);
}

public sealed class FactoryPulseApiFactory : WebApplicationFactory<Program>
{
    private readonly string databasePath = Path.Combine(
        Path.GetTempPath(), $"factorypulse-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:FactoryPulse", $"Data Source={databasePath}");
        builder.ConfigureServices(services =>
        {
            var simulator = services.SingleOrDefault(service =>
                service.ServiceType == typeof(IHostedService) &&
                service.ImplementationType == typeof(TelemetrySimulator));

            if (simulator is not null)
                services.Remove(simulator);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (File.Exists(databasePath))
            File.Delete(databasePath);
    }
}
