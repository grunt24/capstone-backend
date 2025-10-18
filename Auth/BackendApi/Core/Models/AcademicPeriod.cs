using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Core.Models
{
    public class AcademicPeriod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StartYear { get; set; }  // e.g., 2025

        [Required]
        public int EndYear { get; set; }    // e.g., 2026

        [Required]
        public string Semester { get; set; } = "";  // e.g., "First", "Second", "Summer"

        public bool IsCurrent { get; set; } = false;  // Only one should be true

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string AcademicYear => $"{StartYear}-{EndYear}";
    }


}
