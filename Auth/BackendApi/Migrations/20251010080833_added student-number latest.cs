using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class addedstudentnumberlatest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentNumber",
                table: "MidtermGrades",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentNumber",
                table: "FinalsGrades",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentNumber",
                table: "MidtermGrades");

            migrationBuilder.DropColumn(
                name: "StudentNumber",
                table: "FinalsGrades");
        }
    }
}
