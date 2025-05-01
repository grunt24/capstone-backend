using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class Assessment
    {
        [Key]
        public int AssessmentID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required]
        public int TeacherID { get; set; }

        [Required, MaxLength(150)]
        public string? Title { get; set; }

        [Required, MaxLength(20)]
        public string? Type { get; set; }

        [Required]
        public int MaxScore { get; set; }

        [Required]
        public DateTime AssessmentDate { get; set; }

        [ForeignKey("SubjectID")]
        public Subject? Subject { get; set; }

        [ForeignKey("TeacherID")]
        public Teacher? Teacher { get; set; }
    }
}
