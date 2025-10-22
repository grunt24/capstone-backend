using AutoMapper;
using BackendApi.Context;
using BackendApi.Core.General;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GradeCalculationService : IGradeCalculationService
{
    private readonly AppDbContext _context;
    IStudentRepository _studentRepository;
    IAuthRepository _authRepository;
    IMapper _mapper;

    public GradeCalculationService(AppDbContext context, IStudentRepository studentRepository, IMapper mapper, IAuthRepository authRepository)
    {
        _context = context;
        _studentRepository = studentRepository;
        _mapper = mapper;
        _authRepository = authRepository;
    }



    //Grade Weights
    public async Task<GradeWeights?> GetWeightsAsync()
    {
        // Assuming only 1 row exists (Id = 2 in your example)
        return await _context.GradeWeights.FirstOrDefaultAsync();
    }

    public async Task<MidtermGrade> CalculateAndSaveSingleMidtermGradeAsync(MidtermGradeDto studentGradeDto)
    {
        // Fetch grade scale and weights from the database
        var _gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var _weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (_weights == null)
        {
            throw new InvalidOperationException("Grade weights not found in the database.");
        }

        // Map DTO to Model
        var studentGrade = new MidtermGrade
        {
            StudentId = studentGradeDto.StudentId,
            SubjectId = studentGradeDto.SubjectId,
            Semester = studentGradeDto.Semester,
            AcademicYear = studentGradeDto.AcademicYear,
            Quizzes = studentGradeDto.Quizzes.Select(q => new QuizList { Label = q.Label, QuizScore = q.QuizScore, TotalQuizScore = q.TotalQuizScore }).ToList(),
            RecitationScore = studentGradeDto.RecitationScore,
            AttendanceScore = studentGradeDto.AttendanceScore,
            ClassStandingItems = studentGradeDto.ClassStandingItems.Select(cs => new ClassStandingItem { Label = cs.Label, Score = cs.Score, Total = cs.Total }).ToList(),
            SEPScore = studentGradeDto.SEPScore,
            ProjectScore = studentGradeDto.ProjectScore,
            PrelimScore = studentGradeDto.PrelimScore,
            PrelimTotal = studentGradeDto.PrelimTotal,
            MidtermScore = studentGradeDto.MidtermScore,
            MidtermTotal = studentGradeDto.MidtermTotal,
            AcademicPeriodId = studentGradeDto.AcademicPeriodId,
        };

        // Call a private helper method to perform the calculation
        _calculateMidtermGrade(studentGrade, _weights, _gradeScale);

        // Save to DB
        _context.MidtermGrades.Add(studentGrade);
        await _context.SaveChangesAsync();

        return studentGrade;
    }

    public async Task<MidtermGrade> CalculateAndSaveSingleMidtermGradeAsyncV2(MidtermGradeDto studentGradeDto)
    {
        var _gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var _weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (_weights == null)
            throw new InvalidOperationException("Grade weights not found in the database.");

        // Try to get existing grade entry
        var existingGrade = await _context.MidtermGrades
            .Include(g => g.Quizzes)
            .Include(g => g.ClassStandingItems)
            .FirstOrDefaultAsync(g =>
                g.StudentId == studentGradeDto.StudentId &&
                g.SubjectId == studentGradeDto.SubjectId &&
                g.AcademicPeriodId == studentGradeDto.AcademicPeriodId);

        if (existingGrade == null)
        {
            // Create new grade record
            existingGrade = new MidtermGrade
            {
                StudentId = studentGradeDto.StudentId,
                SubjectId = studentGradeDto.SubjectId,
                Semester = studentGradeDto.Semester,
                AcademicYear = studentGradeDto.AcademicYear,
                AcademicPeriodId = studentGradeDto.AcademicPeriodId,
                Quizzes = studentGradeDto.Quizzes.Select(q => new QuizList
                {
                    Label = q.Label,
                    QuizScore = q.QuizScore,
                    TotalQuizScore = q.TotalQuizScore
                }).ToList(),
                ClassStandingItems = studentGradeDto.ClassStandingItems.Select(cs => new ClassStandingItem
                {
                    Label = cs.Label,
                    Score = cs.Score,
                    Total = cs.Total
                }).ToList()
            };

            _context.MidtermGrades.Add(existingGrade);
        }
        else
        {
            // Update basic fields
            existingGrade.RecitationScore = studentGradeDto.RecitationScore;
            existingGrade.AttendanceScore = studentGradeDto.AttendanceScore;
            existingGrade.SEPScore = studentGradeDto.SEPScore;
            existingGrade.ProjectScore = studentGradeDto.ProjectScore;
            existingGrade.PrelimScore = studentGradeDto.PrelimScore;
            existingGrade.PrelimTotal = studentGradeDto.PrelimTotal;
            existingGrade.MidtermScore = studentGradeDto.MidtermScore;
            existingGrade.MidtermTotal = studentGradeDto.MidtermTotal;
            // Quizzes and class standings already exist via `AddQuiz` / `AddClassStanding`
        }

        // Use existing helper to calculate weighted grade and grade point
        _calculateMidtermGrade(existingGrade, _weights, _gradeScale);

        await _context.SaveChangesAsync();

        return existingGrade;
    }


    public async Task<FinalsGrade> CalculateAndSaveFinalGradesAsync(FinalsGradeDto studentGradesDto)
    {
        var _gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var _weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (_weights == null)
        {
            throw new InvalidOperationException("Grade weights not found in the database.");
        }

        var newFinalGrades = new FinalsGrade
        {
            StudentId = studentGradesDto.StudentId,
            SubjectId = studentGradesDto.SubjectId,
            Semester = studentGradesDto.Semester,
            AcademicYear = studentGradesDto.AcademicYear,
            Quizzes = studentGradesDto.Quizzes.Select(q => new QuizList { Label = q.Label, QuizScore = q.QuizScore, TotalQuizScore = q.TotalQuizScore }).ToList(),
            RecitationScore = studentGradesDto.RecitationScore,
            AttendanceScore = studentGradesDto.AttendanceScore,
            ClassStandingItems = studentGradesDto.ClassStandingItems.Select(cs => new ClassStandingItem { Label = cs.Label, Score = cs.Score, Total = cs.Total }).ToList(),
            SEPScore = studentGradesDto.SEPScore,
            ProjectScore = studentGradesDto.ProjectScore,
            FinalsScore = studentGradesDto.FinalsScore,
            FinalsTotal = studentGradesDto.FinalsTotal,
        };

        _calculateFinalGrade(newFinalGrades, _weights, _gradeScale);


        _context.FinalsGrades.AddRange(newFinalGrades);
        await _context.SaveChangesAsync();

        return newFinalGrades;

    }

    public async Task<bool> AddQuizToMidtermGradeAsync(int studentId, int midtermGradeId, string label, int? score, int? total)
    {
        var midtermGrade = await _context.MidtermGrades
            .Include(mg => mg.Quizzes)
            .FirstOrDefaultAsync(mg => mg.Id == midtermGradeId && mg.StudentId == studentId);

        if (midtermGrade == null)
            return false;

        // Check if a quiz with the same label already exists (optional)
        if (midtermGrade.Quizzes.Any(q => q.Label == label))
            return false;

        midtermGrade.Quizzes.Add(new QuizList
        {
            Label = label,
            QuizScore = score,
            TotalQuizScore = total
        });

        await _context.SaveChangesAsync();
        return true;
    }


    // Existing method, modified to be specific to bulk upload
    public async Task<MidtermGradeUploadResult> CalculateAndSaveMidtermGradesAsync(List<MidtermGradeDto> studentGradesDto)
    {
        // This method will be responsible for handling the bulk upload logic
        // It will loop through the list and call the private _calculateMidtermGrade method for each student.
        throw new NotImplementedException();
    }
    public async Task<ResponseData<IEnumerable<MidtermGradeDto>>> GetMidtermGrades()
    {
        var currentUser = await _authRepository.GetCurrentUserAsync();

        var query = _context.MidtermGrades
            .Include(m => m.User)
            .Include(m => m.Quizzes)
            .Include(m => m.ClassStandingItems)
            .Include(m => m.Subject)
                .ThenInclude(s => s.Teacher)
                .AsNoTracking()
            .AsQueryable();

        if (currentUser.Role == UserRole.Teacher)
        {
            // FIXED: Use UserId if Teacher entity uses it
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.UserID == currentUser.Id);

            if (teacher == null)
            {
                return new ResponseData<IEnumerable<MidtermGradeDto>>
                {
                    Success = false,
                    Message = "No teacher record found for this user.",
                    Data = new List<MidtermGradeDto>()
                };
            }

            var teacherSubjectIds = teacher.Subjects.Select(s => s.Id).ToList();

            query = query.Where(m => m.SubjectId.HasValue && teacherSubjectIds.Contains(m.SubjectId.Value));
        }

        var studentsMidtermGrades = await query.ToListAsync();

        if (!studentsMidtermGrades.Any())
        {
            return new ResponseData<IEnumerable<MidtermGradeDto>>
            {
                Success = false,
                Message = "No midterm grades found.",
                Data = new List<MidtermGradeDto>()
            };
        }

        var gradesDto = _mapper.Map<List<MidtermGradeDto>>(studentsMidtermGrades);

        return new ResponseData<IEnumerable<MidtermGradeDto>>
        {
            Data = gradesDto,
            Success = true,
            Message = "Success"
        };
    }





    public async Task<ResponseData<IEnumerable<FinalsGradeDto>>> GetFinalGrades()
    {
        var currentUser = await _authRepository.GetCurrentUserAsync();

        var query = _context.FinalsGrades
            .Include(f => f.User)
            .Include(f => f.Quizzes)
            .Include(f => f.ClassStandingItems)
            .Include(f => f.Subject)
                .ThenInclude(s => s.Teacher)
            .AsNoTracking()
            .AsQueryable();

        // If current user is a teacher, filter grades to only their subjects
        if (currentUser.Role == UserRole.Teacher)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.UserID == currentUser.Id);

            if (teacher == null)
            {
                return new ResponseData<IEnumerable<FinalsGradeDto>>
                {
                    Success = false,
                    Message = "No teacher record found for this user.",
                    Data = new List<FinalsGradeDto>()
                };
            }

            var teacherSubjectIds = teacher.Subjects.Select(s => s.Id).ToList();
            query = query.Where(f => f.SubjectId.HasValue && teacherSubjectIds.Contains(f.SubjectId.Value));
        }

        var studentsFinalGrades = await query.ToListAsync();

        if (!studentsFinalGrades.Any())
        {
            return new ResponseData<IEnumerable<FinalsGradeDto>>
            {
                Success = false,
                Message = "No final grades found.",
                Data = new List<FinalsGradeDto>()
            };
        }

        var gradesDto = _mapper.Map<List<FinalsGradeDto>>(studentsFinalGrades);

        return new ResponseData<IEnumerable<FinalsGradeDto>>
        {
            Data = gradesDto,
            Success = true,
            Message = "Success"
        };
    }

    //delete
    public async Task<ResponseData<string>> DeleteMidtermGradesAsync(List<int> gradeIds)
    {
        if (gradeIds == null || !gradeIds.Any())
        {
            return new ResponseData<string>
            {
                Success = false,
                Message = "No grade IDs provided for deletion.",
                Data = null
            };
        }

        // Find the MidtermGrade records to delete, including their related collections.
        var gradesToDelete = await _context.MidtermGrades
            .Where(g => gradeIds.Contains(g.Id))
            .Include(g => g.Quizzes) // Include the related quizzes
            .Include(g => g.ClassStandingItems) // Include the related class standing items
            .ToListAsync();

        if (!gradesToDelete.Any())
        {
            return new ResponseData<string>
            {
                Success = false,
                Message = "No matching midterm grades found to delete.",
                Data = null
            };
        }

        // Remove the parent entities. Entity Framework will automatically
        // detect and remove the child entities because they are tracked.
        _context.MidtermGrades.RemoveRange(gradesToDelete);

        // Save changes to the database
        var deletedCount = await _context.SaveChangesAsync();

        return new ResponseData<string>
        {
            Success = true,
            Message = $"Records deleted successfully.",
            Data = null // No data to return for a deletion
        };
    }
    public async Task<ResponseData<string>> DeleteFinalsGradesAsync(List<int> gradeIds)
    {
        if (gradeIds == null || !gradeIds.Any())
        {
            return new ResponseData<string>
            {
                Success = false,
                Message = "No grade IDs provided for deletion.",
                Data = null
            };
        }

        // Find the FinalsGrade records to delete, including their related collections.
        var gradesToDelete = await _context.FinalsGrades
            .Where(g => gradeIds.Contains(g.Id))
            .Include(g => g.Quizzes)
            .Include(g => g.ClassStandingItems)
            .ToListAsync();

        if (!gradesToDelete.Any())
        {
            return new ResponseData<string>
            {
                Success = false,
                Message = "No matching finals grades found to delete.",
                Data = null
            };
        }

        // Remove the parent entities. Entity Framework will automatically
        // detect and remove the child entities because of the cascade delete configuration.
        _context.FinalsGrades.RemoveRange(gradesToDelete);

        // Save changes to the database
        var deletedCount = await _context.SaveChangesAsync();

        return new ResponseData<string>
        {
            Success = true,
            Message = $"Records deleted successfully.",
            Data = null // No data to return for a deletion
        };
    }

    private void _calculateFinalGrade(FinalsGrade studentGrade, GradeWeights _weights, List<GradePointEquivalent> _gradeScale)
    {
        // ===== Quizzes (30%) =====
        var totalQuizScore = studentGrade.Quizzes.Sum(q => q.QuizScore ?? 0);
        var totalQuizPossible = studentGrade.Quizzes.Sum(q => q.TotalQuizScore ?? 0);
        studentGrade.TotalQuizScore = totalQuizScore;
        studentGrade.QuizPG = totalQuizPossible > 0
            ? Math.Round((decimal)totalQuizScore / totalQuizPossible * 70 + 30, 2)
            : 0;
        studentGrade.QuizWeightedTotal = Math.Round(studentGrade.QuizPG * _weights.QuizWeighted, 2);

        // ===== Class Standing (25%) =====
        var totalCSPossible = studentGrade.ClassStandingItems.Sum(cs => cs.Total ?? 0);
        var totalCSScore = studentGrade.ClassStandingItems.Sum(cs => cs.Score ?? 0);
        studentGrade.ClassStandingTotalScore = totalCSPossible;
        studentGrade.ClassStandingPG = totalCSPossible > 0
            ? Math.Round((decimal)totalCSScore / totalCSPossible * 70 + 30, 2)
            : 0;

        decimal CSSAverage = Math.Round((studentGrade.RecitationScore + studentGrade.AttendanceScore + studentGrade.ClassStandingPG) / 3, 2);

        studentGrade.ClassStandingAverage = CSSAverage;
        studentGrade.ClassStandingWeightedTotal = Math.Round(studentGrade.ClassStandingAverage * _weights.ClassStandingWeighted, 2);

        // ===== SEP (5%) =====
        studentGrade.SEPPG = studentGrade.SEPScore;
        studentGrade.SEPWeightedTotal = Math.Round(studentGrade.SEPPG * _weights.SEPWeighted, 2);

        // ===== Project (10%) =====
        studentGrade.ProjectPG = studentGrade.ProjectScore;
        studentGrade.ProjectWeightedTotal = Math.Round(studentGrade.ProjectPG * _weights.ProjectWeighted, 2);

        // ===== Final Exam (30%) =====
        studentGrade.FinalsScore = studentGrade.FinalsScore;
        studentGrade.FinalsTotal = studentGrade.FinalsTotal;

        studentGrade.TotalScoreFinals = studentGrade.FinalsScore; //Naging score ng student sa exam
        studentGrade.OverallFinals = studentGrade.FinalsTotal; //Total ng Finals Exam
        var ovarallFinals = studentGrade.OverallFinals;

        studentGrade.CombinedFinalsAverage = Math.Round(((decimal)studentGrade.TotalScoreFinals / studentGrade.OverallFinals * 70)+30, 2);

        studentGrade.FinalsPG = studentGrade.CombinedFinalsAverage;
        studentGrade.FinalsWeightedTotal = Math.Round(studentGrade.FinalsPG * _weights.MidtermWeighted, 2, MidpointRounding.AwayFromZero);

        // ===== Total Final Grade =====
        studentGrade.TotalFinalsGrade = Math.Round(
            (double)(studentGrade.QuizWeightedTotal +
                     studentGrade.ClassStandingWeightedTotal +
                     studentGrade.SEPWeightedTotal +
                     studentGrade.ProjectWeightedTotal +
                     studentGrade.FinalsWeightedTotal),
            2, MidpointRounding.AwayFromZero
        );
        studentGrade.TotalFinalsGradeRounded = Math.Round(studentGrade.TotalFinalsGrade, 0, MidpointRounding.AwayFromZero);

        // ===== Grade Point Equivalent =====
        studentGrade.GradePointEquivalent = studentGrade.TotalFinalsGradeRounded <= 73
            ? 5.00
            : _gradeScale.FirstOrDefault(gp =>
                (!gp.MinPercentage.HasValue || studentGrade.TotalFinalsGradeRounded >= gp.MinPercentage.Value) &&
                studentGrade.TotalFinalsGradeRounded <= gp.MaxPercentage
            )?.GradePoint ?? 5.00;

        var tst = studentGrade.GradePointEquivalent;
    }

    private void _calculateMidtermGrade(MidtermGrade studentGrade, GradeWeights _weights, List<GradePointEquivalent> _gradeScale)
    {
        // ===== Quizzes (30%) =====
        var totalQuizScore = studentGrade.Quizzes.Sum(q => q.QuizScore ?? 0);
        var totalQuizPossible = studentGrade.Quizzes.Sum(q => q.TotalQuizScore ?? 0);
        studentGrade.TotalQuizScore = totalQuizScore;
        studentGrade.QuizPG = NormalizeRawScore(totalQuizScore, totalQuizPossible);
        studentGrade.QuizWeightedTotal = Math.Round(studentGrade.QuizPG * _weights.QuizWeighted, 2);

        // ===== Class Standing (25%) =====
        var totalCSScore = studentGrade.ClassStandingItems.Sum(cs => cs.Score ?? 0);
        var totalCSPossible = studentGrade.ClassStandingItems.Sum(cs => cs.Total ?? 0);
        studentGrade.ClassStandingTotalScore = totalCSPossible;
        studentGrade.ClassStandingPG = NormalizeRawScore(totalCSScore, totalCSPossible);

        decimal CSSAverage = Math.Round((studentGrade.RecitationScore + studentGrade.AttendanceScore + studentGrade.ClassStandingPG) / 3, 2);
        studentGrade.ClassStandingAverage = CSSAverage;
        studentGrade.ClassStandingWeightedTotal = Math.Round(CSSAverage * _weights.ClassStandingWeighted, 2);

        // ===== SEP (5%) =====
        studentGrade.SEPPG = studentGrade.SEPScore;
        studentGrade.SEPWeightedTotal = Math.Round(studentGrade.SEPPG * _weights.SEPWeighted, 2);

        // ===== Project (10%) =====
        studentGrade.ProjectPG = studentGrade.ProjectScore;
        studentGrade.ProjectWeightedTotal = Math.Round(studentGrade.ProjectPG * _weights.ProjectWeighted, 2);

        // ===== Prelim + Midterm Combined (30%) =====
        studentGrade.TotalScorePerlimAndMidterm = studentGrade.PrelimScore + studentGrade.MidtermScore;
        studentGrade.OverallPrelimAndMidterm = studentGrade.PrelimTotal + studentGrade.MidtermTotal;
        studentGrade.CombinedPrelimMidtermAverage = NormalizeRawScore((int)studentGrade.TotalScorePerlimAndMidterm, (int)studentGrade.OverallPrelimAndMidterm);
        studentGrade.MidtermPG = studentGrade.CombinedPrelimMidtermAverage;
        studentGrade.MidtermWeightedTotal = Math.Round(studentGrade.MidtermPG * _weights.MidtermWeighted, 2, MidpointRounding.AwayFromZero);

        // ===== Total Midterm Grade =====
        studentGrade.TotalMidtermGrade = Math.Round(
            (double)(
                studentGrade.QuizWeightedTotal +
                studentGrade.ClassStandingWeightedTotal +
                studentGrade.SEPWeightedTotal +
                studentGrade.ProjectWeightedTotal +
                studentGrade.MidtermWeightedTotal
            ),
            2, MidpointRounding.AwayFromZero
        );
        studentGrade.TotalMidtermGradeRounded = Math.Round(studentGrade.TotalMidtermGrade, 0, MidpointRounding.AwayFromZero);

        // ===== Grade Point Equivalent =====
        if (studentGrade.TotalMidtermGradeRounded <= 73)
        {
            studentGrade.GradePointEquivalent = 5.00;
        }
        else
        {
            var match = _gradeScale.FirstOrDefault(gp =>
                (!gp.MinPercentage.HasValue || studentGrade.TotalMidtermGradeRounded >= gp.MinPercentage.Value) &&
                studentGrade.TotalMidtermGradeRounded <= gp.MaxPercentage
            );
            studentGrade.GradePointEquivalent = match?.GradePoint ?? 5.00;
        }
    }


    //private void _calculateMidtermGrade(MidtermGrade studentGrade, GradeWeights _weights, List<GradePointEquivalent> _gradeScale)
    //{
    //    // ===== Quizzes (30%) =====
    //    var totalQuizScore = studentGrade.Quizzes.Sum(q => q.QuizScore ?? 0);
    //    var totalQuizPossible = studentGrade.Quizzes.Sum(q => q.TotalQuizScore ?? 0);
    //    studentGrade.TotalQuizScore = totalQuizScore;
    //    studentGrade.QuizPG = totalQuizPossible > 0
    //        ? Math.Round((decimal)totalQuizScore / totalQuizPossible * 70 + 30, 2)
    //        : 0;
    //    studentGrade.QuizWeightedTotal = Math.Round(studentGrade.QuizPG * _weights.QuizWeighted, 2);

    //    // ===== Class Standing (25%) =====
    //    var totalCSPossible = studentGrade.ClassStandingItems.Sum(cs => cs.Total ?? 0);
    //    var totalCSScore = studentGrade.ClassStandingItems.Sum(cs => cs.Score ?? 0);
    //    studentGrade.ClassStandingTotalScore = totalCSPossible;
    //    studentGrade.ClassStandingPG = totalCSPossible > 0
    //        ? Math.Round((decimal)totalCSScore / totalCSPossible * 70 + 30, 2) : 0;

    //    decimal CSSAverage = Math.Round((studentGrade.RecitationScore + studentGrade.AttendanceScore + studentGrade.ClassStandingPG) / 3, 2);

    //    studentGrade.ClassStandingAverage = CSSAverage;
    //    studentGrade.ClassStandingWeightedTotal = Math.Round(studentGrade.ClassStandingAverage * _weights.ClassStandingWeighted, 2);

    //    // ===== SEP (5%) =====
    //    studentGrade.SEPPG = studentGrade.SEPScore;
    //    studentGrade.SEPWeightedTotal = Math.Round(studentGrade.SEPPG * _weights.SEPWeighted, 2);

    //    // ===== Project (10%) =====
    //    studentGrade.ProjectPG = studentGrade.ProjectScore;
    //    studentGrade.ProjectWeightedTotal = Math.Round(studentGrade.ProjectPG * _weights.ProjectWeighted, 2);

    //    // ===== Prelim + Midterm Combined (30%) =====
    //    studentGrade.TotalScorePerlimAndMidterm = studentGrade.PrelimScore + studentGrade.MidtermScore;
    //    studentGrade.OverallPrelimAndMidterm = studentGrade.PrelimTotal + studentGrade.MidtermTotal;
    //    studentGrade.CombinedPrelimMidtermAverage = studentGrade.OverallPrelimAndMidterm > 0
    //        ? Math.Round(((decimal)studentGrade.TotalScorePerlimAndMidterm / studentGrade.OverallPrelimAndMidterm * 70) + 30, 2)
    //        : 0;
    //    studentGrade.MidtermPG = studentGrade.CombinedPrelimMidtermAverage;
    //    var midTermPg = studentGrade.MidtermPG;
    //    studentGrade.MidtermWeightedTotal = Math.Round(studentGrade.MidtermPG * _weights.MidtermWeighted, 2, MidpointRounding.AwayFromZero);

    //    // ===== Total Midterm Grade =====
    //    studentGrade.TotalMidtermGrade = Math.Round(
    //        (double)(studentGrade.QuizWeightedTotal +
    //                 studentGrade.ClassStandingWeightedTotal +
    //                 studentGrade.SEPWeightedTotal +
    //                 studentGrade.ProjectWeightedTotal +
    //                 studentGrade.MidtermWeightedTotal),
    //        2, MidpointRounding.AwayFromZero
    //    );
    //    studentGrade.TotalMidtermGradeRounded = Math.Round(studentGrade.TotalMidtermGrade, 0, MidpointRounding.AwayFromZero);

    //    // ===== Grade Point Equivalent =====
    //    if (studentGrade.TotalMidtermGradeRounded <= 73)
    //    {
    //        studentGrade.GradePointEquivalent = 5.00;
    //    }
    //    else
    //    {
    //        var match = _gradeScale.FirstOrDefault(gp =>
    //            (!gp.MinPercentage.HasValue || studentGrade.TotalMidtermGradeRounded >= gp.MinPercentage.Value) &&
    //            studentGrade.TotalMidtermGradeRounded <= gp.MaxPercentage
    //        );
    //        studentGrade.GradePointEquivalent = match?.GradePoint ?? 5.00;
    //    }
    //}

    public async Task<bool> AddQuizToMidtermGradeAsync(int studentId, int gradeId, string label, int score, int total)
    {
        var grade = await _context.MidtermGrades
            .Include(g => g.Quizzes)
            .FirstOrDefaultAsync(g => g.Id == gradeId && g.StudentId == studentId);

        if (grade == null) return false;

        if (grade.Quizzes.Any(q => q.Label == label))
            return false;

        grade.Quizzes.Add(new QuizList
        {
            Label = label,
            QuizScore = score,
            TotalQuizScore = total
        });

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> AddClassStandingToMidtermGradeAsync(int studentId, int gradeId, string label, int score, int total)
    {
        var grade = await _context.MidtermGrades
            .Include(g => g.ClassStandingItems)
            .FirstOrDefaultAsync(g => g.Id == gradeId && g.StudentId == studentId);

        if (grade == null) return false;

        if (grade.ClassStandingItems.Any(cs => cs.Label == label))
            return false;

        grade.ClassStandingItems.Add(new ClassStandingItem
        {
            Label = label,
            Score = score,
            Total = total
        });

        await _context.SaveChangesAsync();
        return true;
    }

    private decimal NormalizeRawScore(int scored, int possible)
    {
        if (possible == 0) return 0;
        return Math.Round(((decimal)scored / possible) * 70 + 30, 2);
    }

    public async Task<MidtermGradeUploadResult> CalculateMidtermGradesForAllSubjectsAsync()
    {
        var result = new MidtermGradeUploadResult();

        var gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (weights == null)
        {
            result.Warnings.Add("Grade weights not found in the database.");
            return result;
        }

        // Get current academic period
        var currentPeriod = await _context.AcademicPeriods.FirstOrDefaultAsync(ap => ap.IsCurrent);
        if (currentPeriod == null)
        {
            result.Warnings.Add("No current academic period found.");
            return result;
        }

        // Get all subjects
        var subjects = await _context.Subjects.ToListAsync();

        foreach (var subject in subjects)
        {
            // Get midterm grades for this subject
            var midtermGrades = await _context.MidtermGrades
                .Include(mg => mg.Quizzes)
                .Include(mg => mg.ClassStandingItems)
                .Where(mg => mg.SubjectId == subject.Id && mg.AcademicPeriodId == currentPeriod.Id)
                .ToListAsync();

            if (!midtermGrades.Any())
            {
                result.Warnings.Add($"No midterm grades found for subject {subject.SubjectName}.");
                continue;
            }

            foreach (var grade in midtermGrades)
            {
                try
                {
                    _calculateMidtermGrade(grade, weights, gradeScale);
                    result.CalculatedGrades.Add(grade);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Failed to calculate grade for student ID {grade.StudentId} in {subject.SubjectName}: {ex.Message}");
                }
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<FinalsGradeUploadResult> CalculateFinalsGradesForAllSubjectsAsync()
    {
        var result = new FinalsGradeUploadResult();

        var gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (weights == null)
        {
            result.Warnings.Add("Grade weights not found in the database.");
            return result;
        }

        // Get current academic period
        var currentPeriod = await _context.AcademicPeriods.FirstOrDefaultAsync(ap => ap.IsCurrent);
        if (currentPeriod == null)
        {
            result.Warnings.Add("No current academic period found.");
            return result;
        }

        // Get all subjects
        var subjects = await _context.Subjects.ToListAsync();

        foreach (var subject in subjects)
        {
            // Get finals grades for this subject
            var finalsGrades = await _context.FinalsGrades
                .Include(f => f.Quizzes)
                .Include(f => f.ClassStandingItems)
                .Include(f => f.Subject)
                .ThenInclude(s => s.Teacher)
                .Where(f => f.SubjectId == subject.Id && f.AcademicPeriodId == currentPeriod.Id)
                .ToListAsync();

            if (!finalsGrades.Any())
            {
                result.Warnings.Add($"No finals grades found for subject {subject.SubjectName}.");
                continue;
            }

            foreach (var grade in finalsGrades)
            {
                try
                {
                    _calculateFinalGrade(grade, weights, gradeScale);
                    result.CalculatedGrades.Add(grade);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add($"Failed to calculate finals grade for student ID {grade.StudentId} in {subject.SubjectName}: {ex.Message}");
                }
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<MidtermGradeUploadResult> CalculateMidtermGradesForSubjectAsync(int subjectId, int academicPeriodId)
    {
        var result = new MidtermGradeUploadResult();

        var gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (weights == null)
        {
            result.Warnings.Add("Grade weights not found in the database.");
            return result;
        }

        // Get all midterm grades for the given subject and period
        var midtermGrades = await _context.MidtermGrades
            .Include(mg => mg.Quizzes)
            .Include(mg => mg.ClassStandingItems)
            .Where(mg => mg.SubjectId == subjectId && mg.AcademicPeriodId == academicPeriodId)
            .ToListAsync();

        if (!midtermGrades.Any())
        {
            result.Warnings.Add("No midterm grade records found for the specified subject and academic period.");
            return result;
        }

        foreach (var grade in midtermGrades)
        {
            try
            {
                _calculateMidtermGrade(grade, weights, gradeScale);
                result.CalculatedGrades.Add(grade);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to calculate grade for student ID {grade.StudentId}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }

    //Finals
    public async Task<FinalsGradeUploadResult> CalculateFinalsGradesForSubjectAsync(int subjectId, int academicPeriodId)
    {
        var result = new FinalsGradeUploadResult();

        var gradeScale = await _context.GradePointEquivalents.ToListAsync();
        var weights = await _context.GradeWeights.FirstOrDefaultAsync();

        if (weights == null)
        {
            result.Warnings.Add("Grade weights not found in the database.");
            return result;
        }

        // Get all finals grades for the given subject and period
        var finalsGrades = await _context.FinalsGrades
            .Include(f => f.Quizzes)
            .Include(f => f.ClassStandingItems)
            .Include(f => f.Subject)
            .ThenInclude(s => s.Teacher)
            .Where(f => f.SubjectId == subjectId && f.AcademicPeriodId == academicPeriodId)
            .ToListAsync();

        if (!finalsGrades.Any())
        {
            result.Warnings.Add("No finals grade records found for the specified subject and academic period.");
            return result;
        }

        foreach (var grade in finalsGrades)
        {
            try
            {
                _calculateFinalGrade(grade, weights, gradeScale);
                result.CalculatedGrades.Add(grade);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to calculate finals grade for student ID {grade.StudentId}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }


    public async Task<IEnumerable<MidtermGradeDto>> GetGradesBySubjectAndPeriodAsync(int subjectId, int academicPeriodId)
    {
        return await _context.MidtermGrades
            .Include(m => m.User)
            .Include(m => m.Subject)
                .ThenInclude(s => s.Teacher)
            .Include(m => m.Quizzes)
            .Include(m => m.ClassStandingItems)
            .Include(m => m.AcademicPeriod)
            .Where(m => m.SubjectId == subjectId && m.AcademicPeriodId == academicPeriodId)
            .Select(m => new MidtermGradeDto
            {
                Id = m.Id,
                StudentId = m.StudentId,
                StudentFullName = m.User.Fullname,
                StudentNumber = m.User.StudentNumber,
                Department = m.User.Department,

                SubjectId = m.SubjectId,
                SubjectCode = m.Subject.SubjectCode,
                SubjectName = m.Subject.SubjectName,
                SubjectTeacher = m.Subject.Teacher.Fullname,

                // ✅ Academic Info
                AcademicPeriodId = m.AcademicPeriodId,
                AcademicYear = m.AcademicYear ?? m.AcademicPeriod.AcademicYear,
                Semester = m.Semester ?? m.AcademicPeriod.Semester,

                // ✅ Quizzes
                Quizzes = m.Quizzes.Select(q => new QuizListDto
                {
                    Id = q.Id,
                    Label = q.Label,
                    QuizScore = q.QuizScore,
                    TotalQuizScore = q.TotalQuizScore
                }).ToList(),

                // ✅ Class Standing
                ClassStandingItems = m.ClassStandingItems.Select(cs => new ClassStandingItemDto
                {
                    Id = cs.Id,
                    Label = cs.Label,
                    Score = cs.Score,
                    Total = cs.Total
                }).ToList(),

                // ✅ Other Scores
                RecitationScore = m.RecitationScore,
                AttendanceScore = m.AttendanceScore,
                ProjectScore = m.ProjectScore,
                SEPScore = m.User.Department == "BSED" ? m.SEPScore : 0,
                MidtermScore = m.MidtermScore,
                MidtermTotal = m.MidtermTotal,
                GradePointEquivalent = m.GradePointEquivalent,
                TotalMidtermGrade = m.TotalMidtermGrade,
                TotalMidtermGradeRounded = m.TotalMidtermGradeRounded
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateMidtermGradeAsync(int studentId, MidtermGradeDto updatedGrade)
    {
        var existingGrade = await _context.MidtermGrades
            .Include(m => m.Quizzes)
            .Include(m => m.ClassStandingItems)
            .FirstOrDefaultAsync(m => m.Id == updatedGrade.Id);

        if (existingGrade == null)
            return false;

        // === Basic Updates ===
        existingGrade.RecitationScore = updatedGrade.RecitationScore;
        existingGrade.AttendanceScore = updatedGrade.AttendanceScore;
        existingGrade.ProjectScore = updatedGrade.ProjectScore;

        existingGrade.SEPScore = string.Equals(updatedGrade.Department, "BSED", StringComparison.OrdinalIgnoreCase)
            ? updatedGrade.SEPScore
            : 0;

        existingGrade.MidtermScore = updatedGrade.MidtermScore;
        existingGrade.MidtermTotal = updatedGrade.MidtermTotal;
        existingGrade.PrelimScore = updatedGrade.PrelimScore;
        existingGrade.PrelimTotal = updatedGrade.PrelimTotal;

        existingGrade.Semester = updatedGrade.Semester;
        existingGrade.AcademicYear = updatedGrade.AcademicYear;
        existingGrade.AcademicPeriodId = updatedGrade.AcademicPeriodId;

        // === Update Quizzes (no re-creation) ===
        var updatedQuizLabels = updatedGrade.Quizzes.Select(q => q.Label).ToHashSet();

        foreach (var quizDto in updatedGrade.Quizzes)
        {
            var existingQuiz = existingGrade.Quizzes.FirstOrDefault(q => q.Label == quizDto.Label);

            if (existingQuiz != null)
            {
                existingQuiz.QuizScore = quizDto.QuizScore ?? 0;
                existingQuiz.TotalQuizScore = quizDto.TotalQuizScore ?? 0;
            }
            else
            {
                existingGrade.Quizzes.Add(new QuizList
                {
                    Label = quizDto.Label,
                    QuizScore = quizDto.QuizScore ?? 0,
                    TotalQuizScore = quizDto.TotalQuizScore ?? 0
                });
            }
        }

        // Remove quizzes that are no longer present
        existingGrade.Quizzes
            .Where(q => !updatedQuizLabels.Contains(q.Label))
            .ToList()
            .ForEach(q => _context.QuizLists.Remove(q));

        // === Update Class Standing Items ===
        var updatedCSLabels = updatedGrade.ClassStandingItems.Select(cs => cs.Label).ToHashSet();

        foreach (var csDto in updatedGrade.ClassStandingItems)
        {
            var existingCS = existingGrade.ClassStandingItems.FirstOrDefault(c => c.Label == csDto.Label);

            if (existingCS != null)
            {
                existingCS.Score = csDto.Score ?? 0;
                existingCS.Total = csDto.Total ?? 0;
            }
            else
            {
                existingGrade.ClassStandingItems.Add(new ClassStandingItem
                {
                    Label = csDto.Label,
                    Score = csDto.Score ?? 0,
                    Total = csDto.Total ?? 0
                });
            }
        }

        // Remove deleted ClassStanding items
        existingGrade.ClassStandingItems
            .Where(cs => !updatedCSLabels.Contains(cs.Label))
            .ToList()
            .ForEach(cs => _context.ClassStanding.Remove(cs));

        await _context.SaveChangesAsync();
        return true;
    }

    //Finals
    public async Task<ResponseData<IEnumerable<FinalsGradeDto>>> GetFinalsGradesBySubjectAndPeriodAsync(int subjectId, int academicPeriodId)
    {
        var finalsGrades = await _context.FinalsGrades
            .Include(f => f.User)
            .Include(f => f.Subject)
                .ThenInclude(s => s.Teacher)
            .Include(f => f.Quizzes)
            .Include(f => f.ClassStandingItems)
            .Include(f => f.AcademicPeriod)
            .Where(f => f.SubjectId == subjectId && f.AcademicPeriodId == academicPeriodId)
            .Select(f => new FinalsGradeDto
            {
                Id = f.Id,

                // ✅ Student Info
                StudentId = f.StudentId,
                StudentFullName = f.User.Fullname,
                StudentNumber = f.User.StudentNumber,
                Department = f.User.Department,

                // ✅ Subject Info
                SubjectId = f.SubjectId,
                SubjectCode = f.Subject.SubjectCode,
                SubjectName = f.Subject.SubjectName,
                SubjectTeacher = f.Subject.Teacher.Fullname,

                // ✅ Academic Info
                AcademicYearId = f.AcademicPeriodId,
                AcademicYear = f.AcademicPeriod.AcademicYear,
                Semester = f.AcademicPeriod.Semester,

                // ✅ Quizzes
                Quizzes = f.Quizzes.Select(q => new QuizListDto
                {
                    Id = q.Id,
                    Label = q.Label,
                    QuizScore = q.QuizScore,
                    TotalQuizScore = q.TotalQuizScore
                }).ToList(),

                // ✅ Class Standing
                ClassStandingItems = f.ClassStandingItems.Select(cs => new ClassStandingItemDto
                {
                    Id = cs.Id,
                    Label = cs.Label,
                    Score = cs.Score,
                    Total = cs.Total
                }).ToList(),

                // ✅ Other Scores
                RecitationScore = f.RecitationScore,
                AttendanceScore = f.AttendanceScore,
                ProjectScore = f.ProjectScore,

                // ✅ SEPScore condition
                SEPScore = f.User.Department == "BSED" ? f.SEPScore : 0,

                // ✅ Finals Exam
                FinalsScore = f.FinalsScore,
                FinalsTotal = f.FinalsTotal,

                // ✅ Computed/Weighted Grades
                TotalScoreFinals = f.TotalScoreFinals,
                OverallFinals = f.OverallFinals,
                CombinedFinalsAverage = f.CombinedFinalsAverage,
                FinalsPG = f.FinalsPG,
                FinalsWeightedTotal = f.FinalsWeightedTotal,

                // ✅ ClassStanding / Quiz Weighted Grades
                QuizPG = f.QuizPG,
                ClassStandingTotalScore = f.ClassStandingTotalScore,
                ClassStandingAverage = f.ClassStandingAverage,
                ClassStandingPG = f.ClassStandingPG,

                // ✅ Final Grade Values
                TotalFinalsGrade = f.TotalFinalsGrade,
                TotalFinalsGradeRounded = f.TotalFinalsGradeRounded,
                GradePointEquivalent = f.GradePointEquivalent
            })
            .AsNoTracking()
            .ToListAsync();

        if (!finalsGrades.Any())
        {
            return new ResponseData<IEnumerable<FinalsGradeDto>>
            {
                Success = false,
                Message = "No finals grades found.",
                Data = new List<FinalsGradeDto>()
            };
        }

        return new ResponseData<IEnumerable<FinalsGradeDto>>
        {
            Success = true,
            Message = "Success",
            Data = finalsGrades
        };
    }

    public async Task<bool> UpdateFinalsGradeAsync(int studentId, FinalsGradeDto updatedGrade)
    {
        var existingGrade = await _context.FinalsGrades
            .Include(f => f.Quizzes)
            .Include(f => f.ClassStandingItems)
            .FirstOrDefaultAsync(f => f.Id == updatedGrade.Id);

        if (existingGrade == null)
            return false;

        // === Basic Updates ===
        existingGrade.RecitationScore = updatedGrade.RecitationScore;
        existingGrade.AttendanceScore = updatedGrade.AttendanceScore;
        existingGrade.ProjectScore = updatedGrade.ProjectScore;
        existingGrade.FinalsScore = updatedGrade.FinalsScore;
        existingGrade.FinalsTotal = updatedGrade.FinalsTotal;

        existingGrade.SEPScore = string.Equals(updatedGrade.Department, "BSED", StringComparison.OrdinalIgnoreCase)
            ? updatedGrade.SEPScore
            : 0;

        existingGrade.Semester = updatedGrade.Semester;
        existingGrade.AcademicYear = updatedGrade.AcademicYear;
        existingGrade.AcademicPeriodId = updatedGrade.AcademicYearId;

        // === Update Quizzes (no re-creation) ===
        var updatedQuizLabels = updatedGrade.Quizzes.Select(q => q.Label).ToHashSet();

        // Update or add
        foreach (var quizDto in updatedGrade.Quizzes)
        {
            var existingQuiz = existingGrade.Quizzes.FirstOrDefault(q => q.Label == quizDto.Label);

            if (existingQuiz != null)
            {
                // Update existing quiz
                existingQuiz.QuizScore = quizDto.QuizScore ?? 0;
                existingQuiz.TotalQuizScore = quizDto.TotalQuizScore ?? 0;
            }
            else
            {
                // Add new quiz if not found
                existingGrade.Quizzes.Add(new QuizList
                {
                    Label = quizDto.Label,
                    QuizScore = quizDto.QuizScore ?? 0,
                    TotalQuizScore = quizDto.TotalQuizScore ?? 0
                });
            }
        }

        // Remove quizzes no longer in DTO
        existingGrade.Quizzes
            .Where(q => !updatedQuizLabels.Contains(q.Label))
            .ToList()
            .ForEach(q => _context.QuizLists.Remove(q));

        // === Update Class Standing ===
        var updatedCSLabels = updatedGrade.ClassStandingItems.Select(cs => cs.Label).ToHashSet();

        foreach (var csDto in updatedGrade.ClassStandingItems)
        {
            var existingCS = existingGrade.ClassStandingItems.FirstOrDefault(c => c.Label == csDto.Label);

            if (existingCS != null)
            {
                existingCS.Score = csDto.Score ?? 0;
                existingCS.Total = csDto.Total ?? 0;
            }
            else
            {
                existingGrade.ClassStandingItems.Add(new ClassStandingItem
                {
                    Label = csDto.Label,
                    Score = csDto.Score ?? 0,
                    Total = csDto.Total ?? 0
                });
            }
        }

        // Remove deleted class standing items
        existingGrade.ClassStandingItems
            .Where(cs => !updatedCSLabels.Contains(cs.Label))
            .ToList()
            .ForEach(cs => _context.ClassStanding.Remove(cs));

        await _context.SaveChangesAsync();
        return true;
    }


}
