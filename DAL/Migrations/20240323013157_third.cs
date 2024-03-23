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
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "00904b09-8a38-4e46-81f4-3156f7ffdcb6", "87a92256-04ce-4935-96ec-80a65588b0c6", "Caregiver", "CAREGIVER" },
                    { "46344206-1d07-4279-9e28-4f809f77cb57", "fa078624-a975-4058-9d3b-a3eff12f77da", "Patient", "PATIENT" },
                    { "55719995-5761-41bb-b629-2450f83656e3", "4a63a658-4452-4627-b6a0-bdb99a647b32", "Family", "FAMILY" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "00904b09-8a38-4e46-81f4-3156f7ffdcb6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46344206-1d07-4279-9e28-4f809f77cb57");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55719995-5761-41bb-b629-2450f83656e3");
        }
    }
}
