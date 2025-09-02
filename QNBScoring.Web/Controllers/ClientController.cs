// ClientController.cs
using Microsoft.AspNetCore.Mvc;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.ViewModels;
using QNBScoring.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QNBScoring.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly ITransactionBancaireRepository _transactionRepository;

        public ClientController(
            IClientRepository clientRepository,
            ITransactionBancaireRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clients = await _clientRepository.GetAllAsync();

            var viewModel = clients.Select(c => new ClientListViewModel
            {
                Id = c.Id,
                AccountNo = c.AccountNo,
                Nom = c.Nom,
                Profession = c.Profession,
                LastImportDate = c.LastTransactionImport,
                TransactionCount = c.Transactions?.Count ?? 0
            });

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }
    }
}