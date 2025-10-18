using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class ayupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicPeriodId",
                table: "MidtermGrades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AcademicPeriodId = table.Column<int>(type: "int", nullable: false),
                    IsEnrolled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_AcademicPeriods_AcademicPeriodId",
                        column: x => x.AcademicPeriodId,
                        principalTable: "AcademicPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentEnrollments_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MidtermGrades_AcademicPeriodId",
                table: "MidtermGrades",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalsGrades_AcademicPeriodId",
                table: "FinalsGrades",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademicPeriodId",
                table: "StudentEnrollments",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId",
                table: "StudentEnrollments",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalsGrades_AcademicPeriods_AcademicPeriodId",
                table: "FinalsGrades",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidtermGrades_AcademicPeriods_AcademicPeriodId",
                table: "MidtermGrades",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalsGrades_AcademicPeriods_AcademicPeriodId",
                table: "FinalsGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_MidtermGrades_AcademicPeriods_AcademicPeriodId",
                table: "MidtermGrades");

            migrationBuilder.DropTable(
                name: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_MidtermGrades_AcademicPeriodId",
                table: "MidtermGrades");

            migrationBuilder.DropIndex(
                name: "IX_FinalsGrades_AcademicPeriodId",
                table: "FinalsGrades");

            migrationBuilder.DropColumn(
                name: "AcademicPeriodId",
                table: "MidtermGrades");

            migrationBuilder.DropColumn(
                name: "AcademicPeriodId",
                table: "FinalsGrades");
        }
    }
}
