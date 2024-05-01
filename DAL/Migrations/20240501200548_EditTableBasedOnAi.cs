using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditTableBasedOnAi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageUrl",
                table: "PersonWithoutAccounts",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTrainingImage",
                table: "Families",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfTrainingImage",
                table: "Families");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "PersonWithoutAccounts",
                newName: "imageUrl");
        }
    }
}
