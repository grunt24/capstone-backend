using Capstone.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Core.Repositories
{
    public interface IScoreRepository
    {
        Task<IEnumerable<Score>> GetAllScoresAsync();
        Task<Score> CreateScoreAsync(Score score);
        Task<Score> UpdateScoreAsync(Score score);
        Task<bool> DeleteScoreAsync(int id);
        Task<Score> GetScoreByIdAsync(int id);
    }
}
