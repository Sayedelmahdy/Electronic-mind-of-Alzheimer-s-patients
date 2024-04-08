using System;
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
                name: "FK_Locations_AspNetUsers_UserId",
                table: "Locations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Locations",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                newName: "IX_Locations_PatientId");

            migrationBuilder.AddColumn<bool>(
                name: "hasPermission",
                table: "SecretAndImportantFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "permissionEndDate",
                table: "SecretAndImportantFiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "ReminderId",
                table: "Mark_Medicine_Reminders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "MainLatitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MainLongitude",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Patients_PatientId",
                table: "Locations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Patients_PatientId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "hasPermission",
                table: "SecretAndImportantFiles");

            migrationBuilder.DropColumn(
                name: "permissionEndDate",
                table: "SecretAndImportantFiles");

            migrationBuilder.DropColumn(
                name: "MainLatitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MainLongitude",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Locations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_PatientId",
                table: "Locations",
                newName: "IX_Locations_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "ReminderId",
                table: "Mark_Medicine_Reminders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_AspNetUsers_UserId",
                table: "Locations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
