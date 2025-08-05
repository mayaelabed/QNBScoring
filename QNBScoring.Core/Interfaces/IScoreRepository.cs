using QNBScoring.Core.Entities;

namespace QNBScoring.Core.Interfaces
{
    public interface IScoreRepository
    {
        Task<Score?> GetByIdAsync(int id);
        Task<Score?> GetByIdWithDemandeAndClientAsync(int id);
        Task AddAsync(Score score);
        Task<IEnumerable<Score>> GetAllAsync();
    }
}
