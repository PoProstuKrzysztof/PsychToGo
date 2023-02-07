﻿using Microsoft.EntityFrameworkCore;
using PsychToGo.Models;

namespace PsychToGo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base( options )
    {
    }

    public DbSet<LocalUser> LocalUsers { get; set; }
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
            .HasForeignKey( m => m.PatientId )
            .IsRequired();

        builder.Entity<PatientMedicine>()
            .HasOne( m => m.Medicine )
            .WithMany( pm => pm.PatientMedicines )
            .HasForeignKey( p => p.MedicineId )
            .IsRequired();

        builder.Entity<Patient>()
            .HasOne(p => p.Psychologist)
            .WithMany( ps => ps.Patients)
            .HasForeignKey( p => p.PsychologistId)
            .IsRequired();

        builder.Entity<Patient>()
            .HasOne( p => p.Psychiatrist )
            .WithMany( p => p.Patients )
            .HasForeignKey( p => p.PsychiatristId )
            .IsRequired();

            
            
    }
}