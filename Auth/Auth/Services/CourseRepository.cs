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
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Course> CreateAsync(Course course)
        {
            if (course == null)
            {
                throw new NullReferenceException("Course cannot be null.");
            }
            
            await _context.CourseTBL.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.CourseTBL.ToListAsync();

        }

        public Task<Course> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Course> UpdateAsync(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
