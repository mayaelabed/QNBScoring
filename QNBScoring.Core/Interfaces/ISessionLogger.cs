using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Interfaces
{
    public interface ISessionLogger
    {
        Task EnregistrerSessionAsync(string userName, string role);
    }
}
