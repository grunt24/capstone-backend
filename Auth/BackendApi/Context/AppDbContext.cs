using BackendApi.Core;
using BackendApi.Core.Models;
using BackendApi.Core.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<StudentModel> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<GradeItem> GradeItems { get; set; }
        //added
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<GradePointEquivalent> GradePointEquivalents { get; set; }
        public DbSet<MidtermGrade> MidtermGrades { get; set; }
        public DbSet<MidtermQuizList> MidtermQuizLists { get; set; }
        public DbSet<ClassStandingItem> ClassStanding { get; set; }
        public DbSet<GradeWeights> GradeWeights { get; set; }

    }


}
