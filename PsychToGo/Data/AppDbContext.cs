using Microsoft.EntityFrameworkCore;
using PsychToGo.Models;

namespace PsychToGo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base( options )
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Psychiatrist> Psychiatrists { get; set; }
    public DbSet<Psychologist> Psychologists { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<MedicineCategory> MedicinesCategories { get; set; }
    public DbSet<PatientMedicine> PatientMedicines { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        

        builder.Entity<PatientMedicine>()
            .HasKey( pm => new { pm.PatientId, pm.MedicineId } );

        builder.Entity<PatientMedicine>()
            .HasOne( p => p.Patient )
            .WithMany( pm => pm.PatientMedicines )
            .HasForeignKey( m => m.MedicineId )
            .IsRequired();

        builder.Entity<PatientMedicine>()
            .HasOne( m => m.Medicine )
            .WithMany( pm => pm.PatientMedicines )
            .HasForeignKey( p => p.PatientId )
            .IsRequired();
    }
}