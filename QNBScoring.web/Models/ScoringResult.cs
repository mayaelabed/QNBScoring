namespace QNBScoring.Core.Models
{
    public class ScoringResult
    {
        public int TotalRequests { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal ApprovalRate => TotalRequests > 0 ? (decimal)ApprovedCount / TotalRequests * 100 : 0;
        public IEnumerable<ClientScoreDetail> ScoreDetails { get; set; } = new List<ClientScoreDetail>();
    }

    public class ClientScoreDetail
    {
        public string ClientId { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Decision { get; set; } = string.Empty;
        public string Reasons { get; set; } = string.Empty;
    }
}