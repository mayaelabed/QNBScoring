using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Interfaces
{
    public interface ILdapService
    {
        bool VerifierMotDePasse(string username, string password);
    }
}
