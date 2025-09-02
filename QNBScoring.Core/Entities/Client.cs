using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string CIN { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime DateEntreeEnRelation { get; set; }
        public string Profession { get; set; } = string.Empty;
        public string ClassificationSED { get; set; } = string.Empty;
        public bool ADesIncidents { get; set; }
        public string AccountNo { get; set; } = string.Empty;
        public int Telephone { get; set; }
        public DateTime? LastTransactionImport { get; set; }


        public ICollection<DemandeChequier> Demandes { get; set; } = [];
        public ICollection<TransactionBancaire> Transactions { get; set; } = new List<TransactionBancaire>();
    }
}
