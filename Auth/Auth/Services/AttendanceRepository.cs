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
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext context;

        public AttendanceRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            if (attendance == null)
            {
                throw new NullReferenceException("Assessment cannot be null.");
            }

            await context.AddAsync(attendance);
            await context.SaveChangesAsync();
            return attendance;
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await context.AttendanceTBL.ToListAsync();
        }

        public Task<Attendance> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Attendance> UpdateAsync(Attendance attendance)
        {
            throw new NotImplementedException();
        }
    }
}
