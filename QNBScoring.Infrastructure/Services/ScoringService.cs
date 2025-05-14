using QNBScoring.Core.DTOs;
using QNBScoring.Core.Interfaces;

namespace QNBScoring.Infrastructure.Services;

public class ScoringService : IScoringService
{
    public IEnumerable<ClientScoreResultDto> Score(IEnumerable<ClientRequestDto> requests)
    {
        var results = new List<ClientScoreResultDto>();

        foreach (var req in requests)
        {
            int score = 0;
            string justification = "";

            if (req.IncidentCount == 0)
            {
                score += 50;
            }
            else if (req.IncidentCount <= 2)
            {
                score += 30;
                justification += "Quelques incidents détectés. ";
            }
            else
            {
                justification += "Trop d'incidents. ";
            }

            var accountAge = (DateTime.Now - req.AccountOpened).TotalDays / 365;
            if (accountAge >= 5)
            {
                score += 30;
            }
            else if (accountAge >= 2)
            {
                score += 20;
                justification += "Ancienneté moyenne. ";
            }
            else
            {
                justification += "Compte récent. ";
            }

            string decision = score >= 60 ? "Accepté" : "Refusé";

            results.Add(new ClientScoreResultDto
            {
                ClientId = req.ClientId,
                Score = score,
                Decision = decision,
                Justification = justification
            });
        }

        return results;
    }

    IEnumerable<ClientRequestDto> IScoringService.Score(IEnumerable<ClientRequestDto> requests)
    {
        throw new NotImplementedException();
    }
}
