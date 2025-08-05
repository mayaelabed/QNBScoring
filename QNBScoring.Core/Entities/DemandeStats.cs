public class DemandeStats
{
    public int TotalDemandes { get; set; }
    public int DemandesScorees { get; set; }
    public int TauxAcceptation { get; set; }
    public string DerniereAnalyse { get; set; }
    public int Acceptees { get; set; }
    public int Refusees { get; set; }
    public int TotalActions { get; set; }
    public string? DerniereAction { get; set; }
    public DateTime? DateDerniereAction { get; set; }
    public int NonScorees { get; set; }
}