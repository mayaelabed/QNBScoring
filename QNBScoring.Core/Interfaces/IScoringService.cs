using QNBScoring.Core.DTOs;

namespace QNBScoring.Core.Interfaces;

public interface IScoringService
{
    IEnumerable<ClientRequestDto> Score(IEnumerable<ClientRequestDto> requests);
}
