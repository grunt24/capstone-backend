using System.ComponentModel.DataAnnotations;

namespace BackendApi.Core.Models
{
    public class StudentEnrollment
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to StudentModel
        [Required]
        public int StudentId { get; set; }
        public StudentModel Student { get; set; }

        // Foreign Key to AcademicPeriod
        [Required]
        public int AcademicPeriodId { get; set; }
        public AcademicPeriod AcademicPeriod { get; set; }

        // Tracks if the student is currently enrolled in this period
        public bool IsEnrolled { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
