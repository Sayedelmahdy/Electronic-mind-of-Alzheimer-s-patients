using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixMedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mark_Medicine_Reminders");

            migrationBuilder.DropTable(
                name: "Medication_Reminders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medication_Reminders",
                columns: table => new
                {
                    Reminder_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    caregiverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    patientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Caregiver_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Medication_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patient_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Repeater = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time_Period = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication_Reminders", x => x.Reminder_ID);
                    table.ForeignKey(
                        name: "FK_Medication_Reminders_Caregivers_caregiverId",
                        column: x => x.caregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Medication_Reminders_Patients_patientId",
                        column: x => x.patientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mark_Medicine_Reminders",
                columns: table => new
                {
                    MarkId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    medication_ReminderReminder_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsTaken = table.Column<bool>(type: "bit", nullable: false),
                    MarkTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReminderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mark_Medicine_Reminders", x => x.MarkId);
                    table.ForeignKey(
                        name: "FK_Mark_Medicine_Reminders_Medication_Reminders_medication_ReminderReminder_ID",
                        column: x => x.medication_ReminderReminder_ID,
                        principalTable: "Medication_Reminders",
                        principalColumn: "Reminder_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mark_Medicine_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders",
                column: "medication_ReminderReminder_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_Reminders_caregiverId",
                table: "Medication_Reminders",
                column: "caregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_Reminders_patientId",
                table: "Medication_Reminders",
                column: "patientId");
        }
    }
}
