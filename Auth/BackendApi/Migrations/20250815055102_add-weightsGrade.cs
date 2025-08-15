using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class addweightsGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalScorePerlimAndMidterm",
                table: "MidtermGrades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "OverallPrelimAndMidterm",
                table: "MidtermGrades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "GradeWeights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quizzes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassStanding = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SEP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Project = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Prelim = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MidtermExam = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeWeights", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GradeWeights");

            migrationBuilder.AlterColumn<double>(
                name: "TotalScorePerlimAndMidterm",
                table: "MidtermGrades",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "OverallPrelimAndMidterm",
                table: "MidtermGrades",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
