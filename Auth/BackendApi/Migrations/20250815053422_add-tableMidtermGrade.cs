using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class addtableMidtermGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassStanding_Users_MidtermGradeId",
                table: "ClassStanding");

            migrationBuilder.DropForeignKey(
                name: "FK_MidtermQuizLists_Users_MidtermGradeId",
                table: "MidtermQuizLists");

            migrationBuilder.DropColumn(
                name: "AttendanceScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassStandingAverage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassStandingPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassStandingTotalScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassStandingWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CombinedPrelimMidtermAverage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GradePointEquivalent",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermExamWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OverallPrelimAndMidterm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrelimScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrelimTotal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProjectPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProjectScore",
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
                name: "RecitationScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPPG",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPWeighted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalMidtermGrade",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalMidtermGradeRounded",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalQuizScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalScorePerlimAndMidterm",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "MidtermGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TotalQuizScore = table.Column<int>(type: "int", nullable: false),
                    QuizPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuizWeighted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecitationScore = table.Column<int>(type: "int", nullable: false),
                    AttendanceScore = table.Column<int>(type: "int", nullable: false),
                    ClassStandingTotalScore = table.Column<int>(type: "int", nullable: false),
                    ClassStandingAverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassStandingPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassStandingWeighted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SEPScore = table.Column<int>(type: "int", nullable: false),
                    SEPPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SEPWeighted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectScore = table.Column<int>(type: "int", nullable: false),
                    ProjectPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectWeighted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrelimScore = table.Column<int>(type: "int", nullable: false),
                    PrelimTotal = table.Column<int>(type: "int", nullable: false),
                    MidtermScore = table.Column<int>(type: "int", nullable: false),
                    MidtermTotal = table.Column<int>(type: "int", nullable: false),
                    TotalScorePerlimAndMidterm = table.Column<double>(type: "float", nullable: false),
                    OverallPrelimAndMidterm = table.Column<double>(type: "float", nullable: false),
                    CombinedPrelimMidtermAverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MidtermPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MidtermExamWeighted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalMidtermGrade = table.Column<double>(type: "float", nullable: false),
                    TotalMidtermGradeRounded = table.Column<double>(type: "float", nullable: false),
                    GradePointEquivalent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MidtermGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MidtermGrades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MidtermGrades_StudentId",
                table: "MidtermGrades",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassStanding_MidtermGrades_MidtermGradeId",
                table: "ClassStanding",
                column: "MidtermGradeId",
                principalTable: "MidtermGrades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidtermQuizLists_MidtermGrades_MidtermGradeId",
                table: "MidtermQuizLists",
                column: "MidtermGradeId",
                principalTable: "MidtermGrades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassStanding_MidtermGrades_MidtermGradeId",
                table: "ClassStanding");

            migrationBuilder.DropForeignKey(
                name: "FK_MidtermQuizLists_MidtermGrades_MidtermGradeId",
                table: "MidtermQuizLists");

            migrationBuilder.DropTable(
                name: "MidtermGrades");

            migrationBuilder.AddColumn<int>(
                name: "AttendanceScore",
                table: "Users",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "ClassStandingTotalScore",
                table: "Users",
                type: "int",
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

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "GradePointEquivalent",
                table: "Users",
                type: "float",
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

            migrationBuilder.AddColumn<int>(
                name: "MidtermScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MidtermTotal",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OverallPrelimAndMidterm",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrelimScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrelimTotal",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProjectPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectScore",
                table: "Users",
                type: "int",
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

            migrationBuilder.AddColumn<int>(
                name: "RecitationScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SEPPG",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SEPScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SEPWeighted",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalMidtermGrade",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalMidtermGradeRounded",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuizScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalScorePerlimAndMidterm",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassStanding_Users_MidtermGradeId",
                table: "ClassStanding",
                column: "MidtermGradeId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidtermQuizLists_Users_MidtermGradeId",
                table: "MidtermQuizLists",
                column: "MidtermGradeId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
