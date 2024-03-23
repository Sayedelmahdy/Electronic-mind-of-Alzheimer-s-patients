using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangePictureToMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pictures");

          

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    Picture_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Image_Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Upload_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FamilyId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.Picture_Id);
                    table.ForeignKey(
                        name: "FK_Medias_Families_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Medias_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medias_FamilyId",
                table: "Medias",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_PatientId",
                table: "Medias",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    Picture_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FamilyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image_Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Upload_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Picture_Id);
                    table.ForeignKey(
                        name: "FK_Pictures_Families_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pictures_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

         

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_FamilyId",
                table: "Pictures",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_PatientId",
                table: "Pictures",
                column: "PatientId");
        }
    }
}
