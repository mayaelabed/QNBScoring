using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QNBScoring.Core.Entities;

namespace QNBScoring.Infrastructure.Services
{
    public class PdfService
    {
        public byte[] GenererRapport(Score score)
        {
            if (score == null)
                throw new ArgumentNullException(nameof(score), "Le score fourni est nul.");

            if (score.Demande == null)
                throw new InvalidOperationException("Le score ne contient pas d'information sur la demande.");

            if (score.Demande.Client == null)
                throw new InvalidOperationException("Le score ne contient pas d'information sur le client.");

            var client = score.Demande.Client;
            var anciennete = client.DateEntreeEnRelation != default
                ? (DateTime.Now - client.DateEntreeEnRelation).TotalDays / 365.0
                : 0;
            if (score?.Demande?.Client == null)
                throw new Exception("Les informations du client ne sont pas chargées (score.Demande.Client == null).");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Content().Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text(text =>
                        {
                            text.Span("🧾 Rapport de Scoring").FontSize(20).Bold().Underline();
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("🧍 Client: ").SemiBold();
                            text.Span($"{client?.Nom ?? "N/A"} {client?.Prenom ?? "N/A"}");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("🆔 CIN: ").SemiBold();
                            text.Span(client?.CIN ?? "N/A");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("📆 Ancienneté: ").SemiBold();
                            text.Span($"{anciennete:F1} ans");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("💼 Profession: ").SemiBold();
                            text.Span(client?.Profession ?? "N/A");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("📊 Classification SED: ").SemiBold();
                            text.Span(client?.ClassificationSED ?? "N/A");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("⚠️ Incidents: ").SemiBold();
                            text.Span(client?.ADesIncidents == true ? "Oui" : "Non");
                        });

                        col.Item().PaddingTop(15).Text(text =>
                        {
                            text.Span("🔢 Résultat du Scoring").FontSize(16).Bold();
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Score: ").SemiBold();
                            text.Span($"{score.Valeur}");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Décision: ").SemiBold();
                            text.Span(score.Decision ?? "N/A");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Commentaire: ").SemiBold();
                            text.Span(score.Commentaire ?? "Aucun");
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

    }
}