using DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Patient>(entity => { entity.ToTable("Patients"); });
            builder.Entity<Family>(entity => { entity.ToTable("Familys"); });
            builder.Entity<Caregiver>(entity => { entity.ToTable("Caregivers"); });
            List<IdentityRole> roles = new List<IdentityRole>
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
            builder.Entity<IdentityRole>().HasData(roles);


        }
    }
}
