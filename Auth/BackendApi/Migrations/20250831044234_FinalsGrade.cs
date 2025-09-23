using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class FinalsGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MidtermGrades_Subjects_SubjectId",
                table: "MidtermGrades");

            migrationBuilder.DropTable(
                name: "MidtermQuizLists");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "MidtermGrades",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FinalsGradeId",
                table: "ClassStanding",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinalsGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    TotalQuizScore = table.Column<int>(type: "int", nullable: false),
                    QuizPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuizWeightedTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecitationScore = table.Column<int>(type: "int", nullable: false),
                    AttendanceScore = table.Column<int>(type: "int", nullable: false),
                    ClassStandingTotalScore = table.Column<int>(type: "int", nullable: false),
                    ClassStandingAverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassStandingPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassStandingWeightedTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SEPScore = table.Column<int>(type: "int", nullable: false),
                    SEPPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SEPWeightedTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectScore = table.Column<int>(type: "int", nullable: false),
                    ProjectPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectWeightedTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalsScore = table.Column<int>(type: "int", nullable: false),
                    FinalsTotal = table.Column<int>(type: "int", nullable: false),
                    TotalScoreFinals = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverallFinals = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CombinedFinalsAverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalsPG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalsWeightedTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFinalsGrade = table.Column<double>(type: "float", nullable: false),
                    TotalFinalsGradeRounded = table.Column<double>(type: "float", nullable: false),
                    GradePointEquivalent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalsGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalsGrades_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FinalsGrades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuizScore = table.Column<int>(type: "int", nullable: true),
                    TotalQuizScore = table.Column<int>(type: "int", nullable: true),
                    FinalsGradeId = table.Column<int>(type: "int", nullable: true),
                    MidtermGradeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizLists_FinalsGrades_FinalsGradeId",
                        column: x => x.FinalsGradeId,
                        principalTable: "FinalsGrades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizLists_MidtermGrades_MidtermGradeId",
                        column: x => x.MidtermGradeId,
                        principalTable: "MidtermGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassStanding_FinalsGradeId",
                table: "ClassStanding",
                column: "FinalsGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalsGrades_StudentId",
                table: "FinalsGrades",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalsGrades_SubjectId",
                table: "FinalsGrades",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLists_FinalsGradeId",
                table: "QuizLists",
                column: "FinalsGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLists_MidtermGradeId",
                table: "QuizLists",
                column: "MidtermGradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassStanding_FinalsGrades_FinalsGradeId",
                table: "ClassStanding",
                column: "FinalsGradeId",
                principalTable: "FinalsGrades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidtermGrades_Subjects_SubjectId",
                table: "MidtermGrades",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassStanding_FinalsGrades_FinalsGradeId",
                table: "ClassStanding");

            migrationBuilder.DropForeignKey(
                name: "FK_MidtermGrades_Subjects_SubjectId",
                table: "MidtermGrades");

            migrationBuilder.DropTable(
                name: "QuizLists");

            migrationBuilder.DropTable(
                name: "FinalsGrades");

            migrationBuilder.DropIndex(
                name: "IX_ClassStanding_FinalsGradeId",
                table: "ClassStanding");

            migrationBuilder.DropColumn(
                name: "FinalsGradeId",
                table: "ClassStanding");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "MidtermGrades",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MidtermQuizLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MidtermGradeId = table.Column<int>(type: "int", nullable: true),
                    QuizScore = table.Column<int>(type: "int", nullable: true),
                    TotalQuizScore = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MidtermQuizLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MidtermQuizLists_MidtermGrades_MidtermGradeId",
                        column: x => x.MidtermGradeId,
                        principalTable: "MidtermGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MidtermQuizLists_MidtermGradeId",
                table: "MidtermQuizLists",
                column: "MidtermGradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MidtermGrades_Subjects_SubjectId",
                table: "MidtermGrades",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
