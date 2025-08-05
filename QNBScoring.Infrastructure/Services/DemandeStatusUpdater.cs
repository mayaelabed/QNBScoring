using QNBScoring.Core.Entities;
using QNBScoring.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

public class DemandeStatusUpdater
{
    private readonly QNBScoringDbContext _context;

    public DemandeStatusUpdater(QNBScoringDbContext context)
    {
        _context = context;
    }

    public void MettreAJourEtatsDemandes()
    {
        var demandes = _context.Demandes
            .Include(d => d.Score)
            .ToList();

        foreach (var demande in demandes)
        {
            if (demande.Score != null)
            {
                demande.EtatDemande = EtatDemande.Traitee;
            }
            else if ((DateTime.Now - demande.DateDemande).TotalDays > 2)
            {
                demande.EtatDemande = EtatDemande.EnCoursDeTraitement;
            }
            else
            {
                demande.EtatDemande = EtatDemande.PasEncoreTraitee;
            }
        }


        _context.SaveChanges();
    }
}
