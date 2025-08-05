using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace QNBScoring.Infrastructure.Services
{
    public class ActiviteService : IActiviteService
    {
        private readonly QNBScoringDbContext _context;

        public ActiviteService(QNBScoringDbContext context)
        {
            _context = context;
        }

        public async Task EnregistrerAsync(string action, string utilisateur, string status)
        {
            var activite = new Activite
            {
                Action = action,
                Utilisateur = utilisateur,
                Date = DateTime.Now,
                Status = status
            };

            _context.Activites.Add(activite);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Activite>> ObtenirDernieresActivitesAsync(int nombre = 5)
        {
            return await _context.Activites
                .OrderByDescending(a => a.Date)
                .Take(nombre)
                .ToListAsync();
        }
    }
}
