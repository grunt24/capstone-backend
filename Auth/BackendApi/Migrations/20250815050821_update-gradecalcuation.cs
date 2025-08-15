using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class updategradecalcuation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PG",
                table: "MidtermQuizLists");

            migrationBuilder.DropColumn(
                name: "QuizPercentage",
                table: "MidtermQuizLists");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "GradePointEquivalents");

            migrationBuilder.DropColumn(
                name: "Attendance",
                table: "ClassStanding");

            migrationBuilder.DropColumn(
                name: "Average",
                table: "ClassStanding");

            migrationBuilder.DropColumn(
                name: "ClassStandingPercentage",
                table: "ClassStanding");

            migrationBuilder.DropColumn(
                name: "PG",
                table: "ClassStanding");

            migrationBuilder.DropColumn(
                name: "Recitation",
                table: "ClassStanding");

            migrationBuilder.RenameColumn(
                name: "SEPSCore",
                table: "Users",
                newName: "SEPScore");

            migrationBuilder.RenameColumn(
                name: "SEPPercentage",
                table: "Users",
                newName: "TotalQuizScore");

            migrationBuilder.RenameColumn(
                name: "ProjectScorePercentage",
                table: "Users",
                newName: "RecitationScore");

            migrationBuilder.RenameColumn(
                name: "PrelimGradeTotalScore",
                table: "Users",
                newName: "PrelimTotal");

            migrationBuilder.RenameColumn(
                name: "PrelimGrade",
                table: "Users",
                newName: "PrelimScore");

            migrationBuilder.RenameColumn(
                name: "PercentageOfMidtermAndPrelim",
                table: "Users",
                newName: "MidtermTotal");

            migrationBuilder.RenameColumn(
                name: "MidtermGradeTotalSCore",
                table: "Users",
                newName: "MidtermScore");

            migrationBuilder.RenameColumn(
                name: "MidtermGradeScore",
                table: "Users",
                newName: "ClassStandingTotalScore");

            migrationBuilder.RenameColumn(
                name: "Average",
                table: "Users",
                newName: "AttendanceScore");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "MidtermQuizLists",
                newName: "Label");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ClassStanding",
                newName: "Label");

            migrationBuilder.AlterColumn<double>(
                name: "TotalScorePerlimAndMidterm",
                table: "Users",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalMidtermGrade",
                table: "Users",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClassStandingAverage",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClassStandingPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClassStandingWeighted",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CombinedPrelimMidtermAverage",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MidtermExamWeighted",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MidtermPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OverallPrelimAndMidterm",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProjectPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProjectWeighted",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuizPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuizWeighted",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SEPPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SEPWeighted",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalMidtermGradeRounded",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalQuizScore",
                table: "MidtermQuizLists",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QuizScore",
                table: "MidtermQuizLists",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Total",
                table: "ClassStanding",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ClassStanding",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassStandingAverage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassStandingPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassStandingWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CombinedPrelimMidtermAverage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermExamWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OverallPrelimAndMidterm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProjectPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProjectWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuizPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuizWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalMidtermGradeRounded",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "SEPScore",
                table: "Users",
                newName: "SEPSCore");

            migrationBuilder.RenameColumn(
                name: "TotalQuizScore",
                table: "Users",
                newName: "SEPPercentage");

            migrationBuilder.RenameColumn(
                name: "RecitationScore",
                table: "Users",
                newName: "ProjectScorePercentage");

            migrationBuilder.RenameColumn(
                name: "PrelimTotal",
                table: "Users",
                newName: "PrelimGradeTotalScore");

            migrationBuilder.RenameColumn(
                name: "PrelimScore",
                table: "Users",
                newName: "PrelimGrade");

            migrationBuilder.RenameColumn(
                name: "MidtermTotal",
                table: "Users",
                newName: "PercentageOfMidtermAndPrelim");

            migrationBuilder.RenameColumn(
                name: "MidtermScore",
                table: "Users",
                newName: "MidtermGradeTotalSCore");

            migrationBuilder.RenameColumn(
                name: "ClassStandingTotalScore",
                table: "Users",
                newName: "MidtermGradeScore");

            migrationBuilder.RenameColumn(
                name: "AttendanceScore",
                table: "Users",
                newName: "Average");

            migrationBuilder.RenameColumn(
                name: "Label",
                table: "MidtermQuizLists",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Label",
                table: "ClassStanding",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "TotalScorePerlimAndMidterm",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMidtermGrade",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalQuizScore",
                table: "MidtermQuizLists",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuizScore",
                table: "MidtermQuizLists",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PG",
                table: "MidtermQuizLists",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "QuizPercentage",
                table: "MidtermQuizLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "GradePointEquivalents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Total",
                table: "ClassStanding",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ClassStanding",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Attendance",
                table: "ClassStanding",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Average",
                table: "ClassStanding",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ClassStandingPercentage",
                table: "ClassStanding",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PG",
                table: "ClassStanding",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Recitation",
                table: "ClassStanding",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
