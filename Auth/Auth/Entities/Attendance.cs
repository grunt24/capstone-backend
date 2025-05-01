using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class Attendance
    {
        [Key]
        public int AttendanceID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required, MaxLength(20)]
        public string? Status { get; set; }

        [ForeignKey("StudentID")]
        public Student? Student { get; set; }

        [ForeignKey("SubjectID")]
        public Subject? Subject { get; set; }
    }
}
