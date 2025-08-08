using Azure.Core;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QNBScoring.Core.Entities;
using QNBScoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DemandeController : Controller
{
    private readonly IDemandeService _demandeService;
    private readonly ILogger<DemandeController> _logger;
    private readonly QNBScoringDbContext _context;

    
    public DemandeController(IDemandeService demandeService, QNBScoringDbContext context, ILogger<DemandeController> logger)
    {
        _demandeService = demandeService;
        _logger = logger;
        _context = context;

    }

    public async Task<IActionResult> Index(string? search, string? type)
    {
        try
        {
            var (demandes, stats) = await _demandeService.GetDemandesAsync(search, type);

            ViewBag.TotalDemandes = stats.TotalDemandes;
            ViewBag.DemandesScorees = stats.DemandesScorees;
            ViewBag.TauxAcceptation = stats.TauxAcceptation;
            ViewBag.DerniereAnalyse = stats.DerniereAnalyse;
            ViewBag.Acceptees = stats.Acceptees;
            ViewBag.Refusees = stats.Refusees;
            ViewBag.NonScorees = stats.NonScorees;
            ViewBag.TypeFilter = type;
            ViewBag.Search = search;

            return View(demandes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des demandes");
            TempData["Error"] = "Une erreur est survenue lors du chargement des demandes";
            return View(new List<DemandeChequier>());
        }
    }

    public IActionResult Create()
    {
        // Initialize with default values if needed
        var model = new DemandeChequier
        {
            DateDemande = DateTime.Now
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        DemandeChequier model,
        IFormFile? PieceIdentiteFile,
        IFormFile? JustificatifFile)
    {
        var accountNo = Request.Form["AccountNo"].ToString();

        // Enhanced validation
        if (string.IsNullOrWhiteSpace(accountNo))
        {
            ModelState.AddModelError("AccountNo", "Le numéro de compte est requis.");
        }
        else if (accountNo.Length <= 9)
        {
            ModelState.AddModelError("AccountNo", "Le numéro de compte est moins que 9 chiffres.");
        }

        // Email validation
        if (!string.IsNullOrEmpty(model.Email))
        {
            if (!IsValidEmail(model.Email))
            {
                ModelState.AddModelError("Email", "L'adresse email n'est pas valide.");
            }
        }

        // Phone number validation
        if (!string.IsNullOrEmpty(model.Telephone))
        {
            if (!IsValidPhoneNumber(model.Telephone))
            {
                ModelState.AddModelError("Telephone", "Le numéro de téléphone n'est pas valide.");
            }
        }

        // File validations
        if (PieceIdentiteFile == null || PieceIdentiteFile.Length == 0)
        {
            ModelState.AddModelError("PieceIdentiteFile", "La pièce d'identité est requise.");
        }
        else if (PieceIdentiteFile.Length > 5 * 1024 * 1024) // 5MB
        {
            ModelState.AddModelError("PieceIdentiteFile", "La taille du fichier ne doit pas dépasser 5MB.");
        }

        if (JustificatifFile != null && JustificatifFile.Length > 5 * 1024 * 1024)
        {
            ModelState.AddModelError("JustificatifFile", "La taille du fichier ne doit pas dépasser 5MB.");
        }

        if (!ModelState.IsValid)
        {
            LogModelStateErrors();
            return View(model);
        }

        try
        {
            var demandeId = await _demandeService.CreateDemandeAsync(model, PieceIdentiteFile, JustificatifFile, accountNo);

            TempData["Success"] = $"Demande #{demandeId} enregistrée avec succès.";
            return RedirectToAction(nameof(Details), new { id = demandeId });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création d'une demande");
            ModelState.AddModelError("", $"Une erreur critique est survenue: {ex.Message}");
            return View(model);
        }
    }
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Simple validation - can be enhanced with regex
        return phoneNumber.Length >= 8 && phoneNumber.All(c => char.IsDigit(c) || c == '+' || c == ' ');
    }

    //GET: Demande/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var demande = await _demandeService.GetDemandeDetailsAsync(id);
        if (demande == null)
            return NotFound();

        return View(demande);
    }

    // POST: Demande/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DemandeChequier model, IFormFile? PieceIdentiteFile, IFormFile? JustificatifFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _demandeService.UpdateDemandeAsync(model, PieceIdentiteFile, JustificatifFile);
        if (!result)
        {
            ModelState.AddModelError("", "Demande introuvable ou erreur lors de la mise à jour.");
            return View(model);
        }

        TempData["Success"] = $"Demande #{model.Id} modifiée avec succès.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
    // GET: /Demande/Suivi/5
    public async Task<IActionResult> Suivi(int id)
    {
        var demande = await _context.Demandes.FindAsync(id);
        if (demande == null)
        {
            return NotFound();
        }
        return View(demande);
    }
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var demande = await _demandeService.GetDemandeDetailsAsync(id);
            if (demande == null)
            {
                return NotFound();
            }
            return View(demande);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des détails de la demande {DemandeId}", id);
            TempData["Error"] = "Une erreur est survenue lors du chargement des détails";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> TelechargerPDF(int id)
    {
        try
        {
            var pdfBytes = await _demandeService.GeneratePdfAsync(id);
            return File(pdfBytes, "application/pdf", $"Demande_Chequier_{id}.pdf");
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Demande introuvable: ID {DemandeId}", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du PDF pour la demande {DemandeId}", id);
            TempData["Error"] = "Une erreur est survenue lors de la génération du PDF";
            return RedirectToAction(nameof(Index));
        }
    }

    private void LogModelStateErrors()
    {
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                _logger.LogWarning("Erreur de validation: {Key} - {Message}", entry.Key, error.ErrorMessage);
            }
        }
    }
}