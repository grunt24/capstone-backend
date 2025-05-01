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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext context;

        public EnrollmentRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            if (enrollment == null)
            {
                throw new NullReferenceException("Subject cannot be null.");
            }

            await context.EnrollmentTBL.AddAsync(enrollment);
            await context.SaveChangesAsync();
            return enrollment;
        }

        public Task<bool> DeleteEnrollmentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await context.EnrollmentTBL.ToListAsync();
        }

        public Task<Enrollment> GetEnrollmentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> UpdateEnrollmentAsync(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }
    }
}
