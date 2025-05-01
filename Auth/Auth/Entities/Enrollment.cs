using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required, MaxLength(20)]
        public string? Semester { get; set; }

        [Required, MaxLength(20)]
        public string? SchoolYear { get; set; }

        [Required, MaxLength(20)]
        public string? Status { get; set; }

        [ForeignKey("StudentID")]
        public Student? Student { get; set; }

        [ForeignKey("SubjectID")]
        public Subject? Subject { get; set; }
    }
}
