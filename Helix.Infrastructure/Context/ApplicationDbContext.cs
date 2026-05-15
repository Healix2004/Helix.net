using Helix.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Doctor>().Property(Doctor => Doctor.Bio).HasMaxLength(500);

            // facilities doctor relationship 
            builder.Entity<Doctor>().HasMany(Doctor => Doctor.Facilities).WithMany(Facilitie => Facilitie.Doctors);
            builder.Entity<Doctor>().HasMany(d=> d.RadiologyOrders).WithOne().HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Doctor>().HasMany(d => d.LabOrders).WithOne().HasForeignKey(l => l.DoctorId).OnDelete(DeleteBehavior.Restrict);

            // patient allergy relationship 
            builder.Entity<Patient>().HasMany(p => p.Allergies).WithOne(a => a.Patient).HasPrincipalKey(p => p.Id).HasForeignKey(a => a.PatientId);
            builder.Entity<Patient>().HasMany(p => p.RadioTestResult).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Patient>().HasMany(p => p.RadiologyOrders).WithOne().HasForeignKey(r => r.PatientId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Patient>().HasMany(p => p.LabTestResult).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Patient>().HasMany(p => p.LabOrders).WithOne().OnDelete(DeleteBehavior.Cascade);

            //Encounter doctor relationship
            builder.Entity<Encounter>()
                .HasOne(e => e.Doctor)
                .WithMany(d => d.Encounters)
                .HasPrincipalKey(d => d.Id)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); 

            //Encounter patient relationship
            builder.Entity<Encounter>()
                .HasOne(e => e.patient)
                .WithMany(d => d.Encounters)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict); 


            //Encounter condition relationship
            builder.Entity<Encounter>()
                .HasMany(e => e.conditions)
                .WithOne(c => c.Encounter)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(c => c.EncounterId);

            //Encounter observation relationship
            builder.Entity<Encounter>()
                .HasMany(e => e.Observations)
                .WithOne(o => o.Encounter)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(o => o.EncounterId);

            builder.Entity<LabTestResult>().Property(lab => lab.Unit).HasMaxLength(50);
            builder.Entity<LabTestResult>().HasOne(lab => lab.Patient).WithMany(p => p.LabTestResult).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TerminologyCodeLookup>().Property(term=>term.Display).HasMaxLength(850);
            builder.Entity<TerminologyCodeLookup>().Property(term=>term.SystemUrl).HasMaxLength(50);
            builder.Entity<TerminologyCodeLookup>().Property(term=>term.Code).HasMaxLength(25);
            builder.Entity<TerminologyCodeLookup>().HasIndex(term => term.Code).IsUnique();
            builder.Entity<TerminologyCodeLookup>().HasIndex(term => term.Display);
            // Composite Index is CRITICAL for performance on ValidateCodeAsync
            builder.Entity<TerminologyCodeLookup>().HasIndex(term => new { term.Display, term.SystemUrl});

            builder.Entity<Facilitie>().Property(f=>f.Name).HasMaxLength(50);
            builder.Entity<Facilitie>().Property(f=>f.SubscriptionPlan).HasMaxLength(50);
            builder.Entity<Facilitie>().Property(f=>f.Address).HasMaxLength(250);

            builder.Entity<Diagnose>().Property(d=>d.Notes).HasMaxLength(500);
            builder.Entity<Diagnose>().HasOne(d => d.Patient).WithMany(p => p.Diagnose).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Consent>().HasOne(c => c.Patient).WithMany(p => p.Consents).HasPrincipalKey(p => p.Id).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Consent>().HasOne(c => c.Doctor).WithMany(d => d.Consents).HasPrincipalKey(d => d.Id).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LabOrder>().HasOne(l => l.Result).WithOne(p => p.LabOrder).HasForeignKey<LabOrder>(l => l.LabResultId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<LabOrder>().HasOne(l=> l.Patient).WithMany(p=>p.LabOrders).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<LabOrder>().HasOne(l => l.Doctor).WithMany(d => d.LabOrders).HasForeignKey(l => l.DoctorId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RadiologyOrder>().HasOne(r => r.Patient).WithMany(p => p.RadiologyOrders).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<RadiologyOrder>().HasOne(r => r.Doctor).WithMany(d => d.RadiologyOrders).HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RadiologyResult>().HasOne(r => r.Order).WithOne(o => o.Result).HasForeignKey<RadiologyResult>(r => r.OrderId) // The foreign key is in the Result table
            .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<RadiologyResult>().HasMany(r => r.Images).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RadiologyImage>().Property(r=>r.FileName).HasMaxLength(255);
            builder.Entity<RadiologyImage>().Property(r=>r.FilePath).HasMaxLength(256);
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<condition> Condition { get; set; }
        public DbSet<Facilitie> Facilities { get; set; }
        public DbSet<TerminologyCodeLookup> TerminologyCodes { get; set; }
        public DbSet<Consent> Consents { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabTestResult> LabTestResults { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<RadiologyResult> RadiologyResults { get; set; }
        public DbSet<RadiologyOrder> RadiologyOrders { get; set; } 
    }
}
