namespace QNBScoring.Core.DTOs;

public class ClientScoreResultDto
{
    public string ClientId { get; set; }
    public int Score { get; set; }
    public string Decision { get; set; } // "Accepté" ou "Refusé"
    public string Justification { get; set; }
}
