using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTBL",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTBL", x => x.CourseID);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentTBL",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTBL", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "SubjectTBL",
                columns: table => new
                {
                    SubjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTBL", x => x.SubjectID);
                });

            migrationBuilder.CreateTable(
                name: "StudentTBL",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YearLevel = table.Column<int>(type: "int", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTBL", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_StudentTBL_CourseTBL_CourseID",
                        column: x => x.CourseID,
                        principalTable: "CourseTBL",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTBL_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherTBL",
                columns: table => new
                {
                    TeacherID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherTBL", x => x.TeacherID);
                    table.ForeignKey(
                        name: "FK_TeacherTBL_DepartmentTBL_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "DepartmentTBL",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherTBL_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceTBL",
                columns: table => new
                {
                    AttendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceTBL", x => x.AttendanceID);
                    table.ForeignKey(
                        name: "FK_AttendanceTBL_StudentTBL_StudentID",
                        column: x => x.StudentID,
                        principalTable: "StudentTBL",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceTBL_SubjectTBL_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "SubjectTBL",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentTBL",
                columns: table => new
                {
                    EnrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SchoolYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentTBL", x => x.EnrollmentID);
                    table.ForeignKey(
                        name: "FK_EnrollmentTBL_StudentTBL_StudentID",
                        column: x => x.StudentID,
                        principalTable: "StudentTBL",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrollmentTBL_SubjectTBL_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "SubjectTBL",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentsTBL",
                columns: table => new
                {
                    AssessmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    TeacherID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentsTBL", x => x.AssessmentID);
                    table.ForeignKey(
                        name: "FK_AssessmentsTBL_SubjectTBL_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "SubjectTBL",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentsTBL_TeacherTBL_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "TeacherTBL",
                        principalColumn: "TeacherID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjectsMapping",
                columns: table => new
                {
                    TeacherSubjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherID = table.Column<int>(type: "int", nullable: false),
                    SubjectID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjectsMapping", x => x.TeacherSubjectID);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectsMapping_SubjectTBL_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "SubjectTBL",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectsMapping_TeacherTBL_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "TeacherTBL",
                        principalColumn: "TeacherID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreTBL",
                columns: table => new
                {
                    ScoreID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    AssessmentID = table.Column<int>(type: "int", nullable: false),
                    ScoreValue = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreTBL", x => x.ScoreID);
                    table.ForeignKey(
                        name: "FK_ScoreTBL_AssessmentsTBL_AssessmentID",
                        column: x => x.AssessmentID,
                        principalTable: "AssessmentsTBL",
                        principalColumn: "AssessmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreTBL_StudentTBL_StudentID",
                        column: x => x.StudentID,
                        principalTable: "StudentTBL",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentsTBL_SubjectID",
                table: "AssessmentsTBL",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentsTBL_TeacherID",
                table: "AssessmentsTBL",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTBL_StudentID",
                table: "AttendanceTBL",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTBL_SubjectID",
                table: "AttendanceTBL",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentTBL_StudentID",
                table: "EnrollmentTBL",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentTBL_SubjectID",
                table: "EnrollmentTBL",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreTBL_AssessmentID",
                table: "ScoreTBL",
                column: "AssessmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreTBL_StudentID",
                table: "ScoreTBL",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTBL_CourseID",
                table: "StudentTBL",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTBL_UserID",
                table: "StudentTBL",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectsMapping_SubjectID",
                table: "TeacherSubjectsMapping",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectsMapping_TeacherID",
                table: "TeacherSubjectsMapping",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTBL_DepartmentID",
                table: "TeacherTBL",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTBL_UserID",
                table: "TeacherTBL",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceTBL");

            migrationBuilder.DropTable(
                name: "EnrollmentTBL");

            migrationBuilder.DropTable(
                name: "ScoreTBL");

            migrationBuilder.DropTable(
                name: "TeacherSubjectsMapping");

            migrationBuilder.DropTable(
                name: "AssessmentsTBL");

            migrationBuilder.DropTable(
                name: "StudentTBL");

            migrationBuilder.DropTable(
                name: "SubjectTBL");

            migrationBuilder.DropTable(
                name: "TeacherTBL");

            migrationBuilder.DropTable(
                name: "CourseTBL");

            migrationBuilder.DropTable(
                name: "DepartmentTBL");
        }
    }
}
