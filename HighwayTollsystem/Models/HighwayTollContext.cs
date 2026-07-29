using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Models;

public partial class HighwayTollContext : DbContext
{
    public HighwayTollContext()
    {
    }

    public HighwayTollContext(DbContextOptions<HighwayTollContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Vehicle> Vehicles { get; set; } = null!;
    public virtual DbSet<Passage> Passages { get; set; } = null!;
    public virtual DbSet<TollGate> TollGates { get; set; } = null!;
    public virtual DbSet<VehicleInspection> VehicleInspections { get; set; } = null!;
    public virtual DbSet<Vignette> Vignettes { get; set; } = null!;
    public virtual DbSet<TrafficViolation> TrafficViolations { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId);

            // Unikátní složený index na SPZ + Zemi registrace
            entity.HasIndex(e => new { e.Spz, e.CountryCode }).IsUnique();

            entity.Property(e => e.Spz).HasMaxLength(20);
            entity.Property(e => e.CountryCode).HasMaxLength(10).HasDefaultValue("CZ");
            entity.Property(e => e.Vin).HasMaxLength(17);
            entity.Property(e => e.RegisteredAt).HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Passage>(entity =>
        {
            entity.HasKey(e => e.PassageId);

            entity.Property(e => e.CalculatedFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("NOW()");

            entity.HasOne(d => d.Vehicle)
                .WithMany(p => p.Passages)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Gate)
                .WithMany(p => p.Passages)
                .HasForeignKey(d => d.GateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TollGate>(entity =>
        {
            entity.HasKey(e => e.GateId);

            entity.Property(e => e.HighwayName).HasMaxLength(20);
            entity.Property(e => e.Direction).HasMaxLength(50);
            entity.Property(e => e.KilometerPost).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.GpsLatitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.GpsLongitude).HasColumnType("decimal(9, 6)");
        });

        modelBuilder.Entity<VehicleInspection>(entity =>
        {
            entity.HasKey(e => e.VehicleInspectionId);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(d => d.Vehicle)
                .WithMany(p => p.VehicleInspections)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Vignette>(entity =>
        {
            entity.HasKey(e => e.VignetteId);

            entity.HasIndex(e => new { e.VehicleId, e.ValidFrom, e.ValidTo });

            entity.Property(e => e.PurchaseDate).HasDefaultValueSql("NOW()");

            entity.HasOne(d => d.Vehicle)
                .WithMany(p => p.Vignettes)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TrafficViolation>(entity =>
        {
            entity.HasKey(e => e.ViolationId);

            entity.Property(e => e.ActualPenaltyAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Details).HasMaxLength(255);

            entity.HasOne(d => d.Passage)
                .WithMany(p => p.TrafficViolations)
                .HasForeignKey(d => d.PassageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}