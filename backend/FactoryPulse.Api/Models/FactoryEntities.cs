namespace FactoryPulse.Api.Models;

public enum MachineStatus { Running, Idle, Fault }
public enum IncidentSeverity { Low, Medium, High }
public enum IncidentStatus { Open, Acknowledged, Resolved }

public sealed class Machine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Line { get; set; } = "";
    public MachineStatus Status { get; set; } = MachineStatus.Running;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<TelemetryReading> Readings { get; set; } = [];
    public List<Incident> Incidents { get; set; } = [];
}

public sealed class TelemetryReading
{
    public long Id { get; set; }
    public Guid MachineId { get; set; }
    public Machine Machine { get; set; } = null!;
    public DateTime RecordedAtUtc { get; set; }
    public double TemperatureC { get; set; }
    public double VibrationMmS { get; set; }
    // Units produced during this sample interval (not a lifetime counter).
    public int UnitsProduced { get; set; }
}

public sealed class Incident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MachineId { get; set; }
    public Machine Machine { get; set; } = null!;
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
}
