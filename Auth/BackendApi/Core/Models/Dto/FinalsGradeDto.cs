namespace BackendApi.Core.Models.Dto
{
    public class FinalsGradeDto
    {
        // Student Information
        public int StudentId { get; set; }
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

        // Prelim and Midterm Exam (30% of the grade)
        public int FinalsScore { get; set; }
        public int FinalsTotal { get; set; }

        public decimal TotalScoreFinals { get; set; }
        public decimal OverallFinals { get; set; }
        public decimal CombinedFinalsAverage { get; set; } // Average of prelim & midterm scores
        public decimal FinalsPG { get; set; } // Percentage Grade for the combined exam
        public decimal FinalsWeightedTotal { get; set; } // The final weighted score for the midterm exam

        // Final calculated grade
        public double TotalFinalsGrade { get; set; } // Summation of all weighted totals
        public double TotalFinalsGradeRounded { get; set; } // e.g., 74.50 = 75
        // The final grade point equivalent
        public double GradePointEquivalent { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
    }
    public class FinalsGradeUploadResult
    {
        public List<FinalsGrade> CalculatedGrades { get; set; } = new List<FinalsGrade>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
