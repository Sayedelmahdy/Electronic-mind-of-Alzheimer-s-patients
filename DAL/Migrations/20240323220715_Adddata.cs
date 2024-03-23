using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Adddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "174cc7f4-082e-42ae-bffc-958974817976", "e8d6d4b4-817c-45c0-9d43-06e41133a2c5", "Family", "FAMILY" },
                    { "90e11084-a3a0-4431-8109-128a3e3e2264", "900aa4ff-0512-4565-8095-614234ed9ecd", "Caregiver", "CAREGIVER" },
                    { "bc321689-9b8c-47b4-8b4c-672ded8dafb1", "0decd453-983b-4bfb-bb4f-51c6077b98ec", "Patient", "PATIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "174cc7f4-082e-42ae-bffc-958974817976");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90e11084-a3a0-4431-8109-128a3e3e2264");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bc321689-9b8c-47b4-8b4c-672ded8dafb1");
        }
    }
}
