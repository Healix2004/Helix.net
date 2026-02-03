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

            // patient allergy relationship 
            builder.Entity<Patient>()
                .HasMany(p => p.Allergies)
                .WithOne(a => a.Patient)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(a => a.PatientId);

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

            //LabTestResult images 
            builder.Entity<LabImages>()
                .HasKey(l => new { l.LabTestResultId, l.ImageUrl });

            // Composite Index is CRITICAL for performance on ValidateCodeAsync
            builder.Entity<MedicalConcept>()
                .HasIndex(x => new { x.SystemUri, x.Code });

            // Index for faster text search
            builder.Entity<MedicalConcept>()
                .HasIndex(x => x.Display);
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<condition> Condition { get; set; }
        public DbSet<MedicalConcept> MedicalConcepts { get; set; }
    }
}
