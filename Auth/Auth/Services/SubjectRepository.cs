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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _context.SubjectTBL.ToListAsync();
        }

        public async Task<Subject> CreateSubjectAsync(Subject subject)
        {
            if (subject == null)
            {
                throw new NullReferenceException("Subject cannot be null.");
            }

            await _context.SubjectTBL.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public Task<Subject> UpdateSubjectAsync(Subject subject)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSubjectAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Subject> GetSubjectByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
