using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Entities
{
    public class UtilisateurApp
    {
        public int Id { get; set; }
        public string NomUtilisateur { get; set; } = string.Empty; // ex: QNB\\john.doe
        public string Role { get; set; } = string.Empty; // "Agent" or "Decideur"
    }

}
