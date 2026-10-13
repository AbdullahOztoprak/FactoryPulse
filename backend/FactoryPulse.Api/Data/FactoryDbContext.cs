using FactoryPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactoryPulse.Api.Data;

public sealed class FactoryDbContext(DbContextOptions<FactoryDbContext> options) : DbContext(options)
{
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<TelemetryReading> Readings => Set<TelemetryReading>();
    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(24).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Line).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>();
        });
        modelBuilder.Entity<TelemetryReading>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.MachineId, x.RecordedAtUtc });
            entity.HasOne(x => x.Machine).WithMany(x => x.Readings).HasForeignKey(x => x.MachineId);
        });
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.MachineId, x.CreatedAtUtc });
            entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Severity).HasConversion<string>();
            entity.Property(x => x.Status).HasConversion<string>();
            entity.HasOne(x => x.Machine).WithMany(x => x.Incidents).HasForeignKey(x => x.MachineId);
        });
    }
}
