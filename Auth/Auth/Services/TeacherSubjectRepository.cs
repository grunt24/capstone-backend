using Capstone.Core.Entities;
using Capstone.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Services
{
    public class TeacherSubjectRepository : ITeacherSubjectRepository
    {
        private readonly AppDbContext context;

        public TeacherSubjectRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<TeacherSubject> CreateAsync(TeacherSubject teacherSubject)
        {
            if (teacherSubject == null)
            {
                throw new NullReferenceException("Teacher Subjects cannot be null.");
            }

            await context.TeacherSubjectsMapping.AddAsync(teacherSubject);
            await context.SaveChangesAsync();   
            return teacherSubject;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TeacherSubject>> GetAllAsync()
        {
            return await context.TeacherSubjectsMapping.ToListAsync();  
        }

        public Task<TeacherSubject> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TeacherSubject> UpdateAsync(TeacherSubject teacherSubject)
        {
            throw new NotImplementedException();
        }
    }
}
