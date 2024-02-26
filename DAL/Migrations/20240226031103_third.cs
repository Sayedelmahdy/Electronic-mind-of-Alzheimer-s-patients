using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Caregivers_CaregiverID",
                table: "Patients");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b009d16-cbb6-4883-b335-6c657261046f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9b83b4ce-433a-4640-af71-200c89640ee6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ddbf6245-02e9-477d-92f3-ef9769d57745");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DiagnosisDate",
                table: "Patients",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CaregiverID",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Relationility",
                table: "Familys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4387e364-6f00-446e-9fd3-e8e30fbd7310", "d5b9bf43-6578-4024-ac30-775d8b0438b9", "Patient", "PATIENT" },
                    { "83939878-b5cf-451e-b656-59c72b0bef89", "23a58ade-4032-45e4-922b-a98683a2640f", "Caregiver", "CAREGIVER" },
                    { "a3fa8bc2-8b53-4ad5-82b5-e3e4cdef2cf5", "db721547-1411-4ac2-b3f9-d029093f8aa6", "Family", "FAMILY" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Caregivers_CaregiverID",
                table: "Patients",
                column: "CaregiverID",
                principalTable: "Caregivers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Caregivers_CaregiverID",
                table: "Patients");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "DiagnosisDate",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CaregiverID",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Relationility",
                table: "Familys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6b009d16-cbb6-4883-b335-6c657261046f", "4ccab050-88f5-40a8-b4f0-594c31e07219", "Caregiver", "CAREGIVER" },
                    { "9b83b4ce-433a-4640-af71-200c89640ee6", "daab67c5-93d2-41ac-82aa-8fbb2cba4b79", "Family", "FAMILY" },
                    { "ddbf6245-02e9-477d-92f3-ef9769d57745", "ce9ce161-1ca1-47a9-97e9-ee718910bb4f", "Patient", "PATIENT" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Caregivers_CaregiverID",
                table: "Patients",
                column: "CaregiverID",
                principalTable: "Caregivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
