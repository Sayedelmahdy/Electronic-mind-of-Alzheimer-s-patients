﻿// <auto-generated />
using System;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(DBContext))]
    partial class DBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAL.Model.Appointment", b =>
                {
                    b.Property<string>("AppointmentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("FamilyId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PatientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AppointmentId");

                    b.HasIndex("FamilyId");

                    b.HasIndex("PatientId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("DAL.Model.GameScore", b =>
                {
                    b.Property<string>("GameScoreId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DifficultyGame")
                        .HasColumnType("int");

                    b.Property<string>("GameScoreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxScore")
                        .HasColumnType("int");

                    b.Property<string>("PatientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PatientScore")
                        .HasColumnType("int");

                    b.HasKey("GameScoreId");

                    b.HasIndex("PatientId");

                    b.ToTable("GameScores");
                });

            modelBuilder.Entity("DAL.Model.Location", b =>
                {
                    b.Property<string>("LocationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LocationId");

                    b.HasIndex("UserId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("DAL.Model.Mark_Medicine_Reminder", b =>
                {
                    b.Property<string>("MarkId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsTaken")
                        .HasColumnType("bit");

                    b.Property<DateTime>("MarkTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReminderId")
                        .HasColumnType("int");

                    b.Property<string>("medication_ReminderReminder_ID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("MarkId");

                    b.HasIndex("medication_ReminderReminder_ID");

                    b.ToTable("Mark_Medicine_Reminders");
                });

            modelBuilder.Entity("DAL.Model.Media", b =>
                {
                    b.Property<string>("Media_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Caption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FamilyId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Image_Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PatientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Upload_Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Media_Id");

                    b.HasIndex("FamilyId");

                    b.HasIndex("PatientId");

                    b.ToTable("Medias");
                });

            modelBuilder.Entity("DAL.Model.Medication_Reminders", b =>
                {
                    b.Property<string>("Reminder_ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Caregiver_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dosage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Medication_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patient_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Repeater")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Time_Period")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("caregiverId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("patientId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Reminder_ID");

                    b.HasIndex("caregiverId");

                    b.HasIndex("patientId");

                    b.ToTable("Medication_Reminders");
                });

            modelBuilder.Entity("DAL.Model.Note", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_At")
                        .HasColumnType("datetime2");

                    b.Property<string>("PatientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("DAL.Model.Report", b =>
                {
                    b.Property<string>("ReportId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CaregiverId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PatientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ReportContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ReportId");

                    b.HasIndex("CaregiverId");

                    b.HasIndex("PatientId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("DAL.Model.SecretAndImportantFile", b =>
                {
                    b.Property<string>("File_Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DocumentPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("File_Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PatientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("File_Id");

                    b.HasIndex("PatientId");

                    b.ToTable("SecretAndImportantFiles");
                });

            modelBuilder.Entity("DAL.Model.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("DAL.Model.Caregiver", b =>
                {
                    b.HasBaseType("DAL.Model.User");

                    b.ToTable("Caregivers", (string)null);
                });

            modelBuilder.Entity("DAL.Model.Family", b =>
                {
                    b.HasBaseType("DAL.Model.User");

                    b.Property<string>("PatientId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Relationility")
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("PatientId");

                    b.ToTable("Families", (string)null);
                });

            modelBuilder.Entity("DAL.Model.Patient", b =>
                {
                    b.HasBaseType("DAL.Model.User");

                    b.Property<string>("CaregiverID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DiagnosisDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FamilyCreatedId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaximumDistance")
                        .HasColumnType("int");

                    b.HasIndex("CaregiverID");

                    b.ToTable("Patients", (string)null);
                });

            modelBuilder.Entity("DAL.Model.Appointment", b =>
                {
                    b.HasOne("DAL.Model.Family", "family")
                        .WithMany()
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("family");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.GameScore", b =>
                {
                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany("GameScores")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.Location", b =>
                {
                    b.HasOne("DAL.Model.User", "user")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("DAL.Model.Mark_Medicine_Reminder", b =>
                {
                    b.HasOne("DAL.Model.Medication_Reminders", "medication_Reminder")
                        .WithMany("Mark_Medicines")
                        .HasForeignKey("medication_ReminderReminder_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("medication_Reminder");
                });

            modelBuilder.Entity("DAL.Model.Media", b =>
                {
                    b.HasOne("DAL.Model.Family", "family")
                        .WithMany("Pictures")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("family");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.Medication_Reminders", b =>
                {
                    b.HasOne("DAL.Model.Caregiver", "caregiver")
                        .WithMany()
                        .HasForeignKey("caregiverId");

                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("patientId");

                    b.Navigation("caregiver");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.Note", b =>
                {
                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.Report", b =>
                {
                    b.HasOne("DAL.Model.Caregiver", "caregiver")
                        .WithMany("Reports")
                        .HasForeignKey("CaregiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany("Reports")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("caregiver");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.SecretAndImportantFile", b =>
                {
                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("patient");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DAL.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DAL.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("DAL.Model.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DAL.Model.Caregiver", b =>
                {
                    b.HasOne("DAL.Model.User", null)
                        .WithOne()
                        .HasForeignKey("DAL.Model.Caregiver", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DAL.Model.Family", b =>
                {
                    b.HasOne("DAL.Model.User", null)
                        .WithOne()
                        .HasForeignKey("DAL.Model.Family", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Model.Patient", "patient")
                        .WithMany("families")
                        .HasForeignKey("PatientId");

                    b.Navigation("patient");
                });

            modelBuilder.Entity("DAL.Model.Patient", b =>
                {
                    b.HasOne("DAL.Model.Caregiver", "caregiver")
                        .WithMany("Patients")
                        .HasForeignKey("CaregiverID");

                    b.HasOne("DAL.Model.User", null)
                        .WithOne()
                        .HasForeignKey("DAL.Model.Patient", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("caregiver");
                });

            modelBuilder.Entity("DAL.Model.Medication_Reminders", b =>
                {
                    b.Navigation("Mark_Medicines");
                });

            modelBuilder.Entity("DAL.Model.Caregiver", b =>
                {
                    b.Navigation("Patients");

                    b.Navigation("Reports");
                });

            modelBuilder.Entity("DAL.Model.Family", b =>
                {
                    b.Navigation("Pictures");
                });

            modelBuilder.Entity("DAL.Model.Patient", b =>
                {
                    b.Navigation("GameScores");

                    b.Navigation("Reports");

                    b.Navigation("families");
                });
#pragma warning restore 612, 618
        }
    }
}
