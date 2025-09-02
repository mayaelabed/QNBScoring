using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Services;
using Twilio.Rest.Api.V2010.Account;

public class DemandeService : IDemandeService
{
    private readonly QNBScoringDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly PdfDemandeService _pdfService;
    private readonly ILogger<DemandeService> _logger;
    private readonly ITransactionBancaireRepository _transactionRepo;

    public decimal Balance { get; private set; }

    public DemandeService(QNBScoringDbContext context, IWebHostEnvironment env,
                         PdfDemandeService pdfService, ILogger<DemandeService> logger, ITransactionBancaireRepository transactionRepo)
    {
        _context = context;
        _env = env;
        _pdfService = pdfService;
        _logger = logger;
        _transactionRepo = transactionRepo;
    }

    public async Task<(IEnumerable<DemandeChequier> Demandes, DemandeStats Stats)> GetDemandesAsync(string search, string type)
    {
        var query = _context.Demandes
            .Include(d => d.Client)
            .Include(d => d.Score)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(d =>
                d.Client.Nom.Contains(search) ||
                d.Client.Prenom.Contains(search) ||
                d.Client.AccountNo.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(type) && type != "Tous")
        {
            query = query.Where(d => d.TypeChequier == type);
        }

        var demandes = await query
            .OrderByDescending(d => d.DateDemande)
            .ToListAsync();

        var stats = CalculateStats(demandes);

        return (demandes, stats);
    }
    public async Task<int> CreateDemandeAsync(DemandeChequier model,
        IFormFile pieceIdentiteFile, IFormFile justificatifFile, string accountNo)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            //sql Injection mitigation: ensure accountNo is sanitized
            // 1. Verify client exists
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.AccountNo == accountNo);

            if (client == null)
            {
                throw new ArgumentException("Aucun client trouvé avec ce numéro de compte.");
            }

            // 2. Update client email if provided
            if (!string.IsNullOrEmpty(model.Email))
            {
                client.Email = model.Email;
                await _context.SaveChangesAsync();
            }

            // 3. Create the transaction record
            var newTransaction = new TransactionBancaire
            {
                AccountNo = accountNo,
                TranDate = DateTime.Now,
                ValueDate = DateTime.Now,
                TranType = "CHEQUE_REQ", // Transaction type for checkbook request
                TranAmount = 0, // Or the actual fee amount if applicable
                Balance = Balance,//await GetCurrentBalanceAsync(accountNo),
                TranCode = "CHQ_REQ",
                TranDescEng = "Checkbook request",
                ReconcileRef = $"DEMANDE_{DateTime.Now:yyyyMMdd}",
                Narrative1 = $"Demande de chéquier {model.TypeChequier}",
                Narrative2 ="",// model.Raison,
                PostGrpUserId = "SYSTEM"
            };

            await _transactionRepo.AddAsync(newTransaction);

            // 4. Create the demande
            model.ClientId = client.Id;
            model.DateDemande = DateTime.Now;
            model.TransactionReference = newTransaction.Id; // Store reference if needed

            if (pieceIdentiteFile != null && pieceIdentiteFile.Length > 0)
            {
                model.PieceIdentitePath = await SauvegarderFichierAsync(pieceIdentiteFile);
            }

            if (justificatifFile != null && justificatifFile.Length > 0)
            {
                model.JustificatifDomicilePath = await SauvegarderFichierAsync(justificatifFile);
            }

            _context.Demandes.Add(model);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return model.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating demande");
            throw; // Re-throw after rollback
        }
    }

    public async Task<DemandeChequier> GetDemandeDetailsAsync(int id)
    {
        return await _context.Demandes
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<byte[]> GeneratePdfAsync(int id)
    {
        var demande = await _context.Demandes
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (demande == null)
        {
            throw new KeyNotFoundException($"Demande introuvable: ID {id}");
        }

        return _pdfService.Générer(demande, demande.Client);
    }

    private DemandeStats CalculateStats(IEnumerable<DemandeChequier> demandes)
    {
        var total = demandes.Count();
        var scorees = demandes.Count(d => d.Score != null);
        var acceptees = demandes.Count(d => d.Score?.Decision == "Accepté");
        var avecRestriction = demandes.Count(d => d.Score?.Decision == "Accepté avec restriction");
        var tauxAcceptation = scorees > 0 ? (int)Math.Round((double)(acceptees + avecRestriction) / scorees * 100) : 0;
        var derniereAnalyse = demandes.OrderByDescending(d => d.DateDemande).FirstOrDefault()?.DateDemande.ToString("dd/MM");

        return new DemandeStats
        {
            TotalDemandes = total,
            DemandesScorees = scorees,
            TauxAcceptation = tauxAcceptation,
            DerniereAnalyse = derniereAnalyse,
            Acceptees = acceptees,
            AvecRestriction = avecRestriction,
            Refusees = demandes.Count(d => d.Score?.Decision == "Refusé"),
            NonScorees = total - scorees
        };
    }
    public async Task<bool> UpdateDemandeAsync(DemandeChequier model, IFormFile? pieceIdentiteFile, IFormFile? justificatifFile)
    {
        var existing = await _context.Demandes.FindAsync(model.Id);
        if (existing == null) return false;

        // ⚠️ Mise à jour sécurisée des champs
        existing.TypeChequier = model.TypeChequier;
        existing.NombreChequiers = model.NombreChequiers;
        existing.Motif = model.Motif;
        existing.ModeLivraison = model.ModeLivraison;
        existing.AdresseLivraison = model.AdresseLivraison;
        existing.Telephone = model.Telephone;
        existing.Email = model.Email;
        existing.DeclarationVeracite = model.DeclarationVeracite;
        existing.ConditionsAcceptees = model.ConditionsAcceptees;

        if (pieceIdentiteFile != null && pieceIdentiteFile.Length > 0)
        {
            existing.PieceIdentitePath = await SauvegarderFichierAsync(pieceIdentiteFile);
        }

        if (justificatifFile != null && justificatifFile.Length > 0)
        {
            existing.JustificatifDomicilePath = await SauvegarderFichierAsync(justificatifFile);
        }

        await _context.SaveChangesAsync();
        return true;
    }


    private async Task<string> SauvegarderFichierAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Le fichier est vide");
        }

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);
        var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsDir, uniqueName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{uniqueName}";
    }
}