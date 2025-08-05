using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;

namespace QNBScoring.Infrastructure.Services
{
    public class SessionLogger : ISessionLogger
    {
        private readonly QNBScoringDbContext _context;

        public SessionLogger(QNBScoringDbContext context)
        {
            _context = context;
        }

        public async Task EnregistrerSessionAsync(string userName, string role)
        {
            var session = new SessionUtilisateur
            {
                NomUtilisateur = userName,
                Role = role,
                DateConnexion = DateTime.Now
            };

            _context.SessionsUtilisateurs.Add(session);
            await _context.SaveChangesAsync();
        }
    }
}
