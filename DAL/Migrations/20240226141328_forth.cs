using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class forth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyPatient_Familys_FamilysId",
                table: "FamilyPatient");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyPatient_Patients_PatientsId",
                table: "FamilyPatient");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4387e364-6f00-446e-9fd3-e8e30fbd7310");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83939878-b5cf-451e-b656-59c72b0bef89");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a3fa8bc2-8b53-4ad5-82b5-e3e4cdef2cf5");

            migrationBuilder.DropColumn(
                name: "Relationility",
                table: "Familys");

            migrationBuilder.RenameColumn(
                name: "PatientsId",
                table: "FamilyPatient",
                newName: "PatientId");

            migrationBuilder.RenameColumn(
                name: "FamilysId",
                table: "FamilyPatient",
                newName: "FamilyId");

            migrationBuilder.RenameIndex(
                name: "IX_FamilyPatient_PatientsId",
                table: "FamilyPatient",
                newName: "IX_FamilyPatient_PatientId");

            migrationBuilder.AddColumn<string>(
                name: "Relationility",
                table: "FamilyPatient",
                type: "nvarchar(max)",
                nullable: true);

           

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyPatient_Familys_FamilyId",
                table: "FamilyPatient",
                column: "FamilyId",
                principalTable: "Familys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyPatient_Patients_PatientId",
                table: "FamilyPatient",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyPatient_Familys_FamilyId",
                table: "FamilyPatient");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyPatient_Patients_PatientId",
                table: "FamilyPatient");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "17b731b0-e3ee-4122-8b8c-5ddab5f50a2a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1893492f-6fd3-4500-81a4-2cceeedd4e52");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c388155-d37f-46b2-bb2e-545be91b28f7");

            migrationBuilder.DropColumn(
                name: "Relationility",
                table: "FamilyPatient");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "FamilyPatient",
                newName: "PatientsId");

            migrationBuilder.RenameColumn(
                name: "FamilyId",
                table: "FamilyPatient",
                newName: "FamilysId");

            migrationBuilder.RenameIndex(
                name: "IX_FamilyPatient_PatientId",
                table: "FamilyPatient",
                newName: "IX_FamilyPatient_PatientsId");

            migrationBuilder.AddColumn<string>(
                name: "Relationility",
                table: "Familys",
                type: "nvarchar(max)",
                nullable: true);

           
            migrationBuilder.AddForeignKey(
                name: "FK_FamilyPatient_Familys_FamilysId",
                table: "FamilyPatient",
                column: "FamilysId",
                principalTable: "Familys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyPatient_Patients_PatientsId",
                table: "FamilyPatient",
                column: "PatientsId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
