using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Services;
using QNBScoring.Web.Models;

namespace QNBScoring.Web.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly IActiviteService _activiteService;
        private readonly QNBScoringDbContext _context;  

        public HomeController(QNBScoringDbContext context, IActiviteService activiteService)
        {
            _context = context;
            _activiteService = activiteService;
        }



        public async Task<IActionResult> Index()
        {
            var total = await _context.Demandes.CountAsync();
            var approuvees = await _context.Demandes
                .Include(d => d.Score)
                .CountAsync(d => d.Score != null && d.Score.Decision == "Accept�");

            var rejetees = await _context.Demandes
                .Include(d => d.Score)
                .CountAsync(d => d.Score != null && d.Score.Decision == "Refus�");

            var activitesBrutes = await _context.Activites
                .OrderByDescending(a => a.Date)
                .Take(5)
                .ToListAsync();

            var activites = activitesBrutes.Select(a => new RecentActivity
            {
                Action = a.Action,
                Utilisateur = a.Utilisateur,
                Temps = GetRelativeTime(a.Date),
                Status = a.Status
            }).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalDemandes = total,
                DemandesApprouvees = approuvees,
                DemandesRejetees = rejetees,
                DernieresActivites = activites
            };

            return View(viewModel);
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



        public IActionResult Dashboard()
        {
            // This would be your previous index page with statistics
            var demandes = _context.Demandes
                .Include(d => d.Client)
                .Include(d => d.Score)
                .ToList();

            var total = demandes.Count;
            var scor�es = demandes.Count(d => d.Score != null);
            var accept�es = demandes.Count(d => d.Score?.Decision == "Accept�");
            var tauxAcceptation = scor�es > 0 ? (int)Math.Round((double)accept�es / scor�es * 100) : 0;
            var derni�reAnalyse = demandes.OrderByDescending(d => d.DateDemande).FirstOrDefault()?.DateDemande.ToString("dd/MM");

            ViewBag.TotalDemandes = total;
            ViewBag.DemandesScor�es = scor�es;
            ViewBag.TauxAcceptation = tauxAcceptation;
            ViewBag.Derni�reAnalyse = derni�reAnalyse;
            ViewBag.Accept�es = accept�es;
            ViewBag.Refus�es = demandes.Count(d => d.Score?.Decision == "Refus�");
            ViewBag.NonScor�es = total - scor�es;

            return View(demandes);
        }
        
    }
}
