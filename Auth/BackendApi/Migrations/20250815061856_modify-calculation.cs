using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class modifycalculation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassStanding",
                table: "GradeWeights");

            migrationBuilder.RenameColumn(
                name: "SEPWeighted",
                table: "MidtermGrades",
                newName: "SEPWeightedTotal");

            migrationBuilder.RenameColumn(
                name: "QuizWeighted",
                table: "MidtermGrades",
                newName: "QuizWeightedTotal");

            migrationBuilder.RenameColumn(
                name: "ProjectWeighted",
                table: "MidtermGrades",
                newName: "ProjectWeightedTotal");

            migrationBuilder.RenameColumn(
                name: "MidtermExamWeighted",
                table: "MidtermGrades",
                newName: "MidtermWeightedTotal");

            migrationBuilder.RenameColumn(
                name: "ClassStandingWeighted",
                table: "MidtermGrades",
                newName: "ClassStandingWeightedTotal");

            migrationBuilder.RenameColumn(
                name: "SEP",
                table: "GradeWeights",
                newName: "SEPWeighted");

            migrationBuilder.RenameColumn(
                name: "Quizzes",
                table: "GradeWeights",
                newName: "QuizWeighted");

            migrationBuilder.RenameColumn(
                name: "Project",
                table: "GradeWeights",
                newName: "ProjectWeighted");

            migrationBuilder.RenameColumn(
                name: "Prelim",
                table: "GradeWeights",
                newName: "MidtermWeighted");

            migrationBuilder.RenameColumn(
                name: "MidtermExam",
                table: "GradeWeights",
                newName: "ClassStandingWeighted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SEPWeightedTotal",
                table: "MidtermGrades",
                newName: "SEPWeighted");

            migrationBuilder.RenameColumn(
                name: "QuizWeightedTotal",
                table: "MidtermGrades",
                newName: "QuizWeighted");

            migrationBuilder.RenameColumn(
                name: "ProjectWeightedTotal",
                table: "MidtermGrades",
                newName: "ProjectWeighted");

            migrationBuilder.RenameColumn(
                name: "MidtermWeightedTotal",
                table: "MidtermGrades",
                newName: "MidtermExamWeighted");

            migrationBuilder.RenameColumn(
                name: "ClassStandingWeightedTotal",
                table: "MidtermGrades",
                newName: "ClassStandingWeighted");

            migrationBuilder.RenameColumn(
                name: "SEPWeighted",
                table: "GradeWeights",
                newName: "SEP");

            migrationBuilder.RenameColumn(
                name: "QuizWeighted",
                table: "GradeWeights",
                newName: "Quizzes");

            migrationBuilder.RenameColumn(
                name: "ProjectWeighted",
                table: "GradeWeights",
                newName: "Project");

            migrationBuilder.RenameColumn(
                name: "MidtermWeighted",
                table: "GradeWeights",
                newName: "Prelim");

            migrationBuilder.RenameColumn(
                name: "ClassStandingWeighted",
                table: "GradeWeights",
                newName: "MidtermExam");

            migrationBuilder.AddColumn<decimal>(
                name: "ClassStanding",
                table: "GradeWeights",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
