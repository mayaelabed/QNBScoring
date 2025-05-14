namespace QNBScoring.Core.DTOs;

public class ClientRequestDto
{
    public string ClientId { get; set; }
    public DateTime AccountOpened { get; set; }
    public int IncidentCount { get; set; }
    public decimal Solde { get; set; }

    // Résultat du scoring
    public int Score { get; set; }
    public string Decision { get; set; } // "Accepté" ou "Refusé"
}