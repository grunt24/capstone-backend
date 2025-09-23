using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubjectInMidtermGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "MidtermGrades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MidtermGrades_SubjectId",
                table: "MidtermGrades",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_MidtermGrades_Subjects_SubjectId",
                table: "MidtermGrades",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MidtermGrades_Subjects_SubjectId",
                table: "MidtermGrades");

            migrationBuilder.DropIndex(
                name: "IX_MidtermGrades_SubjectId",
                table: "MidtermGrades");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "MidtermGrades");
        }
    }
}
