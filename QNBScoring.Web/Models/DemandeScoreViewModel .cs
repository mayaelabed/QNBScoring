namespace QNBScoring.Web.Models
{
    public class DemandeScoreViewModel
    {
        public int DemandeId { get; set; }
        public string ClientNomComplet { get; set; } = string.Empty;
        public DateTime DateDemande { get; set; }
        public double? ScoreValeur { get; set; }
        public string? Decision { get; set; }
        public string? Commentaire { get; set; }
    }

}
