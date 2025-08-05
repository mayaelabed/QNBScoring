using QNBScoring.Core.Entities;

namespace QNBScoring.Core.Interfaces
{
    public interface IScoringService
    {
        Task<Score> CalculerScoreAsync(Client client, DemandeChequier demande);
    }
}
