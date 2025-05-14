using QNBScoring.Core.DTOs;

namespace QNBScoring.Core.Interfaces;

public interface IScoringService
{
<<<<<<< HEAD
    IEnumerable<ClientRequestDto> Score(IEnumerable<ClientRequestDto> requests);
=======
    IEnumerable<ClientScoreResultDto> Score(IEnumerable<ClientRequestDto> requests);
    string? GetLastResults(); // signature uniquement, pas d'implémentation
>>>>>>> 42f6f51 (additionnal fuctionnality)
}
