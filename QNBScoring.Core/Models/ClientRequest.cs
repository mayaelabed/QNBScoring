using System;

namespace QNBScoring.Core.Models
{
    public class ClientRequest
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public DateTime AccountOpened { get; set; }
        public int IncidentCount { get; set; }
        public decimal Solde { get; set; }
    }
}
