using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Entities
{
    public class TeacherSubject
    {
        [Key]
        public int TeacherSubjectID { get; set; }

        [Required]
        public int TeacherID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [ForeignKey("TeacherID")]
        public Teacher? Teacher { get; set; }

        [ForeignKey("SubjectID")]
        public Subject? Subject { get; set; }
    }
}
