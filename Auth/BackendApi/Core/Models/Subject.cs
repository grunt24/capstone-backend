using BackendApi.IRepositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackendApi.Core.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string? SubjectCode { get; set; }

        [Required, MaxLength(150)]
        public string? SubjectName { get; set; }
        public string? Description { get; set; }

        [Required]
        public int Credits { get; set; }

        // Navigation properties for Teacher and Students
        // Foreign key
        public int? TeacherId { get; set; }
        [JsonIgnore]
        public Teacher? Teacher { get; set; }
        [JsonIgnore]
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}
