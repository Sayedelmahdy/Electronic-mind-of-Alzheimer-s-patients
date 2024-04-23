using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class TbPersonWithoutAcc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionForPatient",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonWithoutAccounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainLongitude = table.Column<double>(type: "float", nullable: false),
                    MainLatitude = table.Column<double>(type: "float", nullable: false),
                    Relationility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionForPatient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonWithoutAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonWithoutAccounts_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonWithoutAccounts_PatientId",
                table: "PersonWithoutAccounts",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonWithoutAccounts");

            migrationBuilder.DropColumn(
                name: "DescriptionForPatient",
                table: "AspNetUsers");
        }
    }
}
