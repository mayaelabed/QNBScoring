using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QNBScoring.Core.Interfaces;
using System;

namespace QNBScoring.Web.Controllers
{
    [ApiController]
    [Route("api/testad")]
    [AllowAnonymous] // Permettre l'accès sans auth pour les tests
    public class TestAdController : ControllerBase
    {
        private readonly IAdService _adService;

        public TestAdController(IAdService adService)
        {
            _adService = adService;
        }

        [HttpGet("user/{username}")]
        public IActionResult TestUser(string username)
        {
            try
            {
                var dn = _adService.GetUserDistinguishedName(username);
                var ous = _adService.GetUserOrganizationalUnits(username);

                return Ok(new
                {
                    Username = username,
                    DistinguishedName = dn,
                    OrganizationalUnits = ous,
                    Message = "🔧 Test réalisé avec le service MOCK",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Details = ex.StackTrace
                });
            }
        }

        [HttpGet("status")]
        public IActionResult ServiceStatus()
        {
            return Ok(new
            {
                ServiceType = _adService.GetType().Name,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Status = "OK",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}