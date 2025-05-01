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
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext context;

        public ScoreRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<Score> CreateScoreAsync(Score score)
        {
            if (score == null)
            {
                throw new NullReferenceException("Score cannot be null");
            }

            await context.ScoreTBL.AddAsync(score);
            await context.SaveChangesAsync();

            return score;
        }

        public Task<bool> DeleteScoreAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Score>> GetAllScoresAsync()
        {
            return await context.ScoreTBL.ToListAsync();
        }

        public Task<Score> GetScoreByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Score> UpdateScoreAsync(Score score)
        {
            throw new NotImplementedException();
        }
    }
}
