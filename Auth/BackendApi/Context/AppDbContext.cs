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
        public DbSet<FinalsGrade> FinalsGrades { get; set; }
        public DbSet<QuizList> QuizLists { get; set; }
        public DbSet<ClassStandingItem> ClassStanding { get; set; }
        public DbSet<GradeWeights> GradeWeights { get; set; }
        public DbSet<AcademicPeriod> AcademicPeriods { get; set; }
        public DbSet<StudentEnrollment> StudentEnrollments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Midterm relationships
            modelBuilder.Entity<QuizList>()
                .HasOne(q => q.MidtermGrade)
                .WithMany(g => g.Quizzes)
                .HasForeignKey(q => q.MidtermGradeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassStandingItem>()
                .HasOne(c => c.MidtermGrade)
                .WithMany(g => g.ClassStandingItems)
                .HasForeignKey(c => c.MidtermGradeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Finals relationships
            modelBuilder.Entity<QuizList>()
                .HasOne(q => q.FinalsGrade)
                .WithMany(g => g.Quizzes)
                .HasForeignKey(q => q.FinalsGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassStandingItem>()
                .HasOne(c => c.FinalsGrade)
                .WithMany(g => g.ClassStandingItems)
                .HasForeignKey(c => c.FinalsGradeId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }


    }

}