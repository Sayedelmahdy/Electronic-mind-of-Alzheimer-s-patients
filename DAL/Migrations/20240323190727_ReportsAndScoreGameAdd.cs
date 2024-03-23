using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReportsAndScoreGameAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "GameScores",
                columns: table => new
                {
                    GameScoreId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameScoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DifficultyGame = table.Column<int>(type: "int", nullable: false),
                    PatientScore = table.Column<int>(type: "int", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScores", x => x.GameScoreId);
                    table.ForeignKey(
                        name: "FK_GameScores_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaregiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Reports_Caregivers_CaregiverId",
                        column: x => x.CaregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reports_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_PatientId",
                table: "GameScores",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CaregiverId",
                table: "Reports",
                column: "CaregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PatientId",
                table: "Reports",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameScores");

            migrationBuilder.DropTable(
                name: "Reports");

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
    }
}
