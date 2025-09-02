using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Entities
{
    public class Score
    {
        public int Id { get; set; }
        public double Valeur { get; set; }
        public string Decision { get; set; } = string.Empty;
        public string Commentaire { get; set; } = string.Empty;
         public DateTime DateCreation {  get; set; }

        //public int ClientId { get; set; }
        //public Client Client { get; set; } = null!;

        public int DemandeChequierId { get; set; }
        public DemandeChequier Demande { get; set; } = null!;


    }
}
