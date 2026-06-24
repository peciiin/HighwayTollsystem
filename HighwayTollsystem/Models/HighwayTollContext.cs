using System;
using System.Collections.Generic;
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

    public virtual DbSet<Passage> Passages { get; set; }

    public virtual DbSet<Stk> Stks { get; set; }

    public virtual DbSet<TollGate> TollGates { get; set; }

    public virtual DbSet<TrafficViolation> TrafficViolations { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<Vignette> Vignettes { get; set; }

    public virtual DbSet<ViolationType> ViolationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=HighwayTollSystem;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Passage>(entity =>
        {
            entity.HasKey(e => e.PassageId).HasName("PK__Passages__CC0F002C4A8A415B");

            entity.HasIndex(e => e.Spz, "IX_Passages_Spz");

            entity.Property(e => e.CalculatedFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Spz)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.Gate).WithMany(p => p.Passages)
                .HasForeignKey(d => d.GateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Passages__GateId__6D0D32F4");

            entity.HasOne(d => d.SpzNavigation).WithMany(p => p.Passages)
                .HasForeignKey(d => d.Spz)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Passages__Spz__6C190EBB");
        });

        modelBuilder.Entity<Stk>(entity =>
        {
            entity.HasKey(e => e.StkId).HasName("PK__Stk__56A80AECC8856588");

            entity.ToTable("Stk");

            entity.HasIndex(e => e.Spz, "IX_Stk_Spz");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmissionsValidTo).HasColumnType("datetime");
            entity.Property(e => e.Spz)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ValidTo).HasColumnType("datetime");

            entity.HasOne(d => d.SpzNavigation).WithMany(p => p.Stks)
                .HasForeignKey(d => d.Spz)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stk__Spz__693CA210");
        });

        modelBuilder.Entity<TollGate>(entity =>
        {
            entity.HasKey(e => e.GateId).HasName("PK__TollGate__9582C65039CBB277");

            entity.Property(e => e.Direction)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GpsLatitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.GpsLongitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.HighwayName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.KilometerPost).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<TrafficViolation>(entity =>
        {
            entity.HasKey(e => e.ViolationId).HasName("PK__TrafficV__18B6DC086635FFDB");

            entity.Property(e => e.ActualPenaltyAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Details)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Passage).WithMany(p => p.TrafficViolations)
                .HasForeignKey(d => d.PassageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrafficVi__Passa__72C60C4A");

            entity.HasOne(d => d.ViolationType).WithMany(p => p.TrafficViolations)
                .HasForeignKey(d => d.ViolationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrafficVi__Viola__73BA3083");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Spz).HasName("PK__Vehicles__CA1E142DEAD30959");

            entity.Property(e => e.Spz)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EmissionClass)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Type).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicles__TypeId__5FB337D6");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleT__3214EC0735D28DED");

            entity.Property(e => e.BaseTarif).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TypeName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vignette>(entity =>
        {
            entity.HasKey(e => e.VignetteId).HasName("PK__Vignette__C81AEC1FA0C45C6E");

            entity.HasIndex(e => new { e.Spz, e.ValidFrom, e.ValidTo }, "IX_Vignettes_Spz_Dates");

            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Spz)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
            entity.Property(e => e.ValidTo).HasColumnType("datetime");

            entity.HasOne(d => d.SpzNavigation).WithMany(p => p.Vignettes)
                .HasForeignKey(d => d.Spz)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vignettes__Spz__656C112C");
        });

        modelBuilder.Entity<ViolationType>(entity =>
        {
            entity.HasKey(e => e.ViolationTypeId).HasName("PK__Violatio__3B1A4D1D71E82FAB");

            entity.HasIndex(e => e.Code, "UQ__Violatio__A25C5AA7571436D1").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DefaultPenaltyAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
