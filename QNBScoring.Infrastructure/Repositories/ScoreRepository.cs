using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;

namespace QNBScoring.Infrastructure.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly QNBScoringDbContext _context;

        public ScoreRepository(QNBScoringDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Score score)
        {
            try
            {
                _context.Scores.Add(score);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception("Erreur lors de l’enregistrement du Score : " + inner, dbEx);
            }

        }


        public async Task<Score?> GetByIdAsync(int id)
        {
            return await _context.Scores.FindAsync(id);
        }

        public async Task<Score?> GetByIdWithDemandeAndClientAsync(int id)
        {
            return await _context.Scores
                .Include(s => s.Demande)
                .ThenInclude(d => d.Client)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<IEnumerable<Score>> GetAllAsync()
        {
            return await _context.Scores
                .Include(s => s.Demande)
                .ThenInclude(d => d.Client)
                .ToListAsync();
        }

    }
}
