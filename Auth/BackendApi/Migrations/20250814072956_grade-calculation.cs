using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class gradecalculation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Average",
                table: "Users",
                type: "int",
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

            migrationBuilder.AddColumn<int>(
                name: "MidtermGradeScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MidtermGradeTotalSCore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PercentageOfMidtermAndPrelim",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrelimGrade",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrelimGradeTotalScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectScore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectScorePercentage",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SEPPercentage",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SEPSCore",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalMidtermGrade",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalScorePerlimAndMidterm",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassStanding",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Recitation = table.Column<int>(type: "int", nullable: false),
                    Attendance = table.Column<int>(type: "int", nullable: false),
                    PG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Average = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassStandingPercentage = table.Column<int>(type: "int", nullable: false),
                    MidtermGradeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassStanding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassStanding_Users_MidtermGradeId",
                        column: x => x.MidtermGradeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GradePointEquivalents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinPercentage = table.Column<double>(type: "float", nullable: true),
                    MaxPercentage = table.Column<double>(type: "float", nullable: false),
                    GradePoint = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradePointEquivalents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MidtermQuizLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuizScore = table.Column<int>(type: "int", nullable: false),
                    TotalQuizScore = table.Column<int>(type: "int", nullable: false),
                    PG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuizPercentage = table.Column<int>(type: "int", nullable: false),
                    MidtermGradeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MidtermQuizLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MidtermQuizLists_Users_MidtermGradeId",
                        column: x => x.MidtermGradeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassStanding_MidtermGradeId",
                table: "ClassStanding",
                column: "MidtermGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_MidtermQuizLists_MidtermGradeId",
                table: "MidtermQuizLists",
                column: "MidtermGradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassStanding");

            migrationBuilder.DropTable(
                name: "GradePointEquivalents");

            migrationBuilder.DropTable(
                name: "MidtermQuizLists");

            migrationBuilder.DropColumn(
                name: "Average",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GradePointEquivalent",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermGradeScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MidtermGradeTotalSCore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PercentageOfMidtermAndPrelim",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrelimGrade",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrelimGradeTotalScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProjectScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProjectScorePercentage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPPercentage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SEPSCore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalMidtermGrade",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalScorePerlimAndMidterm",
                table: "Users");
        }
    }
}
