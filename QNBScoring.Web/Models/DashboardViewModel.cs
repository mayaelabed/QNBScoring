using System.Collections.Generic;

namespace QNBScoring.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalDemandes { get; set; }
        public int DemandesApprouvees { get; set; }
        public int DemandesRejetees { get; set; }

        public List<RecentActivity> DernieresActivites { get; set; }
    }

    public class RecentActivity
    {
        public string Action { get; set; }
        public string Utilisateur { get; set; }
        public string Temps { get; set; }
        public string Status { get; set; } // success, danger, info
    }

}
