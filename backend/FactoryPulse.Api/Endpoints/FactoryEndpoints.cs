using FactoryPulse.Api.Data;
using FactoryPulse.Api.Models;
using FactoryPulse.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FactoryPulse.Api.Endpoints;

public static class FactoryEndpoints
{
    public static IEndpointRouteBuilder MapFactoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("FactoryPulse");

        group.MapGet("/overview", async (FactoryDbContext db, CancellationToken ct) =>
        {
            var machines = await db.Machines.AsNoTracking().ToListAsync(ct);
            var now = DateTime.UtcNow;
            var startUtc = now.Date;
            var unitsToday = await db.Readings.AsNoTracking()
                .Where(x => x.RecordedAtUtc >= startUtc)
                .SumAsync(x => (int?)x.UnitsProduced, ct) ?? 0;
            var openIncidents = await db.Incidents.CountAsync(x => x.Status != IncidentStatus.Resolved, ct);
            return Results.Ok(new
            {
                machineCount = machines.Count,
                runningCount = machines.Count(x => x.Status == MachineStatus.Running),
                idleCount = machines.Count(x => x.Status == MachineStatus.Idle),
                faultCount = machines.Count(x => x.Status == MachineStatus.Fault),
                availabilityPercent = machines.Count == 0 ? 0 : Math.Round(100.0 * machines.Count(x => x.Status == MachineStatus.Running) / machines.Count, 1),
                unitsToday,
                openIncidents,
                updatedAtUtc = now,
                dataSource = "SIMULATED"
            });
        });

        group.MapGet("/machines", async (FactoryDbContext db, CancellationToken ct) =>
        {
            var machines = await db.Machines.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
            var results = new List<object>();
            foreach (var machine in machines)
            {
                var reading = await db.Readings.AsNoTracking()
                    .Where(r => r.MachineId == machine.Id)
                    .OrderByDescending(r => r.RecordedAtUtc)
                    .Select(r => new { r.TemperatureC, r.VibrationMmS, r.RecordedAtUtc })
                    .FirstOrDefaultAsync(ct);
                results.Add(new
                {
                    machine.Id, machine.Code, machine.Name, machine.Line,
                    machine.Status, machine.UpdatedAtUtc,
                    latestTemperatureC = reading?.TemperatureC,
                    latestVibrationMmS = reading?.VibrationMmS,
                    lastReadingAtUtc = reading?.RecordedAtUtc
                });
            }
            return Results.Ok(results);
        });

        group.MapGet("/machines/{id:guid}", async (Guid id, FactoryDbContext db, CancellationToken ct) =>
        {
            var machine = await db.Machines.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Line,
                    x.Status,
                    x.UpdatedAtUtc,
                    latestReading = x.Readings
                        .OrderByDescending(r => r.RecordedAtUtc)
                        .Select(r => new
                        {
                            r.RecordedAtUtc,
                            r.TemperatureC,
                            r.VibrationMmS,
                            r.UnitsProduced
                        })
                        .FirstOrDefault(),
                    openIncidentCount = x.Incidents.Count(i => i.Status != IncidentStatus.Resolved)
                })
                .SingleOrDefaultAsync(ct);

            return machine is null ? Results.NotFound() : Results.Ok(machine);
        });

        group.MapGet("/machines/{id:guid}/readings", async (Guid id, int? limit, FactoryDbContext db, CancellationToken ct) =>
        {
            if (!await db.Machines.AnyAsync(x => x.Id == id, ct)) return Results.NotFound();
            var count = Math.Clamp(limit ?? 30, 1, 100);
            var rows = await db.Readings.AsNoTracking()
                .Where(x => x.MachineId == id)
                .OrderByDescending(x => x.RecordedAtUtc)
                .Take(count)
                .Select(x => new { x.RecordedAtUtc, x.TemperatureC, x.VibrationMmS, x.UnitsProduced })
                .ToListAsync(ct);
            rows.Reverse();
            return Results.Ok(rows);
        });

        group.MapGet("/incidents", async (FactoryDbContext db, CancellationToken ct) =>
        {
            var incidents = await db.Incidents.AsNoTracking()
                .Include(x => x.Machine)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(50)
                .Select(x => new
                {
                    x.Id, x.MachineId, machineCode = x.Machine.Code,
                    x.Title, x.Description, x.Severity, x.Status, x.CreatedAtUtc, x.ResolvedAtUtc
                })
                .ToListAsync(ct);
            return Results.Ok(incidents);
        });

        group.MapPost("/incidents", async (CreateIncidentRequest request, FactoryDbContext db, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length > 120 ||
                string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length > 1000 ||
                !Enum.IsDefined(request.Severity))
                return Results.BadRequest(new { error = "Valid title, description and severity are required." });
            if (!await db.Machines.AnyAsync(x => x.Id == request.MachineId, ct))
                return Results.BadRequest(new { error = "Unknown machine." });

            var incident = new Incident
            {
                MachineId = request.MachineId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Severity = request.Severity
            };
            db.Incidents.Add(incident);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/incidents/{incident.Id}", new { incident.Id, incident.Status });
        });

        group.MapPatch("/incidents/{id:guid}/status", async (Guid id, ChangeIncidentStatusRequest request, FactoryDbContext db, CancellationToken ct) =>
        {
            var incident = await db.Incidents.FindAsync([id], ct);
            if (incident is null) return Results.NotFound();
            if (!Enum.IsDefined(request.Status) || !IncidentWorkflow.CanTransition(incident.Status, request.Status))
                return Results.BadRequest(new { error = "Invalid incident status transition." });
            incident.Status = request.Status;
            if (request.Status == IncidentStatus.Resolved) incident.ResolvedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Results.Ok(new { incident.Id, incident.Status, incident.ResolvedAtUtc });
        });

        return app;
    }
}

public sealed record CreateIncidentRequest(Guid MachineId, string Title, string Description, IncidentSeverity Severity);
public sealed record ChangeIncidentStatusRequest(IncidentStatus Status);
