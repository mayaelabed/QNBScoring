using QNBScoring.Core.Entities;

namespace QNBScoring.Core.Interfaces
{
    public interface ITransactionBancaireRepository
    {
        Task<IEnumerable<TransactionBancaire>> GetAllAsync();
        Task AddAsync(TransactionBancaire transaction);
    }
}
