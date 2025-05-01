using Capstone.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Repositories
{
    public interface IAssessmentRepository
    {
        Task<IEnumerable<Assessment>> GetAllAsync();
        Task<Assessment> CreateAsync(Assessment assessment);
        Task<Assessment> UpdateAsync(Assessment assessment);
        Task<bool> DeleteAsync(int id);
        Task<Assessment> GetByIdAsync(int id);
    }
}
