using BackendApi.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Core
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        // The final grade given by the teacher
        public double? MainGrade { get; set; }

        // The calculated grade based on quizzes and exams
        public double? CalculatedGrade { get; set; }

        // Foreign key to the StudentSubject entry
        [ForeignKey("StudentSubject")]
        public int StudentSubjectId { get; set; }

        public StudentSubject? StudentSubject { get; set; }

        // Collection of individual scores (quizzes, exams)
        public ICollection<GradeItem> GradeItems { get; set; } = new List<GradeItem>();
    }

    public class GradeItem
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }

        // Foreign key to the Grade record
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        public Grade? Grade { get; set; }
    }
}