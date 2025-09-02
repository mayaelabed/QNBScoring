using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;

namespace QNBScoring.Infrastructure.Services
{
    public class ScoringService : IScoringService
    {
        private readonly ITransactionBancaireRepository _transactionRepo;

        public ScoringService(ITransactionBancaireRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }


        public async Task<Score> CalculerScoreAsync(Client client, DemandeChequier demande)
        {
            double score = 0;
            var unAnAvant = DateTime.Now.AddYears(-1);

            // 1. Règles de base du client (40 points max)
            if (!client.ADesIncidents) score += 20;
            if ((DateTime.Now - client.DateEntreeEnRelation).TotalDays >= 3 * 365) score += 10;
            if (client.ClassificationSED is "A" or "B") score += 10;

            // 2. Analyse des transactions (50 points max)
            var transactions = (await _transactionRepo.GetAllAsync())
               .Where(t => t.AccountNo == client.AccountNo && t.TranDate >= unAnAvant)
               .ToList();

            if (transactions.Any())
            {
                // Critères de scoring pour les chèques
                var cheques = transactions.Where(t =>
                    t.TranType == "D" &&
                    !t.Description.Contains("COM") &&
                    (t.OperationType == "INWARD CLEARING" ||
                     t.OperationType == "Q.N.B.TRANSFER CHQ" ||
                     t.OperationType == "CERTIFIED CHEQUES" ||
                     t.OperationType == "CLEARING CHEQUE")
                ).ToList();

                if (cheques.Any())
                {
                    // a. Nombre de chèques (15 points max)
                    var nbCheques = cheques.Count;
                    score += nbCheques switch
                    {
                        < 5 => 5,
                        < 10 => 10,
                        _ => 15
                    };

                    // b. Montant moyen des chèques (15 points max)
                    var montantMoyen = cheques.Average(c => Math.Abs(c.TranAmount));
                    score += montantMoyen switch
                    {
                        < 500 => 15,
                        < 1000 => 10,
                        _ => 5
                    };

                    // c. Fréquence mensuelle (10 points max)
                    var nbMoisAvecCheques = cheques.Select(c => new { c.TranDate.Year, c.TranDate.Month }).Distinct().Count();
                    var frequence = (double)nbCheques / nbMoisAvecCheques;
                    score += frequence switch
                    {
                        < 2 => 10,
                        < 5 => 5,
                        _ => 0
                    };
                }

                // 3. Autres critères financiers (10 points max)
                var soldeMoyen = transactions.Average(t => t.Balance);
                if (soldeMoyen > 5000) score += 5;
                if (soldeMoyen > 10000) score += 5;
            }

            // 4. Critères spécifiques au chéquier demandé (10 points max)
            if (demande.TypeChequier == "Retail") score += 5;
            if (demande.NombreChequiers == 1) score += 5;

            // Décision finale unique et cohérente
            var (decision, commentaire) = score switch
            {
                >= 70 => ("Accepté", "Profil a été accepté suivant les critères de la banque"),
                >= 50 => ("Accepté avec restriction", "Profil moyen - chéquier limité recommandé"),
                _ => ("Refusé", "Profil à risque - demande refusée")
            };

            return new Score
            {
                Valeur = score,
                Decision = decision,
                Commentaire = commentaire,
                DemandeChequierId = demande.Id,
                DateCreation = DateTime.Now // Ajout recommandé
            };
        }

        private double CalculateStandardDeviation(IEnumerable<decimal> values)
        {
            if (!values.Any()) return 0;

            var avg = values.Average();
            var sumOfSquares = values.Sum(v => (v - avg) * (v - avg));
            return Math.Sqrt((double)sumOfSquares / values.Count());
        }
    }
}