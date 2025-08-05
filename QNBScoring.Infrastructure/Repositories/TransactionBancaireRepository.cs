using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;

namespace QNBScoring.Infrastructure.Repositories
{
    public class TransactionBancaireRepository : ITransactionBancaireRepository
    {
        private readonly QNBScoringDbContext _context;

        // Remove the ITransactionBancaireRepository parameter
        public TransactionBancaireRepository(QNBScoringDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TransactionBancaire transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransactionBancaire>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }
    }
}