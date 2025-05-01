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
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _context.TeacherTBL.ToListAsync();
        }

        public async Task<Teacher> CreateTeacherAsync(Teacher teacher)
        {
            if (teacher == null)
            {
                throw new NullReferenceException("Teacher cannot be null.");
            }

            await _context.TeacherTBL.AddAsync(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public Task<bool> DeleteTeacherAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Teacher> GetTeacherByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Teacher> UpdateTeacherAsync(Teacher teacher)
        {
            throw new NotImplementedException();
        }
    }
}
