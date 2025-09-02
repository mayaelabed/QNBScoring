using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Entities
{
    public class Activities
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string Utilisateur { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } // success, danger, info
    }

}
