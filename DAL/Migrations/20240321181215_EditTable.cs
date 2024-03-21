using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Familys_AspNetUsers_Id",
                table: "Familys");

            migrationBuilder.DropTable(
                name: "FamilyPatient");

            migrationBuilder.DropTable(
                name: "User_Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Familys",
                table: "Familys");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "SecretAndImportantFiles");

            migrationBuilder.RenameTable(
                name: "Familys",
                newName: "Families");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "SecretAndImportantFiles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "Pictures",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FamilyId",
                table: "Pictures",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "Notes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "caregiverId",
                table: "Medication_Reminders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "patientId",
                table: "Medication_Reminders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Locations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FamilyId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Families",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Relationility",
                table: "Families",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Families",
                table: "Families",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SecretAndImportantFiles_PatientId",
                table: "SecretAndImportantFiles",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_FamilyId",
                table: "Pictures",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_PatientId",
                table: "Pictures",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_PatientId",
                table: "Notes",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_Reminders_caregiverId",
                table: "Medication_Reminders",
                column: "caregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_Reminders_patientId",
                table: "Medication_Reminders",
                column: "patientId");

            migrationBuilder.CreateIndex(
                name: "IX_Mark_Medicine_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders",
                column: "medication_ReminderReminder_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_FamilyId",
                table: "Appointments",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Families_PatientId",
                table: "Families",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Families_FamilyId",
                table: "Appointments",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Families_AspNetUsers_Id",
                table: "Families",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Families_Patients_PatientId",
                table: "Families",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_AspNetUsers_UserId",
                table: "Locations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Medicine_Reminders_Medication_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders",
                column: "medication_ReminderReminder_ID",
                principalTable: "Medication_Reminders",
                principalColumn: "Reminder_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medication_Reminders_Caregivers_caregiverId",
                table: "Medication_Reminders",
                column: "caregiverId",
                principalTable: "Caregivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medication_Reminders_Patients_patientId",
                table: "Medication_Reminders",
                column: "patientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Patients_PatientId",
                table: "Notes",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Families_FamilyId",
                table: "Pictures",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Patients_PatientId",
                table: "Pictures",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SecretAndImportantFiles_Patients_PatientId",
                table: "SecretAndImportantFiles",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Families_FamilyId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_AspNetUsers_Id",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_Families_Patients_PatientId",
                table: "Families");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_AspNetUsers_UserId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Medicine_Reminders_Medication_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Medication_Reminders_Caregivers_caregiverId",
                table: "Medication_Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Medication_Reminders_Patients_patientId",
                table: "Medication_Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Patients_PatientId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Families_FamilyId",
                table: "Pictures");

            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Patients_PatientId",
                table: "Pictures");

            migrationBuilder.DropForeignKey(
                name: "FK_SecretAndImportantFiles_Patients_PatientId",
                table: "SecretAndImportantFiles");

            migrationBuilder.DropIndex(
                name: "IX_SecretAndImportantFiles_PatientId",
                table: "SecretAndImportantFiles");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_FamilyId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_PatientId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Notes_PatientId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Medication_Reminders_caregiverId",
                table: "Medication_Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Medication_Reminders_patientId",
                table: "Medication_Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Mark_Medicine_Reminders_medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Locations_UserId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_FamilyId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Families",
                table: "Families");

            migrationBuilder.DropIndex(
                name: "IX_Families_PatientId",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "caregiverId",
                table: "Medication_Reminders");

            migrationBuilder.DropColumn(
                name: "patientId",
                table: "Medication_Reminders");

            migrationBuilder.DropColumn(
                name: "medication_ReminderReminder_ID",
                table: "Mark_Medicine_Reminders");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "Relationility",
                table: "Families");

            migrationBuilder.RenameTable(
                name: "Families",
                newName: "Familys");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "SecretAndImportantFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "SecretAndImportantFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FamilyId",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Familys",
                table: "Familys",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FamilyPatient",
                columns: table => new
                {
                    FamilyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Relationility = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyPatient", x => new { x.FamilyId, x.PatientId });
                    table.ForeignKey(
                        name: "FK_FamilyPatient_Familys_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Familys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamilyPatient_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Appointments",
                columns: table => new
                {
                    UserAppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Appointments", x => x.UserAppointmentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FamilyPatient_PatientId",
                table: "FamilyPatient",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Familys_AspNetUsers_Id",
                table: "Familys",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
