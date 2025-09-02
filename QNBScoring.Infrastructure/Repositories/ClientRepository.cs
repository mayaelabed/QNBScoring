using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;

namespace QNBScoring.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly QNBScoringDbContext _context;

        public ClientRepository(QNBScoringDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<Client?> GetByAccountNoAsync(string accountNo)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.AccountNo == accountNo);
        }

        public Task<Client> GetClientByAccountNo(string accountNo)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<TransactionBancaire>> GetByClientAccountNoAsync(string accountNo)
        {
            return await _context.Transactions
                .Where(t => t.AccountNo == accountNo)
                .ToListAsync();
        }
        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
    }
}
