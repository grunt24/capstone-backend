using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using BackendApi.Repositories;
using BackendApi.Services;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Globalization;
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
        private readonly IAuthRepository _authRepo;

        public GradeCalculationController(IGradeCalculationService gradeCalculationService, AppDbContext dbContext, IAuthRepository authRepo)
        {
            _gradeCalculationService = gradeCalculationService;
            _dbContext = dbContext;
            _authRepo = authRepo;
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

            // Read Semester and Academic Year from Row 4 (index 3) and Column 1 (index 0)
            const int semesterAndAYRowIndex = 3;
            const int semesterAndAYColumnIndex = 0;
            var semesterAndAYText = dataTable.Rows[semesterAndAYRowIndex][semesterAndAYColumnIndex]?.ToString();

            string? semester = null;
            string? academicYear = null;

            if (!string.IsNullOrEmpty(semesterAndAYText))
            {
                var parts = semesterAndAYText.Split(',');
                if (parts.Length >= 2)
                {
                    semester = parts[0].Trim();
                    academicYear = parts[1].Trim();

                    // ✅ Normalize semester text
                    if (semester.Contains("1st", StringComparison.OrdinalIgnoreCase))
                        semester = "First";
                    else if (semester.Contains("2nd", StringComparison.OrdinalIgnoreCase))
                        semester = "Second";
                    else
                        semester = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(semester.ToLower());
                }
            }
            else
            {
                return BadRequest("Semester and Academic Year not found in the uploaded file.");
            }

            //added
            var academicPeriod = await _dbContext.AcademicPeriods
                .SingleOrDefaultAsync(ap => ap.IsCurrent);

            if (academicPeriod == null)
            {
                return BadRequest("No current academic period is set. Please configure one in the system before uploading grades.");
            }

            // ✅ Use the current academic period's values
            var academicYearId = academicPeriod.Id;
            //var academicYear = $"{academicPeriod.StartYear}-{academicPeriod.EndYear}";
            //var semester = academicPeriod.Semester;




            //if (academicPeriod == null)
            //{
            //    return BadRequest($"Academic period '{academicYear} - {semester}' not found, and no current academic period is set.");
            //}

            //var academicYearId = academicPeriod.Id;


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

            var currentUser = await _authRepo.GetCurrentUserAsync();

            if (currentUser.Role != UserRole.Teacher)
            {
                return Forbid("Only teachers are allowed to upload grades.");
            }

            // ✅ Get subject including the assigned teacher
            var subject = await _dbContext.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);

            if (subject == null)
            {
                return NotFound($"Subject with code '{subjectCode}' not found in the database.");
            }

            // ✅ Get teacher record of current user
            var teacher = await _dbContext.Teachers
                .FirstOrDefaultAsync(t => t.UserID == currentUser.Id);

            if (teacher == null)
            {
                return Forbid("You are not registered as a teacher.");
            }

            // ✅ Check if the current teacher is assigned to this subject
            if (subject.TeacherId != teacher.Id)
            {
                return Unauthorized(new { message = $"You are not assigned to the subject '{subject.SubjectName}'. Only the assigned teacher can upload grades for this subject." });

            }

            // ✅ Extract subjectId for further use
            var subjectId = subject.Id;


            // Read PrelimTotal and MidtermTotal from Row 13 (index 12)
            const int totalScoresRowIndex = 12;
            const int prelimTotalColumnIndex = 31; // Column AF
            const int midtermTotalColumnIndex = 32; // Column AG

            var prelimTotal = dataTable.Rows[totalScoresRowIndex][prelimTotalColumnIndex] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[totalScoresRowIndex][prelimTotalColumnIndex]);
            var midtermTotal = dataTable.Rows[totalScoresRowIndex][midtermTotalColumnIndex] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[totalScoresRowIndex][midtermTotalColumnIndex]);


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

                var studentNumber = await _dbContext.Users
    .Where(u => u.Id == studentId)
    .Select(u => u.StudentNumber)
    .FirstOrDefaultAsync();

                var studentDto = new MidtermGradeDto
                {
                    StudentId = studentId,
                    StudentFullName = studentName,
                    SubjectId = subjectId, // Added SubjectId to the DTO
                    StudentNumber = studentNumber,
                    Semester = semester,
                    AcademicYear = academicYear,
                    RecitationScore = row[recScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[recScoreColumnIndex]),
                    AttendanceScore = row[attScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[attScoreColumnIndex]),
                    SEPScore = row[sepScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[sepScoreColumnIndex]),
                    ProjectScore = row[projScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[projScoreColumnIndex]),
                    PrelimScore = row[prelimScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[prelimScoreColumnIndex]),
                    PrelimTotal = prelimTotal,
                    MidtermScore = row[midtermScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[midtermScoreColumnIndex]),
                    MidtermTotal = midtermTotal,
                    AcademicPeriodId = academicYearId,
                };

                // ✅ Check if midterm grade for this student already exists
                bool alreadyExists = await _dbContext.MidtermGrades.AnyAsync(g =>
                    g.StudentId == studentId &&
                    g.SubjectId == subjectId &&
                    g.AcademicYear == academicYear &&
                    g.Semester == semester
                );

                if (alreadyExists)
                {
                    result.Warnings.Add(
                        $"Grade already uploaded for {studentName} ({academicYear}, {semester}) in subject {subjectCode}. Skipping row."
                    );
                    continue; // skip to next student
                }


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

            // Read Semester and Academic Year from Row 4 (index 3) and Column 1 (index 0)
            const int semesterAndAYRowIndex = 3;
            const int semesterAndAYColumnIndex = 0;
            var semesterAndAYText = dataTable.Rows[semesterAndAYRowIndex][semesterAndAYColumnIndex]?.ToString();

            string? semester = null;
            string? academicYear = null;

            if (!string.IsNullOrEmpty(semesterAndAYText))
            {
                var parts = semesterAndAYText.Split(',');
                if (parts.Length >= 2)
                {
                    semester = parts[0].Trim();
                    academicYear = parts[1].Trim();

                    // ✅ Normalize semester text
                    if (semester.Contains("1st", StringComparison.OrdinalIgnoreCase))
                        semester = "First";
                    else if (semester.Contains("2nd", StringComparison.OrdinalIgnoreCase))
                        semester = "Second";
                    else
                        semester = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(semester.ToLower());
                }
            }
            else
            {
                return BadRequest("Semester and Academic Year not found in the uploaded file.");
            }


            //added
            // ✅ Remove any "AY" prefix before splitting
            var cleanedAcademicYear = academicYear.Replace("AY", "", StringComparison.OrdinalIgnoreCase).Trim();

            var academicYearParts = cleanedAcademicYear.Split('-');
            if (academicYearParts.Length != 2
                || !int.TryParse(academicYearParts[0].Trim(), out int startYear)
                || !int.TryParse(academicYearParts[1].Trim(), out int endYear))
            {
                return BadRequest($"Invalid academic year format '{academicYear}'. Expected format 'YYYY-YYYY' or 'AY YYYY-YYYY'.");
            }



            var academicPeriod = await _dbContext.AcademicPeriods
    .FirstOrDefaultAsync(ap =>
        ap.StartYear == startYear &&
        ap.EndYear == endYear &&
        ap.Semester.Equals(semester, StringComparison.OrdinalIgnoreCase));

            if (academicPeriod == null)
            {
                return BadRequest($"Academic period '{academicYear} - {semester}' not found in the database.");
            }

            var academicYearId = academicPeriod.Id;

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

            // ✅ Get the current logged-in user
            var currentUser = await _authRepo.GetCurrentUserAsync();

            if (currentUser.Role != UserRole.Teacher)
            {
                return Forbid("Only teachers are allowed to upload grades.");
            }

            // ✅ Get subject including the assigned teacher
            var subject = await _dbContext.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);

            if (subject == null)
            {
                return NotFound($"Subject with code '{subjectCode}' not found in the database.");
            }

            // ✅ Get teacher record of current user
            var teacher = await _dbContext.Teachers
                .FirstOrDefaultAsync(t => t.UserID == currentUser.Id);

            if (teacher == null)
            {
                return Forbid("You are not registered as a teacher.");
            }

            // ✅ Check if the current teacher is assigned to this subject
            if (subject.TeacherId != teacher.Id)
            {
                return Unauthorized(new { message = $"{currentUser.Fullname} is not assigned to the subject '{subject.SubjectName}'. Only the assigned teacher can upload grades for this subject." });

            }

            // ✅ Extract subjectId for further use
            var subjectId = subject.Id;


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
                    //result.Warnings.Add("");
                    continue;
                }

                var studentNumber = await _dbContext.Users
.Where(u => u.Id == studentId)
.Select(u => u.StudentNumber)
.FirstOrDefaultAsync();

                var studentDto = new FinalsGradeDto
                {
                    StudentId = studentId,
                    StudentFullName = studentName,
                    SubjectId = subjectId,
                    StudentNumber = studentName,
                    Semester = semester,
                    AcademicYear = academicYear,
                    RecitationScore = row[recScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[recScoreColumnIndex]),
                    AttendanceScore = row[attScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[attScoreColumnIndex]),
                    SEPScore = row[sepScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[sepScoreColumnIndex]),
                    ProjectScore = row[projScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[projScoreColumnIndex]),
                    FinalsScore = row[finalsScoreColumnIndex] is DBNull ? 0 : Convert.ToInt32(row[finalsScoreColumnIndex]),
                    FinalsTotal = finalExamTotal,
                    TotalScoreFinals = totalScoreFinals,
                    OverallFinals = overAllFInals,
                    AcademicYearId = academicYearId,
                };

                bool alreadyExists = await _dbContext.FinalsGrades.AnyAsync(g =>
    g.StudentId == studentId &&
    g.SubjectId == subjectId &&
    g.AcademicYear == academicYear &&
    g.Semester == semester
);

                if (alreadyExists)
                {
                    result.Warnings.Add(
                        $"Grade already uploaded for {studentName} ({academicYear}, {semester}) in subject {subjectCode}. Skipping row."
                    );
                    continue; // skip to next student
                }

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

        [HttpGet("students-grades-by-ay-semester")]
        public async Task<IActionResult> GetStudentsGradesByAcademicYearAndSemester([FromQuery] string academicYear, [FromQuery] string semester)
        {
            if (string.IsNullOrEmpty(academicYear) || string.IsNullOrEmpty(semester))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Academic year and semester are required."
                });
            }

            var currentUser = await _authRepo.GetCurrentUserAsync();

            // Base queries
            var midtermQuery = _dbContext.MidtermGrades
                .Include(g => g.Subject)
                .Include(g => g.User)
                .Include(g => g.Quizzes)
                .Where(g => g.AcademicYear == academicYear && g.Semester == semester);

            var finalsQuery = _dbContext.FinalsGrades
                .Include(g => g.Subject)
                .Include(g => g.User)
                .Include(g => g.Quizzes)
                .Where(g => g.AcademicYear == academicYear && g.Semester == semester);

            // Filter based on role
            if (currentUser.Role == UserRole.Student)
            {
                midtermQuery = midtermQuery.Where(g => g.StudentId == currentUser.Id);
                finalsQuery = finalsQuery.Where(g => g.StudentId == currentUser.Id);
            }
            else if (currentUser.Role == UserRole.Teacher)
            {
                midtermQuery = midtermQuery.Where(g => g.Subject.TeacherId == currentUser.Id);
                finalsQuery = finalsQuery.Where(g => g.Subject.TeacherId == currentUser.Id);
            }
            // Admin/Superadmin = no filtering

            var midtermGrades = await midtermQuery
                .Select(g => new
                { g.StudentId, Fullname = g.User.Fullname, 
                    g.SubjectId, g.Subject.SubjectName, 
                    Department = g.User.Department, 
                    YearLevel = g.User.YearLevel,
                    g.Subject.SubjectCode, 
                    g.AcademicYear, g.Semester, 
                    g.QuizPG, g.RecitationScore, 
                    g.AttendanceScore, g.ClassStandingPG,
                    g.ProjectScore, g.SEPScore, 
                    g.PrelimScore, g.PrelimTotal, 
                    g.MidtermScore, g.MidtermTotal, 
                    g.CombinedPrelimMidtermAverage, 
                    g.TotalMidtermGradeRounded, g.AcademicPeriodId,
                    Quizzes = g.Quizzes.Select(q => new { q.Id, q.Label, q.QuizScore, q.TotalQuizScore }).ToList(),
                    ClassStandingItems = g.ClassStandingItems.Select(c => new {c.Id, c.Label, c.Score, c.Total}).ToList()
                })
                .ToListAsync();


            var finalGrades = await finalsQuery
                .Select(g => new
                { g.StudentId, Fullname = g.User.Fullname, 
                    g.SubjectId, g.Subject.SubjectName, 
                    g.Subject.SubjectCode,
                    g.AcademicYear, g.Semester, 
                    g.QuizPG, g.RecitationScore, 
                    g.AttendanceScore, g.ClassStandingPG, 
                    g.ProjectScore, g.SEPScore, 
                    g.FinalsScore, g.FinalsTotal, 
                    g.CombinedFinalsAverage, 
                    g.TotalFinalsGradeRounded, 
                    Quizzes = g.Quizzes.Select(q => new { q.Id, q.Label, q.QuizScore, q.TotalQuizScore }).ToList(),
                    ClassStandingItems = g.ClassStandingItems.Select(c => new { c.Id, c.Label, c.Score, c.Total }).ToList()
                }).ToListAsync();

            // Merge grades (for all roles)
            var mergedGrades = midtermGrades
                .Select(m => new
                {
                    studentId = m.StudentId,
                    studentFullName = m.Fullname,
                    department = m.Department,
                    yearLevel = m.YearLevel,
                    AcademicPeriodId = m.AcademicPeriodId,
                    subjectId = m.SubjectId,
                    subjectName = m.SubjectName,
                    subjectCode = m.SubjectCode,
                    academicYear = m.AcademicYear,
                    semester = m.Semester,
                    midtermGrade = m,
                    finalGrade = finalGrades,
                })
                .ToList();

            // If user is student → return only their grades (midterm + final)
            if (currentUser.Role == UserRole.Student)
            {
                return Ok(new
                {
                    success = true,
                    message = "Your grades retrieved successfully.",
                    data = mergedGrades // already filtered above
                });
            }

            // If teacher/admin → return all merged data
            return Ok(new
            {
                success = true,
                message = "Student grades retrieved successfully.",
                data = mergedGrades
            });
        }



        [HttpGet("grades-count")]
        public async Task<IActionResult> GetGradesCount()
        {
            var midtermGrades = await _dbContext.MidtermGrades
                .Select(g => new
                {
                    g.Semester,
                    g.AcademicYear,
                    SortKey = GenerateSortKey(g.AcademicYear, g.Semester)
                })
                .ToListAsync();

            var finalGrades = await _dbContext.FinalsGrades
                .Select(g => new
                {
                    g.Semester,
                    g.AcademicYear,
                    SortKey = GenerateSortKey(g.AcademicYear, g.Semester)
                })
                .ToListAsync();

            var allGrades = midtermGrades
                .Concat(finalGrades)
                .Distinct()
                .OrderByDescending(g => g.SortKey)
                .ToList();

            var latest = allGrades.FirstOrDefault();

            // 🆕 Full filters like "2024-2025 First Semester"
            var yearSemesterFilters = allGrades
                .Where(g => !string.IsNullOrEmpty(g.AcademicYear) && !string.IsNullOrEmpty(g.Semester))
                .Select(g => $"{g.AcademicYear.Replace("AY ", "").Trim()} {g.Semester}")
                .Distinct()
                .OrderByDescending(g => g)
                .ToList();

            // ✅ Only count midterm and final grades with GradePointEquivalent > 0
            var midtermCount = await _dbContext.MidtermGrades
                .CountAsync(g => g.TotalMidtermGradeRounded > 0);

            var finalCount = await _dbContext.FinalsGrades
                .CountAsync(g => g.TotalFinalsGradeRounded > 0);

            return Ok(new
            {
                success = true,
                message = "Grades count retrieved successfully",
                data = new
                {
                    midtermCount,
                    finalCount,
                    currentSemester = latest?.Semester ?? "Not Available",
                    currentAcademicYear = latest?.AcademicYear ?? "Not Available",
                    academicYearSemesterFilters = yearSemesterFilters // 🆕 for frontend dropdown
                }
            });
        }




        private static int GenerateSortKey(string academicYear, string semester)
        {
            int year = ExtractStartYear(academicYear);
            int semOrder = NormalizeSemester(semester);
            return (year * 10) + semOrder;
        }

        private static int ExtractStartYear(string academicYear)
        {
            if (string.IsNullOrWhiteSpace(academicYear)) return 0;
            var parts = academicYear.Split('-');
            return int.TryParse(parts[0], out int year) ? year : 0;
        }

        private static int NormalizeSemester(string semester)
        {
            return semester switch
            {
                "1st Semester" => 1,
                "2nd Semester" => 2,
                "Summer" => 3,
                _ => 0
            };
        }

        // Midterm - calculate for all subjects in current academic period
        [HttpPost("calculate-midterm-all")]
        public async Task<IActionResult> CalculateMidtermAllAsync()
        {
            var result = await _gradeCalculationService.CalculateMidtermGradesForAllSubjectsAsync();
            return Ok(result);
        }

        // Finals - calculate for all subjects in current academic period
        [HttpPost("calculate-finals-all")]
        public async Task<IActionResult> CalculateFinalsAllAsync()
        {
            var result = await _gradeCalculationService.CalculateFinalsGradesForAllSubjectsAsync();
            return Ok(result);
        }




        // GET: api/midterm-grades/{subjectId}/{academicPeriodId}
        [HttpGet("{subjectId:int}/{academicPeriodId:int}")]
        public async Task<IActionResult> GetGradesBySubjectAndPeriod(int subjectId, int academicPeriodId)
        {
            var grades = await _gradeCalculationService.GetGradesBySubjectAndPeriodAsync(subjectId, academicPeriodId);
            return Ok(grades);
        }

        // PUT: api/midterm-grades/{studentId}
        [HttpPut("{studentId:int}")]
        public async Task<IActionResult> UpdateMidtermGrade(int studentId, [FromBody] MidtermGradeDto updatedGrade)
        {
            if (updatedGrade == null)
                return BadRequest("Invalid grade data.");

            var result = await _gradeCalculationService.UpdateMidtermGradeAsync(studentId, updatedGrade);

            if (!result)
                return NotFound($"Student ID {studentId} not found or update failed.");

            return Ok(new { message = "Midterm grade updated successfully." });
        }



    }
}