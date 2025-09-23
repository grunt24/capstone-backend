using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Services;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    /// <summary>
    /// API controller for handling grade-related requests.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GradeCalculationController : ControllerBase
    {
        private readonly IGradeCalculationService _gradeCalculationService;
        private readonly AppDbContext _dbContext;

        public GradeCalculationController(IGradeCalculationService gradeCalculationService, AppDbContext dbContext)
        {
            _gradeCalculationService = gradeCalculationService;
            _dbContext = dbContext;
        }
        [HttpGet("equivalents")]
        public async Task<IActionResult> GetGradePointEquivalents()
        {
            var result = await _dbContext.GradePointEquivalents
                .OrderByDescending(g => g.MinPercentage) // show from highest %
                .ToListAsync();

            return Ok(new { success = true, data = result });
        }

        [HttpGet("grade-percentage")]
        public async Task<IActionResult> GetWeights()
        {
            var weights = await _gradeCalculationService.GetWeightsAsync();
            if (weights == null)
                return NotFound(new { success = false, message = "Grade weights not found" });

            return Ok(new { success = true, message = "Success", data = weights });
        }

        [HttpGet("students-midtermGrades")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _gradeCalculationService.GetMidtermGrades();
            return Ok(result);
        }
        [HttpGet("students-finalGrades")]
        public async Task<IActionResult> GetFinalGrades()
        {
            var result = await _gradeCalculationService.GetFinalGrades();
            return Ok(result);
        }

        [HttpPost("manual-insert")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MidtermGrade))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ManualInsertGrade([FromBody] MidtermGradeDto midtermGradeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var calculatedGrade = await _gradeCalculationService.CalculateAndSaveSingleMidtermGradeAsync(midtermGradeDto);
                return Ok(calculatedGrade);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = false
                }
            });

            if (dataSet.Tables.Count == 0)
            {
                return BadRequest("The uploaded file does not contain any data tables.");
            }

            var dataTable = dataSet.Tables[0];

            // Read SubjectCode from Row 8 (index 7) and look up SubjectId
            const int subjectCodeRowIndex = 7;
            const int subjectCodeColumnIndex = 0;

            var subjectCode = dataTable.Rows[subjectCodeRowIndex][subjectCodeColumnIndex]?.ToString()?.Split(':')[1].Trim();

            if (string.IsNullOrEmpty(subjectCode))
            {
                return BadRequest("Subject code not found in the uploaded file. Please make sure to include it!");
            }

            var subjectLookup = await _dbContext.Subjects
                .Where(s => s.SubjectCode == subjectCode)
                .ToDictionaryAsync(s => s.SubjectCode, s => s.Id);

            if (!subjectLookup.TryGetValue(subjectCode, out var subjectId))
            {
                return NotFound($"Subject with code '{subjectCode}' not found in the database.");
            }


            const int quizTotalRowIndex = 11;
            const int classStandingTotalRowIndex = 11;
            const int studentDataStartingRowIndex = 13;
            const int nameColumnIndex = 1;
            const int recScoreColumnIndex = 13;
            const int attScoreColumnIndex = 14;

            const int sepScoreColumnIndex = 27;
            const int projScoreColumnIndex = 29;
            const int prelimScoreColumnIndex = 31;
            const int midtermScoreColumnIndex = 32;

            const int quizScoresStartingColumnIndex = 2;
            const int classStandingStartingColumnIndex = 15;

            var quizTotals = new List<int?>();
            for (int i = 0; i < 8; i++)
            {
                var value = dataTable.Rows[quizTotalRowIndex][quizScoresStartingColumnIndex + i];
                quizTotals.Add(value is DBNull ? null : Convert.ToInt32(value));
            }

            var classStandingTotals = new List<int?>();
            for (int i = 0; i < 8; i++)
            {
                var value = dataTable.Rows[classStandingTotalRowIndex][classStandingStartingColumnIndex + i];
                classStandingTotals.Add(value is DBNull ? null : Convert.ToInt32(value));
            }

            var userLookup = await _dbContext.Users
                .Where(u => u.Fullname != null)
                .GroupBy(u => u.Fullname.Trim())
                .ToDictionaryAsync(g => g.Key, g => g.First().Id);

            for (int rowIndex = studentDataStartingRowIndex; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                var row = dataTable.Rows[rowIndex];

                var studentNameObject = row[nameColumnIndex];
                var studentName = studentNameObject?.ToString()?.Trim();

                if (string.IsNullOrEmpty(studentName) || studentName.Contains("FEMALE:"))
                {
                    continue;
                }

                if (!userLookup.TryGetValue(studentName, out var studentId))
                {
                    result.Warnings.Add($"Student with name '{studentName}' not found in the database. Skipping row.");
                    continue;
                }

                var studentDto = new MidtermGradeDto
                {
                    StudentId = studentId,
                    StudentFullName = studentName,
                    SubjectId = subjectId, // Added SubjectId to the DTO
                    RecitationScore = row[recScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[recScoreColumnIndex]),
                    AttendanceScore = row[attScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[attScoreColumnIndex]),
                    SEPScore = row[sepScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[sepScoreColumnIndex]),
                    ProjectScore = row[projScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[projScoreColumnIndex]),
                    PrelimScore = row[prelimScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[prelimScoreColumnIndex]),
                    PrelimTotal = 100,
                    MidtermScore = row[midtermScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[midtermScoreColumnIndex]),
                    MidtermTotal = 100
                };

                for (int i = 0; i < 8; i++)
                {
                    var quizScoreValue = row[quizScoresStartingColumnIndex + i];
                    if (quizScoreValue is not DBNull)
                    {
                        studentDto.Quizzes.Add(new QuizListDto
                        {
                            Id = i + 1,
                            Label = $"Quiz {i + 1}",
                            QuizScore = Convert.ToInt32(quizScoreValue),
                            TotalQuizScore = quizTotals[i]
                        });
                    }
                }

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

                var midtermGradeResult = await _gradeCalculationService.CalculateAndSaveSingleMidtermGradeAsync(studentDto);
                result.CalculatedGrades.Add(midtermGradeResult);
            }

            return Ok(result);
        }

        [HttpPost("upload-finals")]
        public async Task<IActionResult> UploadFinalsGrades(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = new FinalsGradeUploadResult();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = false
                }
            });

            if (dataSet.Tables.Count == 0)
            {
                return BadRequest("The uploaded file does not contain any data tables.");
            }

            var dataTable = dataSet.Tables[0];

            // Read SubjectCode from Row 8 (index 7) and look up SubjectId
            const int subjectCodeRowIndex = 7;
            const int subjectCodeColumnIndex = 0;

            var subjectCode = dataTable.Rows[subjectCodeRowIndex][subjectCodeColumnIndex]?.ToString()?.Split(':')[1].Trim();

            if (string.IsNullOrEmpty(subjectCode))
            {
                return BadRequest("Subject code not found in the uploaded file. Please make sure to include it!");
            }

            var subjectLookup = await _dbContext.Subjects
                .Where(s => s.SubjectCode == subjectCode)
                .ToDictionaryAsync(s => s.SubjectCode, s => s.Id);

            if (!subjectLookup.TryGetValue(subjectCode, out var subjectId))
            {
                return NotFound($"Subject with code '{subjectCode}' not found in the database.");
            }

            // Constants based on the Excel file structure
            const int quizTotalRowIndex = 11;
            const int classStandingTotalRowIndex = 11;
            const int studentDataStartingRowIndex = 13;
            const int nameColumnIndex = 1;
            const int recScoreColumnIndex = 13;
            const int attScoreColumnIndex = 14;

            const int sepScoreColumnIndex = 27;
            const int projScoreColumnIndex = 29;

            const int finalExamTotalRowIndex = 12;
            const int finalsScoreColumnIndex = 32;

            const int quizScoresStartingColumnIndex = 2;
            const int classStandingStartingColumnIndex = 15;
            const int finalExamTotalColumnIndex = 31;

            const int totalScoreFinalsRowIndex = 12;     // AG14 → row 14 → index 13
            const int overallFinalsRowIndex = 13;        // AG13 → row 13 → index 12
            const int agColumnIndex = 32;                // AG → column 33 → index 32

            var quizTotals = new List<int?>();
            for (int i = 0; i < 8; i++)
            {
                var value = dataTable.Rows[quizTotalRowIndex][quizScoresStartingColumnIndex + i];
                quizTotals.Add(value is DBNull ? null : Convert.ToInt32(value));
            }

            var classStandingTotals = new List<int?>();
            for (int i = 0; i < 8; i++)
            {
                var value = dataTable.Rows[classStandingTotalRowIndex][classStandingStartingColumnIndex + i];
                classStandingTotals.Add(value is DBNull ? null : Convert.ToInt32(value));
            }

            var totalScoreFinals = dataTable.Rows[totalScoreFinalsRowIndex][agColumnIndex] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[totalScoreFinalsRowIndex][agColumnIndex]);

            var overAllFInals = dataTable.Rows[overallFinalsRowIndex][agColumnIndex] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[overallFinalsRowIndex][agColumnIndex]);

            var finalExamTotal = dataTable.Rows[finalExamTotalRowIndex][finalExamTotalColumnIndex] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[finalExamTotalRowIndex][finalExamTotalColumnIndex]);

            var userLookup = await _dbContext.Users
                .Where(u => u.Fullname != null)
                .GroupBy(u => u.Fullname.Trim())
                .ToDictionaryAsync(g => g.Key, g => g.First().Id);

            var studentGradesToCalculate = new List<FinalsGradeDto>();

            for (int rowIndex = studentDataStartingRowIndex; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                var row = dataTable.Rows[rowIndex];
                var studentNameObject = row[nameColumnIndex];
                var studentName = studentNameObject?.ToString()?.Trim();

                if (string.IsNullOrEmpty(studentName) || studentName.Contains("FEMALE:"))
                {
                    continue;
                }

                if (!userLookup.TryGetValue(studentName, out var studentId))
                {
                    result.Warnings.Add("");
                    continue;
                }

                var studentDto = new FinalsGradeDto
                {
                    StudentId = studentId,
                    StudentFullName = studentName,
                    SubjectId = subjectId,
                    RecitationScore = row[recScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[recScoreColumnIndex]),
                    AttendanceScore = row[attScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[attScoreColumnIndex]),
                    SEPScore = row[sepScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[sepScoreColumnIndex]),
                    ProjectScore = row[projScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[projScoreColumnIndex]),
                    FinalsScore = row[finalsScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[finalsScoreColumnIndex]),
                    FinalsTotal = finalExamTotal,
                    TotalScoreFinals = totalScoreFinals,
                    OverallFinals = overAllFInals



                };

                for (int i = 0; i < 8; i++)
                {
                    var quizScoreValue = row[quizScoresStartingColumnIndex + i];
                    if (quizScoreValue is not DBNull)
                    {
                        studentDto.Quizzes.Add(new QuizListDto
                        {
                            Id = i + 1,
                            Label = $"Quiz {i + 1}",
                            QuizScore = Convert.ToInt32(quizScoreValue),
                            TotalQuizScore = quizTotals[i]
                        });
                    }
                }

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

                var midtermGradeResult = await _gradeCalculationService.CalculateAndSaveFinalGradesAsync(studentDto);
                result.CalculatedGrades.Add(midtermGradeResult);
            }


            return Ok(result);
        }

        [HttpDelete("delete-midtermGrades")]
        public async Task<ActionResult<ResponseData<string>>> DeleteMidtermGrades([FromBody] List<int> gradeIds)
        {
            if (gradeIds == null || gradeIds.Count == 0)
            {
                return BadRequest(new ResponseData<string>
                {
                    Success = false,
                    Message = "No grade IDs provided.",
                    Data = null
                });
            }

            var result = await _gradeCalculationService.DeleteMidtermGradesAsync(gradeIds);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        [HttpDelete("delete-finalGrades")]
        public async Task<IActionResult> DeleteFinalsGradesAsync([FromBody] List<int> gradeIds)
        {
            var result = await _gradeCalculationService.DeleteFinalsGradesAsync(gradeIds);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}