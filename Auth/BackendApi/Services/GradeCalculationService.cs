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
            MidtermTotal = studentGradeDto.MidtermTotal
        };

        // Call a private helper method to perform the calculation
        _calculateMidtermGrade(studentGrade, _weights, _gradeScale);

        // Save to DB
        _context.MidtermGrades.Add(studentGrade);
        await _context.SaveChangesAsync();

        return studentGrade;
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
        var studentsFinalGrades = await _context.FinalsGrades
            .Include(m => m.User)
            .Include(m => m.Quizzes)
            .Include(m => m.ClassStandingItems)
            .Include(m => m.Subject)
                .ThenInclude(t => t.Teacher)
            .ToListAsync();

        if (!studentsFinalGrades.Any())
        {
            return new ResponseData<IEnumerable<FinalsGradeDto>>
            {
                Success = false,
                Message = "No final grades found in the database.",
                Data = new List<FinalsGradeDto>()
            };
        }

        // The mapper returns a List<MidtermGradeDto>
        var gradesDto = _mapper.Map<List<FinalsGradeDto>>(studentsFinalGrades);

        // The List<MidtermGradeDto> is implicitly convertible to IEnumerable<MidtermGradeDto>
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
        studentGrade.QuizPG = totalQuizPossible > 0
            ? Math.Round((decimal)totalQuizScore / totalQuizPossible * 70 + 30, 2)
            : 0;
        studentGrade.QuizWeightedTotal = Math.Round(studentGrade.QuizPG * _weights.QuizWeighted, 2);

        // ===== Class Standing (25%) =====
        var totalCSPossible = studentGrade.ClassStandingItems.Sum(cs => cs.Total ?? 0);
        var totalCSScore = studentGrade.ClassStandingItems.Sum(cs => cs.Score ?? 0);
        studentGrade.ClassStandingTotalScore = totalCSPossible;
        studentGrade.ClassStandingPG = totalCSPossible > 0
            ? Math.Round((decimal)totalCSScore / totalCSPossible * 70 + 30, 2) : 0;

        decimal CSSAverage = Math.Round((studentGrade.RecitationScore + studentGrade.AttendanceScore + studentGrade.ClassStandingPG) / 3, 2);

        studentGrade.ClassStandingAverage = CSSAverage;
        studentGrade.ClassStandingWeightedTotal = Math.Round(studentGrade.ClassStandingAverage * _weights.ClassStandingWeighted, 2);

        // ===== SEP (5%) =====
        studentGrade.SEPPG = studentGrade.SEPScore;
        studentGrade.SEPWeightedTotal = Math.Round(studentGrade.SEPPG * _weights.SEPWeighted, 2);

        // ===== Project (10%) =====
        studentGrade.ProjectPG = studentGrade.ProjectScore;
        studentGrade.ProjectWeightedTotal = Math.Round(studentGrade.ProjectPG * _weights.ProjectWeighted, 2);

        // ===== Prelim + Midterm Combined (30%) =====
        studentGrade.TotalScorePerlimAndMidterm = studentGrade.PrelimScore + studentGrade.MidtermScore;
        studentGrade.OverallPrelimAndMidterm = studentGrade.PrelimTotal + studentGrade.MidtermTotal;
        studentGrade.CombinedPrelimMidtermAverage = studentGrade.OverallPrelimAndMidterm > 0
            ? Math.Round(((decimal)studentGrade.TotalScorePerlimAndMidterm / studentGrade.OverallPrelimAndMidterm * 70) + 30, 2)
            : 0;
        studentGrade.MidtermPG = studentGrade.CombinedPrelimMidtermAverage;
        var midTermPg = studentGrade.MidtermPG;
        studentGrade.MidtermWeightedTotal = Math.Round(studentGrade.MidtermPG * _weights.MidtermWeighted, 2, MidpointRounding.AwayFromZero);

        // ===== Total Midterm Grade =====
        studentGrade.TotalMidtermGrade = Math.Round(
            (double)(studentGrade.QuizWeightedTotal +
                     studentGrade.ClassStandingWeightedTotal +
                     studentGrade.SEPWeightedTotal +
                     studentGrade.ProjectWeightedTotal +
                     studentGrade.MidtermWeightedTotal),
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

}
