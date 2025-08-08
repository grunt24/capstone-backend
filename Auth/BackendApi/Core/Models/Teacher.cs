using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendApi.IRepositories;

namespace BackendApi.Core.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        public string? Fullname { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public StudentModel? User { get; set; }
        // Navigation
        public ICollection<Subject>? Subjects { get; set; }
    }
}
