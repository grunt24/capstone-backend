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
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext context;

        public DepartmentRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<Department> CreateAsync(Department department)
        {
            if (department == null)
            {
                throw new NullReferenceException("Subject cannot be null.");
            }

            await context.DepartmentTBL.AddAsync(department);
            await context.SaveChangesAsync();
            return department;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await context.DepartmentTBL.ToListAsync();
        }

        public Task<Department> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Department> UpdateAsync(Department department)
        {
            throw new NotImplementedException();
        }
    }
}
