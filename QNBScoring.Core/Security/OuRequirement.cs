using Microsoft.AspNetCore.Authorization;

namespace QNBScoring.Core.Security
{
    public class OuRequirement : IAuthorizationRequirement
    {
        public string RequiredOu { get; }
        public OuRequirement(string requiredOu) => RequiredOu = requiredOu;
    }
}