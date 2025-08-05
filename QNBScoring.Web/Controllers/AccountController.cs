using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Web.Models;
using System.Security.Claims;

namespace QNBScoring.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILdapService _ldapService;
        private readonly ISessionLogger _logger;
        private readonly QNBScoringDbContext _context;

        public AccountController(ILdapService ldapService, ISessionLogger logger, QNBScoringDbContext context)
        {
            _ldapService = ldapService;
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            var username = User.Identity?.Name ?? Environment.UserName;
            return View(new LoginViewModel { NomUtilisateur = username });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ldapOk = _ldapService.VerifierMotDePasse(model.NomUtilisateur, model.MotDePasse);
            if (!ldapOk)
            {
                ModelState.AddModelError("", "Mot de passe incorrect.");
                return View(model);
            }

            var utilisateur = await _context.UtilisateursApp
                .FirstOrDefaultAsync(u => u.NomUtilisateur == model.NomUtilisateur);

            if (utilisateur == null)
            {
                ModelState.AddModelError("", "Utilisateur inconnu.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, utilisateur.NomUtilisateur),
                new Claim(ClaimTypes.Role, utilisateur.Role)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", principal);
            await _logger.EnregistrerSessionAsync(utilisateur.NomUtilisateur, utilisateur.Role);

            return utilisateur.Role == "Decideur"
                ? RedirectToAction("Dashboard", "Home")
                : RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
