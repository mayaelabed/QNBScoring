using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.Security;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace QNBScoring.Infrastructure.Security
{
    public class OuRequirementHandler : AuthorizationHandler<OuRequirement>
    {
        private readonly IAdService _adService;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

        public OuRequirementHandler(IAdService adService, IMemoryCache cache)
        {
            _adService = adService;
            _cache = cache;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OuRequirement requirement)
        {
            var windowsIdentity = context.User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var sam = windowsIdentity.Name.Contains("\\") ? windowsIdentity.Name.Split('\\').Last() : windowsIdentity.Name;
            var cacheKey = $"ous:{sam}";
            if (!_cache.TryGetValue(cacheKey, out System.Collections.Generic.List<string> ous))
            {
                ous = _adService.GetUserOrganizationalUnits(sam);
                _cache.Set(cacheKey, ous, _cacheTtl);
            }

            if (ous.Any(ou => string.Equals(ou, requirement.RequiredOu, StringComparison.OrdinalIgnoreCase)))
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}