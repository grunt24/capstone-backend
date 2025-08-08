using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendApi.Core.Models;

namespace BackendApi.Core.Models
{
    public class StudentSubject
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int StudentID { get; set; }

        public StudentModel? User { get; set; }

        [Required]
        [ForeignKey("Subject")]
        public int SubjectID { get; set; }

        public Subject? Subject { get; set; }
    }
}
