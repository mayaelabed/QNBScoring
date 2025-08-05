using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;

namespace QNBScoring.Infrastructure.Repositories
{
    public class DemandeChequierRepository : IDemandeChequierRepository
    {
        private readonly QNBScoringDbContext _context;

        public DemandeChequierRepository(QNBScoringDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DemandeChequier demande)
        {
            _context.Demandes.Add(demande);
            await _context.SaveChangesAsync();
        }

        public async Task<DemandeChequier?> GetByIdAsync(int id)
        {
            return await _context.Demandes.FindAsync(id);
        }

        public async Task<DemandeChequier?> GetByIdWithClientAsync(int id)
        {
            return await _context.Demandes
                .Include(d => d.Client)
                .Include(d => d.Score)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DemandeChequier>> GetAllWithClientsAsync()
        {
            return await _context.Demandes
                .Include(d => d.Client)
                .ToListAsync();
        }

        public async Task<IEnumerable<DemandeChequier>> GetAllAsync()
        {
            return await _context.Demandes.ToListAsync();
        }
    }
}
