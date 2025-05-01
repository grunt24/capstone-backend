using Capstone.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Core;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Assessment> AssessmentsTBL { get; set; }
    public DbSet<Attendance> AttendanceTBL { get; set; }
    public DbSet<Course> CourseTBL { get; set; }
    public DbSet<Department> DepartmentTBL { get; set; }
    public DbSet<Enrollment> EnrollmentTBL { get; set; }
    public DbSet<Score> ScoreTBL { get; set; }
    public DbSet<Student> StudentTBL { get; set; }
    public DbSet<Subject> SubjectTBL { get; set; }
    public DbSet<Teacher> TeacherTBL { get; set; }
    public DbSet<TeacherSubject> TeacherSubjectsMapping { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Score>()
            .HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentID)
            .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete for Student

        modelBuilder.Entity<Score>()
            .HasOne(s => s.Assessment)
            .WithMany()
            .HasForeignKey(s => s.AssessmentID)
            .OnDelete(DeleteBehavior.Cascade); // Keeps cascade delete for Assessment

        base.OnModelCreating(modelBuilder);
    }


}