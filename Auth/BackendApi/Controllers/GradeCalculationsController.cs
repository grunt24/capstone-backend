using BackendApi.Context;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    /// <summary>
    /// API controller for handling grade-related requests.
    /// This controller provides an endpoint to calculate and save a student's midterm grade.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GradeController : ControllerBase
    {
        private readonly IGradeCalculationService _gradeCalculationService;

        private readonly AppDbContext _dbContext;

        public GradeController(IGradeCalculationService gradeCalculationService, AppDbContext dbContext)
        {
            _gradeCalculationService = gradeCalculationService;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Calculates and saves a student's midterm grade.
        /// </summary>
        /// <param name="midtermGradeDto">The DTO containing all the data required for the grade calculation.</param>
        /// <returns>An action result with the calculated midterm grade if successful, or a bad request if the input is invalid.</returns>
        [HttpPost("calculate-midterm")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MidtermGradeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalculateMidtermGrade([FromBody] MidtermGradeDto midtermGradeDto)
        {
            if (!ModelState.IsValid)
            {
                // Return a bad request if the DTO is not valid.
                return BadRequest(ModelState);
            }

            try
            {
                // Call the service to perform the calculation and database save.
                var calculatedGrade = await _gradeCalculationService.CalculateAndSaveMidtermGradeAsync(midtermGradeDto);

                // You might want to map the result back to a DTO for the response.
                // For simplicity, we'll return the calculated model directly here.
                return Ok(calculatedGrade);
            }
            catch (InvalidOperationException ex)
            {
                // This handles the case where the grade weights are not found.
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                // A generic catch-all for any other unexpected errors.
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while calculating the grade.");
            }
        }
        [HttpPost("upload-midterm")]
        public async Task<IActionResult> UploadMidtermGrades(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = new MidtermGradeUploadResult();

            // Register code pages to handle various encodings.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Use ExcelDataReader to read the file into a DataSet
            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = false // We will handle the "header" rows manually
                }
            });

            if (dataSet.Tables.Count == 0)
            {
                return BadRequest("The uploaded file does not contain any data tables.");
            }

            var dataTable = dataSet.Tables[0];

            // Define constants for row and column indices (Excel is 1-based, C# is 0-based)
            const int quizTotalRowIndex = 11; // Row 12 in Excel
            const int classStandingTotalRowIndex = 11; // Row 12 in Excel
            const int studentDataStartingRowIndex = 13; // Row 14 in Excel
            const int nameColumnIndex = 1; // Column B
            const int recScoreColumnIndex = 13; // Column N
            const int attScoreColumnIndex = 14; // Column O

            // Updated column indices based on user request (AB14, AD14, AF14, AG14)
            const int sepScoreColumnIndex = 27; // Column AB
            const int projScoreColumnIndex = 29; // Column AD
            const int prelimScoreColumnIndex = 31; // Column AF
            const int midtermScoreColumnIndex = 32; // Column AG

            const int quizScoresStartingColumnIndex = 2; // Column C
            const int classStandingStartingColumnIndex = 15; // Column P

            // 1. Read the maximum scores from the designated header rows
            var quizTotals = new List<int?>();
            for (int i = 0; i < 8; i++) // Assuming 8 quizzes
            {
                var value = dataTable.Rows[quizTotalRowIndex][quizScoresStartingColumnIndex + i];
                quizTotals.Add(value is DBNull ? null : Convert.ToInt32(value));
            }

            var classStandingTotals = new List<int?>();
            for (int i = 0; i < 8; i++) // Assuming 8 class standing items
            {
                var value = dataTable.Rows[classStandingTotalRowIndex][classStandingStartingColumnIndex + i];
                classStandingTotals.Add(value is DBNull ? null : Convert.ToInt32(value));
            }

            // 2. Fetch all users from the database for efficient lookup
            var userLookup = await _dbContext.Users
                .Where(u => u.Fullname != null)
                .GroupBy(u => u.Fullname.Trim())
                .ToDictionaryAsync(g => g.Key, g => g.First().Id);

            // 3. Process student data rows
            for (int rowIndex = studentDataStartingRowIndex; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                var row = dataTable.Rows[rowIndex];

                // Check for valid student name in the row
                var studentNameObject = row[nameColumnIndex];
                var studentName = studentNameObject?.ToString()?.Trim();

                if (string.IsNullOrEmpty(studentName) || studentName.Contains("FEMALE:"))
                {
                    continue; // Skip empty rows or the 'FEMALE:' header
                }

                // Look up student ID
                if (!userLookup.TryGetValue(studentName, out var studentId))
                {
                    result.Warnings.Add($"Student with name '{studentName}' not found in the database. Skipping row.");
                    continue;
                }

                // Create the DTO with student data
                var studentDto = new MidtermGradeDto
                {
                    StudentId = studentId,
                    StudentFullName = studentName,
                    RecitationScore = row[recScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[recScoreColumnIndex]),
                    AttendanceScore = row[attScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[attScoreColumnIndex]),
                    SEPScore = row[sepScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[sepScoreColumnIndex]),
                    ProjectScore = row[projScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[projScoreColumnIndex]),
                    PrelimScore = row[prelimScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[prelimScoreColumnIndex]),
                    PrelimTotal = 100, // Assuming a fixed total of 100 for prelims
                    MidtermScore = row[midtermScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[midtermScoreColumnIndex]),
                    MidtermTotal = 100 // Assuming a fixed total of 100 for midterms
                };

                // Populate quizzes using the pre-read totals
                for (int i = 0; i < 8; i++)
                {
                    var quizScoreValue = row[quizScoresStartingColumnIndex + i];
                    if (quizScoreValue is not DBNull)
                    {
                        studentDto.Quizzes.Add(new MidtermQuizListDto
                        {
                            Id = i + 1,
                            Label = $"Quiz {i + 1}",
                            QuizScore = Convert.ToInt32(quizScoreValue),
                            TotalQuizScore = quizTotals[i]
                        });
                    }
                }

                // Populate class standing items using the pre-read totals
                for (int i = 0; i < 8; i++)
                {
                    var classStandingScoreValue = row[classStandingStartingColumnIndex + i];
                    if (classStandingScoreValue is not DBNull)
                    {
                        studentDto.ClassStandingItems.Add(new ClassStandingItemDto
                        {
                            Id = i + 1,
                            Label = $"SW/ASS/GRP WRK {i + 1}",
                            Score = Convert.ToInt32(classStandingScoreValue),
                            Total = classStandingTotals[i]
                        });
                    }
                }

                var midtermGradeResult = await _gradeCalculationService.CalculateAndSaveMidtermGradeAsync(studentDto);
                result.CalculatedGrades.Add(midtermGradeResult);
            }

            return Ok(result);
        }
    }
}
