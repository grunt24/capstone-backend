using Capstone.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Repositories
{
    public interface ITeacherSubjectRepository
    {
        Task<IEnumerable<TeacherSubject>> GetAllAsync();
        Task<TeacherSubject> CreateAsync(TeacherSubject teacherSubject);
        Task<TeacherSubject> UpdateAsync(TeacherSubject teacherSubject);
        Task<bool> DeleteAsync(int id);
        Task<TeacherSubject> GetByIdAsync(int id);
    }
}
