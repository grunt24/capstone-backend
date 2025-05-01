using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }

        [Required, MaxLength(20)]
        public string? SubjectCode { get; set; }

        [Required, MaxLength(150)]
        public string? SubjectName { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Credits { get; set; }

        public ICollection<TeacherSubject>? TeacherSubjects { get; set; }
    }
}
