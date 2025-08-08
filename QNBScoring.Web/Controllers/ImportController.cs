using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QNBScoring.Infrastructure.Services;
using System.Threading.Tasks;

namespace QNBScoring.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly ExcelImportService _importService;

        public ImportController(ExcelImportService importService)
        {
            _importService = importService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Importer(IFormFile file)
        {
            var (success, message) = await _importService.ImportAsync(file);
            TempData["ImportResult"] = message;

            return RedirectToAction("Index");
        }
    }
}
