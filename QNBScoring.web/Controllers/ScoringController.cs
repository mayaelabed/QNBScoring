using Microsoft.AspNetCore.Mvc;
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.DTOs;

namespace QNBScoring.Web.Controllers
{
    public class ScoringController : Controller
    {
        private readonly IExcelImportService _import;
        private readonly IScoringService _scoring;
        private readonly IPdfService _pdf;

        // Injection des dépendances via le constructeur
        public ScoringController(
            IExcelImportService importService,
            IScoringService scoringService,
            IPdfService pdfService)
        {
            _import = importService;
            _scoring = scoringService;
            _pdf = pdfService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var username = User.Identity?.Name;
            ViewBag.User = username;
            return View();
        }        

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fichier manquant");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var filePath = Path.Combine(uploads, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                file.CopyTo(stream);

            var requests = _import.Import(filePath);
            ViewData["Requests"] = requests;
            return View("Preview", requests);
        }
    }
}