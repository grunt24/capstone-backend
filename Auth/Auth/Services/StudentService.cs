using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Services
{
    public class StudentService : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.StudentTBL
                .Include(s=>s.Course)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            var existingUser = await _context.Users.FindAsync(student.UserID);
            var existingCourse = await _context.CourseTBL.FindAsync(student.CourseID);

            if (existingUser == null || existingCourse == null)
                throw new InvalidOperationException("User or Course does not exist.");

            student.User = existingUser;
            student.Course = existingCourse;

            await _context.StudentTBL.AddAsync(student);
            await _context.SaveChangesAsync();

            return student;
        }


        public Task<bool> DeleteStudentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Student> GetStudentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Student> UpdateStudentAsync(Student student)
        {
            throw new NotImplementedException();
        }
    }
}
