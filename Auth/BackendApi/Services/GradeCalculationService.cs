using BackendApi.Context;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using Microsoft.EntityFrameworkCore;

public class GradeCalculationService : IGradeCalculationService
{
    private readonly AppDbContext _context;

    public GradeCalculationService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<MidtermGrade> CalculateAndSaveMidtermGradeAsync(MidtermGradeDto studentGradeDto)
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
            // The FullName will be retrieved from the Student model when the DTO is created
            Quizzes = studentGradeDto.Quizzes.Select(q => new MidtermQuizList { Label = q.Label, QuizScore = q.QuizScore, TotalQuizScore = q.TotalQuizScore }).ToList(),
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

        // ===== Quizzes (30%) =====
        var totalQuizScore = studentGrade.Quizzes.Sum(q => q.QuizScore ?? 0);
        var totalQuizPossible = studentGrade.Quizzes.Sum(q => q.TotalQuizScore ?? 0);
        studentGrade.TotalQuizScore = totalQuizScore;
        studentGrade.QuizPG = totalQuizPossible > 0
            ? Math.Round((decimal)totalQuizScore / totalQuizPossible * 70 + 30, 2)
            : 0;
        studentGrade.QuizWeightedTotal = Math.Round(studentGrade.QuizPG * _weights.QuizWeighted, 2);

        // ===== Class Standing (25%) =====
        //total score
        var totalCSPossible = studentGrade.ClassStandingItems.Sum(cs => cs.Total ?? 0);
        //nakuhag score ng student
        var totalCSScore = studentGrade.ClassStandingItems.Sum(cs => cs.Score ?? 0);

        // Assuming Recitation and Attendance scores are also part of the total Class Standing.
        // Adjust the totalCSPossible if Recitation and Attendance also have possible maximums.
        studentGrade.ClassStandingTotalScore = totalCSPossible;
        studentGrade.ClassStandingPG = totalCSScore > 0
            ? Math.Round((decimal)totalCSScore / totalCSPossible * 70 + 30, 2) : 0;

        decimal CSSAverage = Math.Round((studentGrade.RecitationScore + studentGrade.AttendanceScore + studentGrade.ClassStandingPG)/3, 2);

        studentGrade.ClassStandingAverage = (decimal)CSSAverage;
        studentGrade.ClassStandingWeightedTotal = Math.Round(studentGrade.ClassStandingAverage * _weights.ClassStandingWeighted, 2);

        // ===== SEP (5%) =====
        // Assuming SEPScore is already a percentage grade
        studentGrade.SEPPG = studentGrade.SEPScore;
        studentGrade.SEPWeightedTotal = Math.Round(studentGrade.SEPPG * _weights.SEPWeighted, 2);

        // ===== Project (10%) =====
        // Assuming ProjectScore is already a percentage grade
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
        var test = studentGrade.TotalMidtermGrade;
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

        // Save to DB
        _context.MidtermGrades.Add(studentGrade);
        await _context.SaveChangesAsync();

        return studentGrade;
    }
}
