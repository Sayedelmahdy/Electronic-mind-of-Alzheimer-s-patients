using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditMedication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Medicine_Reminders_Medication_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mark_Medicine_Reminders",
                table: "Mark_Medicine_Reminders");

            migrationBuilder.DropColumn(
                name: "Time_Period",
                table: "Medication_Reminders");

            migrationBuilder.RenameTable(
                name: "Mark_Medicine_Reminders",
                newName: "MarkMedicineReminders");

            migrationBuilder.RenameColumn(
                name: "ReminderId",
                table: "MarkMedicineReminders",
                newName: "MedicationReminderId");

            migrationBuilder.RenameIndex(
                name: "IX_Mark_Medicine_Reminders_medication_ReminderReminder_ID",
                table: "MarkMedicineReminders",
                newName: "IX_MarkMedicineReminders_medication_ReminderReminder_ID");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Medication_Reminders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Medcine_Type",
                table: "Medication_Reminders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarkMedicineReminders",
                table: "MarkMedicineReminders",
                column: "MarkId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarkMedicineReminders_Medication_Reminders_medication_ReminderReminder_ID",
                table: "MarkMedicineReminders",
                column: "medication_ReminderReminder_ID",
                principalTable: "Medication_Reminders",
                principalColumn: "Reminder_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarkMedicineReminders_Medication_Reminders_medication_ReminderReminder_ID",
                table: "MarkMedicineReminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarkMedicineReminders",
                table: "MarkMedicineReminders");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Medication_Reminders");

            migrationBuilder.DropColumn(
                name: "Medcine_Type",
                table: "Medication_Reminders");

            migrationBuilder.RenameTable(
                name: "MarkMedicineReminders",
                newName: "Mark_Medicine_Reminders");

            migrationBuilder.RenameColumn(
                name: "MedicationReminderId",
                table: "Mark_Medicine_Reminders",
                newName: "ReminderId");

            migrationBuilder.RenameIndex(
                name: "IX_MarkMedicineReminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders",
                newName: "IX_Mark_Medicine_Reminders_medication_ReminderReminder_ID");

            migrationBuilder.AddColumn<string>(
                name: "Time_Period",
                table: "Medication_Reminders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mark_Medicine_Reminders",
                table: "Mark_Medicine_Reminders",
                column: "MarkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Medicine_Reminders_Medication_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders",
                column: "medication_ReminderReminder_ID",
                principalTable: "Medication_Reminders",
                principalColumn: "Reminder_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
