using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class addedstudentnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentNumber",
                table: "Users");
        }
    }
}
