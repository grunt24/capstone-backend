using Capstone.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Repositories
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance> CreateAsync(Attendance attendance);
        Task<Attendance> UpdateAsync(Attendance attendance);
        Task<bool> DeleteAsync(int id);
        Task<Attendance> GetByIdAsync(int id);
    }
}
