using FactoryPulse.Api.Data;
using FactoryPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactoryPulse.Api.Services;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(FactoryDbContext db)
    {
        if (await db.Machines.AnyAsync()) return;

        var specs = new (string Code, string Name, string Line, MachineStatus Status)[]
        {
            ("CNC-01", "CNC Milling 01", "Assembly A", MachineStatus.Running),
            ("CNC-02", "CNC Milling 02", "Assembly A", MachineStatus.Running),
            ("ROB-03", "Robot Cell 03", "Assembly A", MachineStatus.Idle),
            ("PRS-04", "Press Unit 04", "Assembly B", MachineStatus.Running),
            ("WLD-05", "Welding Station 05", "Assembly B", MachineStatus.Running),
            ("CNV-06", "Conveyor 06", "Assembly B", MachineStatus.Fault),
            ("INS-07", "Inspection Unit 07", "Quality", MachineStatus.Running),
            ("PKG-08", "Packaging Line 08", "Packaging", MachineStatus.Idle)
        };
        var machines = specs.Select(s => new Machine
        {
            Code = s.Code, Name = s.Name, Line = s.Line, Status = s.Status
        }).ToList();
        db.Machines.AddRange(machines);

        var rng = new Random(2026);
        var now = DateTime.UtcNow;
        foreach (var machine in machines)
        {
            for (var step = 48; step >= 0; step--)
            {
                var time = now.AddMinutes(-15 * step);
                db.Readings.Add(new TelemetryReading
                {
                    MachineId = machine.Id,
                    RecordedAtUtc = time,
                    TemperatureC = Math.Round(37 + rng.NextDouble() * 16 + (machine.Status == MachineStatus.Fault ? 9 : 0), 1),
                    VibrationMmS = Math.Round(0.7 + rng.NextDouble() * 2.9, 2),
                    UnitsProduced = machine.Status == MachineStatus.Running ? rng.Next(5, 20) : 0
                });
            }
        }
        db.Incidents.AddRange(
            new Incident { MachineId = machines[5].Id, Title = "Conveyor vibration exceeded threshold", Description = "Simulated alert from conveyor sensor.", Severity = IncidentSeverity.High, CreatedAtUtc = now.AddMinutes(-25) },
            new Incident { MachineId = machines[2].Id, Title = "Inspection required", Description = "Scheduled operator check before the next run.", Severity = IncidentSeverity.Medium, CreatedAtUtc = now.AddHours(-2) }
        );
        await db.SaveChangesAsync();
    }
}
