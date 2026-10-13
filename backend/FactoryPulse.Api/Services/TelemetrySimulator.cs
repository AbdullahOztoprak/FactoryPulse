using FactoryPulse.Api.Data;
using FactoryPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactoryPulse.Api.Services;

/// <summary>Clearly synthetic demo telemetry. No industrial equipment is connected.</summary>
public sealed class TelemetrySimulator(IServiceScopeFactory scopeFactory, ILogger<TelemetrySimulator> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        var random = new Random();

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FactoryDbContext>();
                var machines = await db.Machines.ToListAsync(stoppingToken);
                var now = DateTime.UtcNow;
                foreach (var machine in machines)
                {
                    // Small status changes keep the demo interactive without real devices.
                    if (random.NextDouble() < 0.012)
                        machine.Status = machine.Status == MachineStatus.Running ? MachineStatus.Idle : MachineStatus.Running;
                    machine.UpdatedAtUtc = now;
                    db.Readings.Add(new TelemetryReading
                    {
                        MachineId = machine.Id,
                        RecordedAtUtc = now,
                        TemperatureC = Math.Round(35 + random.NextDouble() * 20 + (machine.Status == MachineStatus.Fault ? 9 : 0), 1),
                        VibrationMmS = Math.Round(0.7 + random.NextDouble() * 2.9, 2),
                        UnitsProduced = machine.Status == MachineStatus.Running ? random.Next(1, 6) : 0
                    });
                }
                await db.SaveChangesAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Telemetry simulator stopped.");
        }
    }
}
