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
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly AppDbContext context;

        public AssessmentRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<Assessment> CreateAsync(Assessment assessment)
        {
            if (assessment == null)
            {
                throw new NullReferenceException("Assessment cannot be null.");
            }   

            await context.AddAsync(assessment);
            await context.SaveChangesAsync();
            return assessment;

        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Assessment>> GetAllAsync()
        {
            return await context.AssessmentsTBL.ToListAsync();
        }

        public Task<Assessment> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Assessment> UpdateAsync(Assessment assessment)
        {
            throw new NotImplementedException();
        }
    }
}
