using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Services;
using System.Linq;
using System.Threading.Tasks;

namespace QNBScoring.Web.Controllers
{
    public class ScoringController : Controller
    {
        private readonly IDemandeChequierRepository _demandeRepo;
        private readonly IScoreRepository _scoreRepo;
        private readonly IScoringService _scoringService;
        private readonly PdfService _pdfService;
        private readonly QNBScoringDbContext _context;
        private readonly IEmailService _emailService;

        public ScoringController(
            IDemandeChequierRepository demandeRepo,
            IScoreRepository scoreRepo,
            IScoringService scoringService,
            PdfService pdfService,
            QNBScoringDbContext context, IEmailService emailService)
        {
            _demandeRepo = demandeRepo;
            _scoreRepo = scoreRepo;
            _scoringService = scoringService;
            _pdfService = pdfService;
            _context = context;
            _emailService = emailService;

        }

        public async Task<IActionResult> Index()
        {
            var scores = await _scoreRepo.GetAllAsync();
            return View(scores);
        }

        // GET: /Scoring/Analyser?demandeId=5
        public async Task<IActionResult> Analyser(int demandeId)
        {
            var demande = await _demandeRepo.GetByIdWithClientAsync(demandeId);
            if (demande == null)
                return NotFound();

            if (demande.Score != null)
            {
                TempData["Info"] = "⚠️ Un score existe déjà pour cette demande. Redirection vers le résultat.";
                return RedirectToAction("Resultats", new { scoreId = demande.Score.Id });
            }

            var score = await _scoringService.CalculerScoreAsync(demande.Client, demande);
            await _scoreRepo.AddAsync(score);

            return RedirectToAction("Resultats", new { scoreId = score.Id });
        }
        public IActionResult Historique(string search, string decision)
        {
            var scores = _context.Scores
                .Include(s => s.Demande)
                    .ThenInclude(d => d.Client)
                .AsQueryable();

            // 🔍 Filtrage par nom ou prénom du client
            if (!string.IsNullOrWhiteSpace(search))
            {
                string normalized = search.Trim().ToLower();
                scores = scores.Where(s =>
                    s.Demande.Client.Nom.ToLower().Contains(normalized) ||
                    s.Demande.Client.Prenom.ToLower().Contains(normalized));
            }

            // ✅ Filtrage par décision
            if (!string.IsNullOrWhiteSpace(decision))
            {
                scores = scores.Where(s => s.Decision == decision);
            }

            // Pour conserver les valeurs dans la vue
            ViewBag.Search = search;
            ViewBag.Decision = decision;

            return View(scores.ToList());
        }




        // GET: /Scoring/Resultats?scoreId=3
        public async Task<IActionResult> Resultats(int scoreId)
        {
            var score = await _scoreRepo.GetByIdWithDemandeAndClientAsync(scoreId);
            if (score == null)
                return NotFound();

            return View(score);
        }

        // GET: /Scoring/TéléchargerPDF?scoreId=3
        public async Task<IActionResult> TéléchargerPDF(int scoreId)
        {
            var score = await _scoreRepo.GetByIdWithDemandeAndClientAsync(scoreId);
            if (score == null)
                return NotFound();

            var pdfBytes = _pdfService.GenererRapport(score);
            return File(pdfBytes, "application/pdf", "rapport_scoring.pdf");
        }


        /*public async Task<IActionResult> Statistics()
        {
            var scores = await _scoreRepo.GetAllAsync();

            ViewBag.ScoreData = scores
                .Select(s => new {
                    Date = s.Demande.DateDemande.ToString("yyyy-MM-dd"),
                    Valeur = s.Valeur,
                    Decision = s.Decision,
                    Profession = s.Demande.Client.Profession

                }).ToList();

            return View(scores);
        }
        */
        [HttpPost]
        public async Task<IActionResult> EnvoyerMail(int scoreId)
        {
            var score = await _context.Scores
                .Include(s => s.Demande)
                .ThenInclude(d => d.Client)
                .FirstOrDefaultAsync(s => s.Id == scoreId);

            if (score == null) return NotFound();

            var agentEmail = score.Demande.Client.Email; // supposons que tu as ajouté Email à l’entité Client
            var subject = $"Résultat de la demande de chéquier #{score.Demande.Id}";
            var body = $@"
            <h3>Bonjour {score.Demande.Client.Prenom},</h3>
            <p>Votre demande de chéquier a été traitée.</p>
            <p><b>Résultat :</b> {score.Decision}</p>
            <p><b>Score obtenu :</b> {score.Valeur}%</p>
            <p><b>Commentaire :</b> {score.Commentaire}</p>
            <hr>
            <small>Message envoyé automatiquement par le système QNBScoring</small>";

            await _emailService.SendEmailAsync(agentEmail, subject, body);

            TempData["Success"] = $"Email envoyé à l’agent {score.Demande.Client.Nom}.";
            return RedirectToAction("Historique");
        }
        [HttpPost]
        public async Task<IActionResult> Supprimer(int scoreId)
        {
            var score = await _scoreRepo.GetByIdAsync(scoreId);
            if (score == null)
                return NotFound();

            _context.Set<Score>().Remove(score);
            await _context.SaveChangesAsync();

            TempData["Info"] = "✅ Le score a été supprimé.";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Statistics()
        {
            var scores = await _scoreRepo.GetAllAsync();

            var scoreData = scores
                .Select(s => new
                {
                    s.Decision,
                    Date = s.Demande.DateDemande.ToString("yyyy-MM-dd"),
                    s.Valeur
                })
                .ToList();

            ViewBag.ScoreData = scoreData;
            return View(scores);
        }
       /* [HttpGet]
        public async Task<IActionResult> Historique(
    [FromQuery] string? startDate,
    [FromQuery] string? endDate,
    [FromQuery] string? decision)
        {
            var query = _context.Scores
                .Include(s => s.Demande)
                .ThenInclude(d => d.Client)
                .AsQueryable();

            // Filtre par date
            if (DateTime.TryParse(startDate, out var start))
            {
                query = query.Where(s => s.Demande.DateDemande >= start);
            }

            if (DateTime.TryParse(endDate, out var end))
            {
                query = query.Where(s => s.Demande.DateDemande <= end.AddDays(1));
            }

            // Filtre par décision
            if (!string.IsNullOrEmpty(decision))
            {
                query = query.Where(s => s.Decision == decision);
            }

            var scores = await query
                .OrderByDescending(s => s.Demande.DateDemande)
                .ToListAsync();

            return View(scores);
        }*/

        [HttpPost]
        public async Task<IActionResult> Recalculer(int demandeId)
        {
            var demande = await _demandeRepo.GetByIdWithClientAsync(demandeId);
            if (demande == null)
                return NotFound();

            var existingScore = demande.Score;
            if (existingScore != null)
            {
                _context.Set<Score>().Remove(existingScore); 
                await _context.SaveChangesAsync();
            }

            var nouveauScore = await _scoringService.CalculerScoreAsync(demande.Client, demande);
            await _scoreRepo.AddAsync(nouveauScore);

            TempData["Info"] = "✅ Le score a été recalculé avec succès.";
            return RedirectToAction("Resultats", new { scoreId = nouveauScore.Id });
        }
    }
}
