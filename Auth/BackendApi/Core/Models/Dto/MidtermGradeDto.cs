namespace BackendApi.Core.Models.Dto
{
    /// <summary>
    /// Data Transfer Object for MidtermGrade, used to transfer data between layers.
    /// This DTO includes the student's full name for display purposes.
    /// </summary>
    /// 

    public class CalculateGradesInputDto
    {
        public int SubjectId { get; set; }
        public int AcademicPeriodId { get; set; }
    }


    public class MidtermGradeDto
    {
        // Student Information
        public int StudentId { get; set; }
        public string? StudentNumber { get; set; }
        public string? Department { get; set; }

        public string? StudentFullName { get; set; } // Added to include the student's full name
        public int? SubjectId { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectTeacher { get; set; }

        // MidtermGrade properties
        public int Id { get; set; }
        public ICollection<QuizListDto> Quizzes { get; set; } = new List<QuizListDto>();
        public int TotalQuizScore { get; set; }
        public decimal QuizPG { get; set; }
        public decimal QuizWeighted { get; set; }

        public int RecitationScore { get; set; }
        public int AttendanceScore { get; set; }

        public ICollection<ClassStandingItemDto> ClassStandingItems { get; set; } = new List<ClassStandingItemDto>();
        public int ClassStandingTotalScore { get; set; }
        public decimal ClassStandingAverage { get; set; }
        public decimal ClassStandingPG { get; set; }
        public decimal ClassStandingWeighted { get; set; }

        public int SEPScore { get; set; }
        public decimal SEPPG { get; set; }
        public decimal SEPWeighted { get; set; }

        public int ProjectScore { get; set; }
        public decimal ProjectPG { get; set; }
        public decimal ProjectWeighted { get; set; }

        public int PrelimScore { get; set; }
        public int PrelimTotal { get; set; }

        public int MidtermScore { get; set; }
        public int MidtermTotal { get; set; }

        public decimal TotalScorePerlimAndMidterm { get; set; }
        public decimal OverallPrelimAndMidterm { get; set; }

        public decimal CombinedPrelimMidtermAverage { get; set; }
        public decimal MidtermPG { get; set; }
        public decimal MidtermExamWeighted { get; set; }

        public double TotalMidtermGrade { get; set; }
        public double TotalMidtermGradeRounded { get; set; }

        public double GradePointEquivalent { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
        public int? AcademicPeriodId { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for MidtermQuizList.
    /// </summary>
    public class QuizListDto
    {
        public int Id { get; set; }
        public string? Label { get; set; }
        public int? QuizScore { get; set; }
        public int? TotalQuizScore { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for ClassStandingItem.
    /// </summary>
    public class ClassStandingItemDto
    {
        public int Id { get; set; }
        public string? Label { get; set; }
        public int? Score { get; set; }
        public int? Total { get; set; }
    }
    public class MidtermGradeUploadResult
    {
        public List<MidtermGrade> CalculatedGrades { get; set; } = new List<MidtermGrade>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
