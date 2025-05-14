namespace QNBScoring.Core.Models
{
    public class ClientScore
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Decision { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
    }
}
