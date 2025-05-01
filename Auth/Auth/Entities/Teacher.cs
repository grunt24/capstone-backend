using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class Teacher
    {
        [Key]
        public int TeacherID { get; set; }

        public string? Fullname { get; set; }


        [Required]
        public int UserID { get; set; }

        [MaxLength(100)]
        public Department? Department { get; set; }
        [ForeignKey("DepartmentID")]
        public int DepartmentID { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        public ICollection<TeacherSubject>? TeacherSubjects { get; set; }
    }
}
