using QNBScoring.Core.Entities;

namespace QNBScoring.Core.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(int id);
        Task<Client?> GetByAccountNoAsync(string accountNo);
        Task<IEnumerable<Client>> GetAllAsync();
        Task AddAsync(Client client);
        Task<Client> GetClientByAccountNo(string accountNo);
        Task UpdateAsync(Client client);

    }
}
