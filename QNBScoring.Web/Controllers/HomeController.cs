using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Filters;
using QNBScoring.Infrastructure.Services;
using QNBScoring.Web.Models;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace QNBScoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActiviteService _activiteService;
        private readonly QNBScoringDbContext _context;
        private readonly IAdService _adService;

        public HomeController(
            QNBScoringDbContext context,
            IActiviteService activiteService,
            IAdService adService)
        {
            _context = context;
            _activiteService = activiteService;
            _adService = adService;
        }

        public async Task<IActionResult> Index()
        {
            // R�cup�rer infos utilisateur
            var identity = HttpContext.User.Identity as WindowsIdentity;
            var username = identity?.Name ?? "Unknown";
            var sam = username.Contains("\\") ? username.Split('\\').Last() : username;

            // ?? REDIRECTION CONDITIONNELLE
            if (sam.Equals("maya", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard");
            }
            else if (sam.Equals("testuser", StringComparison.OrdinalIgnoreCase))
            {
                // Reste sur Index pour testuser
                return await ShowIndex(sam);
            }

            // Par d�faut pour les autres utilisateurs
            return await ShowIndex(sam);
        }

        // ?? M�THODE PRIV�E POUR AFFICHER L'INDEX
        private async Task<IActionResult> ShowIndex(string samAccountName)
        {
            var ous = _adService.GetUserOrganizationalUnits(samAccountName);

            // Stats des demandes
            var total = await _context.Demandes.CountAsync();
            var approuvees = await _context.Demandes
                .Include(d => d.Score)
                .CountAsync(d => d.Score != null && d.Score.Decision == "Accept�");

            var rejetees = await _context.Demandes
                .Include(d => d.Score)
                .CountAsync(d => d.Score != null && d.Score.Decision == "Refus�");

            var avecRestriction = await _context.Demandes
                .Include(d => d.Score)
                .CountAsync(d => d.Score != null && d.Score.Decision == "Accept� avec restriction");

            var activitesBrutes = await _context.Activites
                .OrderByDescending(a => a.Date)
                .Take(5)
                .ToListAsync();

            var activities = activitesBrutes.Select(a => new RecentActivity
            {
                Action = a.Action,
                Utilisateur = a.Utilisateur,
                Temps = GetRelativeTime(a.Date),
                Status = a.Status
            }).ToList();

            var viewModel = new DashboardViewModel
            {
                Username = samAccountName,
                OUs = ous,
                TotalDemandes = total,
                DemandesApprouvees = approuvees,
                DemandesRejetees = rejetees,
                DemandesAvecRestriction = avecRestriction,
                DernieresActivites = activities
            };

            return View("Index", viewModel);
        }

        [Authorize(Policy = "AdminOnly")] // ?? SEULEMENT MAYA
        //[RoleAuthorize("Responsables", "Home/Dashboard")]
        public IActionResult Dashboard()
        {
            // V�rifier que c'est bien maya qui acc�de
            var identity = HttpContext.User.Identity as WindowsIdentity;
            var username = identity?.Name ?? "Unknown";
            var sam = username.Contains("\\") ? username.Split('\\').Last() : username;

            if (!sam.Equals("maya", StringComparison.OrdinalIgnoreCase))
            {
                // Rediriger vers Index si ce n'est pas maya
                return RedirectToAction("Index");
            }

            var demandes = _context.Demandes
                .Include(d => d.Client)
                .Include(d => d.Score)
                .ToList();

            var total = demandes.Count;
            var scor�es = demandes.Count(d => d.Score != null);
            var accept�es = demandes.Count(d => d.Score?.Decision == "Accept�");
            var avecRestriction = demandes.Count(d => d.Score?.Decision == "Accept� avec restriction");
            var tauxAcceptation = scor�es > 0
                   ? (int)Math.Round((double)(accept�es + avecRestriction) / scor�es * 100)
                   : 0;
            var derni�reAnalyse = demandes.OrderByDescending(d => d.DateDemande).FirstOrDefault()?.DateDemande.ToString("dd/MM");

            ViewBag.TotalDemandes = total;
            ViewBag.DemandesScor�es = scor�es;
            ViewBag.TauxAcceptation = tauxAcceptation;
            ViewBag.Derni�reAnalyse = derni�reAnalyse;
            ViewBag.Accept�es = accept�es;
            ViewBag.AvecRestriction = avecRestriction;
            ViewBag.Refus�es = demandes.Count(d => d.Score?.Decision == "Refus�");
            ViewBag.NonScor�es = total - scor�es;

            // ?? AJOUTER LE USERNAME POUR AFFICHAGE
            ViewBag.Username = sam;

            return View(demandes);
        }

        private string GetRelativeTime(DateTime date)
        {
            var span = DateTime.Now - date;

            if (span.TotalMinutes < 1) return "� l'instant";
            if (span.TotalMinutes < 60) return $"Il y a {Math.Floor(span.TotalMinutes)} min";
            if (span.TotalHours < 24) return $"Il y a {Math.Floor(span.TotalHours)} heure(s)";
            return $"Il y a {Math.Floor(span.TotalDays)} jour(s)";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DemandeChequier demande)
        {
            if (ModelState.IsValid)
            {
                demande.DateDemande = DateTime.Now;
                _context.Add(demande);
                await _context.SaveChangesAsync();

                await _activiteService.EnregistrerAsync("Nouvelle demande", User.Identity?.Name ?? "System", "success");

                return RedirectToAction(nameof(Index));
            }

            return View(demande);
        }
    }
}