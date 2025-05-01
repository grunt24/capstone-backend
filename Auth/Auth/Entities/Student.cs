using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Capstone.Core.Entities
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        public string? Fullname { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required, MaxLength(50)]
        public string? StudentNumber { get; set; }

        [Required]
        public int YearLevel { get; set; }

        public Course? Course { get; set; }
        [ForeignKey("CourseID")]
        public int CourseID { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}
