using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Core.Models
{
    public class MidtermGrade
    {
        [ForeignKey("User")]
        public int StudentId { get; set; }
        public StudentModel? User { get; set; }
        [ForeignKey("SubjectId")]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int Id { get; set; }
        public ICollection<QuizList> Quizzes { get; set; } = new List<QuizList>();
        public int TotalQuizScore { get; set; } // TS
        public decimal QuizPG { get; set; } // Percentage Grade for quizzes
        public decimal QuizWeightedTotal { get; set; } // The final weighted score for quizzes

        // Class Standing (25% of the grade)
        public int RecitationScore { get; set; }
        public int AttendanceScore { get; set; }
        public ICollection<ClassStandingItem> ClassStandingItems { get; set; } = new List<ClassStandingItem>();
        public int ClassStandingTotalScore { get; set; } // TS
        public decimal ClassStandingAverage { get; set; }
        public decimal ClassStandingPG { get; set; } // Percentage Grade for class standing
        public decimal ClassStandingWeightedTotal { get; set; } // The final weighted score for class standing


        // Speak English Policy (SEP) (5% of the grade)
        public int SEPScore { get; set; }
        public decimal SEPPG { get; set; }
        public decimal SEPWeightedTotal { get; set; } // The final weighted score for SEP

        // Project (10% of the grade)
        public int ProjectScore { get; set; }
        public decimal ProjectPG { get; set; }
        public decimal ProjectWeightedTotal { get; set; } // The final weighted score for the project

        // Prelim and Midterm Exam (30% of the grade)
        public int PrelimScore { get; set; }
        public int PrelimTotal { get; set; }
        public int MidtermScore { get; set; }
        public int MidtermTotal { get; set; }
        public decimal TotalScorePerlimAndMidterm { get; set; }
        public decimal OverallPrelimAndMidterm { get; set; }
        public decimal CombinedPrelimMidtermAverage { get; set; } // Average of prelim & midterm scores
        public decimal MidtermPG { get; set; } // Percentage Grade for the combined exam
        public decimal MidtermWeightedTotal { get; set; } // The final weighted score for the midterm exam

        // Final calculated grade
        public double TotalMidtermGrade { get; set; } // Summation of all weighted totals
        public double TotalMidtermGradeRounded { get; set; } // e.g., 74.50 = 75

        // The final grade point equivalent
        public double GradePointEquivalent { get; set; }
        // New properties for Semester and Academic Year
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }

    }
    public class QuizList
    {
        public int Id { get; set; }
        public string? Label { get; set; } // e.g., "Quiz 1"
        public int? QuizScore { get; set; }
        public int? TotalQuizScore { get; set; }
    }
    public class ClassStandingItem
    {
        public int Id { get; set; }
        public string? Label { get; set; } // e.g., "SW/ASS/GRP WRK 1"
        public int? Score { get; set; }
        public int? Total { get; set; }
    }
}
