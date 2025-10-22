using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class FixQuizListRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassStanding_FinalsGrades_FinalsGradeId",
                table: "ClassStanding");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizLists_FinalsGrades_FinalsGradeId",
                table: "QuizLists");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassStanding_FinalsGrades_FinalsGradeId",
                table: "ClassStanding",
                column: "FinalsGradeId",
                principalTable: "FinalsGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizLists_FinalsGrades_FinalsGradeId",
                table: "QuizLists",
                column: "FinalsGradeId",
                principalTable: "FinalsGrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassStanding_FinalsGrades_FinalsGradeId",
                table: "ClassStanding");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizLists_FinalsGrades_FinalsGradeId",
                table: "QuizLists");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassStanding_FinalsGrades_FinalsGradeId",
                table: "ClassStanding",
                column: "FinalsGradeId",
                principalTable: "FinalsGrades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizLists_FinalsGrades_FinalsGradeId",
                table: "QuizLists",
                column: "FinalsGradeId",
                principalTable: "FinalsGrades",
                principalColumn: "Id");
        }
    }
}
