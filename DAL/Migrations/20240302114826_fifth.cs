using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "70c5701f-0bb0-422d-965c-68eb1a371979", "08fb01b0-2d72-48e9-90e8-bf028de7396b", "Family", "FAMILY" },
                    { "e1dc1812-bb9a-42c6-b2e8-5267ddf63383", "a8123c14-ccad-4e87-98a2-23cf7021728d", "Patient", "PATIENT" },
                    { "f3713832-fd17-4d47-816a-ae32e6e090ae", "da78b88f-6a32-4146-8db5-998841236555", "Caregiver", "CAREGIVER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "17b731b0-e3ee-4122-8b8c-5ddab5f50a2a", "78807915-e339-43c5-ab71-e262592818d3", "Patient", "PATIENT" },
                    { "1893492f-6fd3-4500-81a4-2cceeedd4e52", "1f2db763-e202-4bc8-9148-14080bad93d9", "Family", "FAMILY" },
                    { "2c388155-d37f-46b2-bb2e-545be91b28f7", "740af42c-1937-4212-9044-ce2066ffa076", "Caregiver", "CAREGIVER" }
                });
        }
    }
}
