using BackendApi.Core.Models.Dto;
using BackendApi.IRepositories;
using System.ComponentModel.DataAnnotations;

namespace BackendApi.Core.Models
{
    public enum UserRole
    {
        Superadmin = 1,
        Admin,
        Student,
        Teacher,
    }
    public class StudentModel
    {
        [Key]
        public int Id { get; set; }
        public string? StudentNumber { get; set; } = null;

        [MaxLength(30)]
        public string Username { get; set; }


        [MaxLength(100)]
        public string Password { get; set; }
        public string? Fullname { get; set; }
        public string? Department { get; set; }
        public string? YearLevel { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;

        // Navigation property for student-subject relation
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        //Added - B2
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? LatestTransaction { get; set; }
        public ICollection<UserEvent> UserEvents { get; set; }
    }
}
