using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Context
{
    public class DBContext: IdentityDbContext<User>
    {
        public DBContext() {
           
        }
        public DBContext(DbContextOptions options)
       : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }
       
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Family> Families { get; set; }
        public virtual DbSet<Caregiver> Caregivers { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Mark_Medicine_Reminder> MarkMedicineReminders { get; set; }
        public virtual DbSet<Medication_Reminders> Medication_Reminders { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<SecretAndImportantFile> SecretAndImportantFiles { get; set; }
        public virtual DbSet<GameScore> GameScores { get; set; }
        public virtual DbSet<Report> Reports { get; set; } // يااااااااااااارب
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Patient>(entity => { entity.ToTable("Patients"); });
            builder.Entity<Family>(entity => { entity.ToTable("Families"); });
            builder.Entity<Caregiver>(entity => { entity.ToTable("Caregivers"); });
            builder.Entity<Medication_Reminders>()
              .HasOne(mr => mr.caregiver)
              .WithMany()
              .HasForeignKey(mr => mr.Caregiver_Id);

            builder.Entity<Medication_Reminders>()
                .HasOne(mr => mr.patient)
                .WithMany()
                .HasForeignKey(mr => mr.Patient_Id);
            /*
                        builder.Entity<FamilyPatient>()
                        .HasKey(fp => new { fp.FamilyId, fp.PatientId });

                        builder.Entity<FamilyPatient>()
                            .HasOne(fp => fp.Family)
                            .WithMany(f => f.FamilyPatients)
                            .HasForeignKey(fp => fp.FamilyId);

                        builder.Entity<FamilyPatient>()
                            .HasOne(fp => fp.Patient)
                            .WithMany(p => p.FamilyPatients)
                            .HasForeignKey(fp => fp.PatientId);*/
            /*List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Family",
                    NormalizedName = "FAMILY",
                    ConcurrencyStamp=Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Name = "Patient",
                    NormalizedName = "PATIENT",
                    ConcurrencyStamp=Guid.NewGuid().ToString()
                 },
                new IdentityRole
                {
                    Name = "Caregiver",
                    NormalizedName = "CAREGIVER",
                    ConcurrencyStamp=Guid.NewGuid().ToString()
                 },
            };
            builder.Entity<IdentityRole>().HasData(roles);*/

        }
    }
}
