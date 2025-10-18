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
            // MidtermGrade -> QuizList & ClassStandingItem cascade
            modelBuilder.Entity<QuizList>()
                .HasOne<MidtermGrade>()
                .WithMany(g => g.Quizzes)
                .HasForeignKey("MidtermGradeId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassStandingItem>()
                .HasOne<MidtermGrade>()
                .WithMany(g => g.ClassStandingItems)
                .HasForeignKey("MidtermGradeId")
                .OnDelete(DeleteBehavior.Cascade);

            // FinalsGrade -> QuizList & ClassStandingItem cascade
            modelBuilder.Entity<QuizList>()
                .HasOne<FinalsGrade>()
                .WithMany(g => g.Quizzes)
                .HasForeignKey("FinalsGradeId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassStandingItem>()
                .HasOne<FinalsGrade>()
                .WithMany(g => g.ClassStandingItems)
                .HasForeignKey("FinalsGradeId")
                .OnDelete(DeleteBehavior.Cascade);

            // MidtermGrade -> QuizList & ClassStandingItem
            modelBuilder.Entity<QuizList>()
                .HasOne<MidtermGrade>()
                .WithMany(g => g.Quizzes)
                .HasForeignKey("MidtermGradeId")
                .OnDelete(DeleteBehavior.Cascade); // Safe: only one cascade path

            modelBuilder.Entity<ClassStandingItem>()
                .HasOne<MidtermGrade>()
                .WithMany(g => g.ClassStandingItems)
                .HasForeignKey("MidtermGradeId")
                .OnDelete(DeleteBehavior.Cascade); // Safe

            // FinalsGrade -> QuizList & ClassStandingItem
            modelBuilder.Entity<QuizList>()
                .HasOne<FinalsGrade>()
                .WithMany(g => g.Quizzes)
                .HasForeignKey("FinalsGradeId")
                .OnDelete(DeleteBehavior.NoAction); // Prevents multiple cascade path error

            modelBuilder.Entity<ClassStandingItem>()
                .HasOne<FinalsGrade>()
                .WithMany(g => g.ClassStandingItems)
                .HasForeignKey("FinalsGradeId")
                .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }


    }

}