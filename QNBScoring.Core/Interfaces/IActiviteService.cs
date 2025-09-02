using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Interfaces
{
    public interface IActiviteService
    {
        Task EnregistrerAsync(string action, string utilisateur, string status);
        Task<List<Activities>> ObtenirDernieresActivitesAsync(int nombre = 5);
    }
}
