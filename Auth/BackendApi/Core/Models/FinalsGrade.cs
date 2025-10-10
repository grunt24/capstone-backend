using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Core.Models
{
    public class FinalsGrade
    {
        public int Id { get; set; }

        public string? StudentNumber { get; set; }
        [ForeignKey("User")]
        public int StudentId { get; set; }
        public StudentModel? User { get; set; }

        [ForeignKey("SubjectId")]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // Final Grades will use the same QuizList and ClassStandingItem tables
        public ICollection<QuizList> Quizzes { get; set; } = new List<QuizList>();
        public int TotalQuizScore { get; set; }
        public decimal QuizPG { get; set; }
        public decimal QuizWeightedTotal { get; set; }
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
}
