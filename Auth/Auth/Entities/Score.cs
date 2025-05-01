using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class Score
    {
        [Key]
        public int ScoreID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int AssessmentID { get; set; }

        [Required]
        public int ScoreValue { get; set; }

        [MaxLength(50)]
        public string? Remarks { get; set; }

        [ForeignKey("StudentID")]
        public Student? Student { get; set; }

        [ForeignKey("AssessmentID")]
        public Assessment? Assessment { get; set; }
    }
}
