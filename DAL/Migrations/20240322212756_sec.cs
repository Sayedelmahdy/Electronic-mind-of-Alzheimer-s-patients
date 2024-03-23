using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class sec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ddd787c-9894-4cae-b7f6-3483fdec4068");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9bec7098-1d58-4867-906b-7050a8e11256");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c980f667-04e2-462c-8d08-3b7b789aece7");

            migrationBuilder.AddColumn<string>(
                name: "FamilyCreatedId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumDistance",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamilyCreatedId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MaximumDistance",
                table: "Patients");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3ddd787c-9894-4cae-b7f6-3483fdec4068", "8f0fddcb-f285-453e-8d36-50d9301232cf", "Caregiver", "CAREGIVER" },
                    { "9bec7098-1d58-4867-906b-7050a8e11256", "7351ce35-cdc4-445f-98e9-c3c8f75659ca", "Family", "FAMILY" },
                    { "c980f667-04e2-462c-8d08-3b7b789aece7", "2d252eee-0e7e-4503-a4c6-6e52d2cc0104", "Patient", "PATIENT" }
                });
        }
    }
}
