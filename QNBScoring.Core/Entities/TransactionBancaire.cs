using QNBScoring.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Transactions")]
public class TransactionBancaire
{
    public int Id { get; set; }

    [ForeignKey(nameof(Client))]
    public required string AccountNo { get; set; }

    public DateTime TranDate { get; set; }
    public DateTime ValueDate { get; set; }
    public required string TranType { get; set; }
    public decimal TranAmount { get; set; }
    public decimal Balance { get; set; }
    public required string TranCode { get; set; }
    public required string TranDescEng { get; set; }
    public required string ReconcileRef { get; set; }
    public string? OperationType { get; set; } // Pour stocker le type d'opération (INWARD CLEARING, etc.)
    public string? Description { get; set; } // Pour stocker la description de la transaction
    public required string Narrative1 { get; set; }
    public string? Narrative2 { get; set; }
    public string? Narrative3 { get; set; }
    public string? Narrative4 { get; set; }
    public string? PostGrpUserId { get; set; }

    // 🔗 Navigation property vers Client
    public Client? Client { get; set; }
}
