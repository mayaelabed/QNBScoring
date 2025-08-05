using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Core.Entities
{
    public enum EtatDemande
    {
        PasEncoreTraitee,
        EnCoursDeTraitement,
        Traitee
    }

    public class DemandeChequier
    {
        public int Id { get; set; }
        public DateTime DateDemande { get; set; } = DateTime.Now;
        public EtatDemande EtatDemande { get; set; } = EtatDemande.PasEncoreTraitee;

        public int ClientId { get; set; }
        [ValidateNever]
        public Client Client { get; set; } = null!;

        // 🧾 Informations sur le chéquier

        public string TypeChequier { get; set; } = "Standard"; // Exemples : Standard, Agréé, Spécial
        [Range(5, 15)]
        public int NombreChequiers { get; set; } =1;
       
        public string Motif { get; set; } = string.Empty;

        // 📦 Conditions de livraison
        public string ModeLivraison { get; set; } = "À retirer en agence"; // Exemples : À retirer en agence, Livraison à domicile
         public string AdresseLivraison { get; set; } = string.Empty;

        // 📞 Informations de contact
        
        [Phone]
        public string Telephone { get; set; } = string.Empty;
        
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // ✅ Validation
         public bool DeclarationVeracite { get; set; } = false;
         public bool ConditionsAcceptees { get; set; } = false;

        // 📄 Pièces jointes
        public string? PieceIdentitePath { get; set; }
        public string? JustificatifDomicilePath { get; set; }
        

        // 🔗 Relation avec le score
        public Score? Score { get; set; }
        public int TransactionReference { get; set; }
        //public string Raison { get; set; }
    }

    /* Détails des nouveaux champs
TypeChequier : Permet de spécifier le type de chéquier souhaité (Standard, Agréé, Spécial).
DEMANDE EXPERT

NombreChequiers : Indique la quantité de chéquiers demandés.

Motif : Justifie la demande de chéquier (par exemple, gestion des dépenses professionnelles).
DEMANDE EXPERT

ModeLivraison : Précise le mode de livraison préféré (par exemple, à retirer en agence ou livraison à domicile).

AdresseLivraison : Si la livraison à domicile est choisie, cette adresse sera utilisée.

Telephone et Email : Coordonnées du demandeur pour le suivi de la demande.

DeclarationVeracite et ConditionsAcceptees : Champs booléens pour confirmer que le demandeur certifie l'exactitude des informations fournies et accepte les conditions générales.

PieceIdentitePath et JustificatifDomicilePath : Chemins vers les fichiers téléchargés pour la pièce d'identité et le justificatif de domicile.
DEMANDE EXPERT*/

}
