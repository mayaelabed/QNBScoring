using QNBScoring.Core.Entities;

namespace QNBScoring.Core.Interfaces
{
    public interface IDemandeChequierRepository
    {
        Task<DemandeChequier?> GetByIdAsync(int id);
        Task<DemandeChequier?> GetByIdWithClientAsync(int id);
        Task<IEnumerable<DemandeChequier>> GetAllWithClientsAsync();
        Task<IEnumerable<DemandeChequier>> GetAllAsync();
        Task AddAsync(DemandeChequier demande);
    }
}
