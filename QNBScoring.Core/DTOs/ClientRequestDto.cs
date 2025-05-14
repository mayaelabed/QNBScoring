namespace QNBScoring.Core.DTOs;

public class ClientRequestDto
{
<<<<<<< HEAD
    public string ClientId { get; set; }
=======
    public required string ClientId { get; set; }
    public required string ClientName { get; set; }

>>>>>>> 42f6f51 (additionnal fuctionnality)
    public DateTime AccountOpened { get; set; }
    public int IncidentCount { get; set; }
    public decimal Solde { get; set; }

<<<<<<< HEAD
    // Résultat du scoring
    public int Score { get; set; }
    public string Decision { get; set; } // "Accepté" ou "Refusé"
=======
    public int Score { get; set; }
    public required string Decision { get; set; } // "Accepté" ou "Refusé"
>>>>>>> 42f6f51 (additionnal fuctionnality)
}