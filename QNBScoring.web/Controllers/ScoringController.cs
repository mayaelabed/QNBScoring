using Microsoft.AspNetCore.Mvc;
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.DTOs;
<<<<<<< HEAD
=======
using System.Text.Json;
using QNBScoring.Infrastructure; // Namespace for AppDbContext
using Microsoft.EntityFrameworkCore; // For DbContext

>>>>>>> 42f6f51 (additionnal fuctionnality)

namespace QNBScoring.Web.Controllers
{
    public class ScoringController : Controller
    {
        private readonly IExcelImportService _import;
        private readonly IScoringService _scoring;
        private readonly IPdfService _pdf;
<<<<<<< HEAD
=======
        private readonly AppDbContext _dbContext;
>>>>>>> 42f6f51 (additionnal fuctionnality)

        // Injection des dépendances via le constructeur
        public ScoringController(
            IExcelImportService importService,
            IScoringService scoringService,
<<<<<<< HEAD
            IPdfService pdfService)
=======
            IPdfService pdfService,
            AppDbContext dbContext)
>>>>>>> 42f6f51 (additionnal fuctionnality)
        {
            _import = importService;
            _scoring = scoringService;
            _pdf = pdfService;
<<<<<<< HEAD
        }

=======
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult GenerateReport(List<ClientRequestDto> results)
        {
            if (results == null || !results.Any())
                return BadRequest("Aucune donnée à exporter.");

            // Score the requests
            var scoredResults = _scoring.Score(results);

            // Save to database
            foreach (var result in scoredResults)
            {
                var scoreEntry = new ClientScoreResultDto
                {
                    ClientId = result.ClientId,
                    Score = result.Score,
                    Decision = result.Decision,
                    Justification = result.Justification
                };
                _dbContext.ClientScores.Add(scoreEntry);
            }

            _dbContext.SaveChanges();

            // Generate PDF report
            var filePath = _pdf.GenerateReport(scoredResults);

            TempData["ReportPath"] = filePath;
            return RedirectToAction("DownloadReport");
        }


        public IActionResult DownloadReport()
        {
            var path = TempData["ReportPath"] as string;

            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
                return NotFound("Fichier introuvable");

            var fileBytes = System.IO.File.ReadAllBytes(path);
            var fileName = Path.GetFileName(path);
            return File(fileBytes, "application/pdf", fileName);
        }

        public IActionResult Statistics()
        {
            var latest = _scoring.GetLastResults();
            if (string.IsNullOrEmpty(latest)) return View(new List<ClientScoreResultDto>());

            var data = JsonSerializer.Deserialize<List<ClientScoreResultDto>>(latest);
            return View(data);
        }


>>>>>>> 42f6f51 (additionnal fuctionnality)
        [HttpGet]
        public IActionResult Index()
        {
            var username = User.Identity?.Name;
            ViewBag.User = username;
            return View();
        }        

        [HttpPost]
<<<<<<< HEAD
=======
        [HttpPost]
>>>>>>> 42f6f51 (additionnal fuctionnality)
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fichier manquant");

<<<<<<< HEAD
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var filePath = Path.Combine(uploads, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                file.CopyTo(stream);

            var requests = _import.Import(filePath);
            ViewData["Requests"] = requests;
            return View("Preview", requests);
        }
=======
            // Assure-toi que le dossier existe
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, file.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var requests = _import.Import(filePath);
                ViewData["Requests"] = requests;

                return View("Preview", requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'importation : {ex.Message}");
            }
        }

>>>>>>> 42f6f51 (additionnal fuctionnality)
    }
}