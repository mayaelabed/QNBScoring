using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QNBScoring.Core.Entities;

namespace QNBScoring.Web.Services
{
    public class PdfService
    {
        public byte[] GenererRapport(Score score)
{
    var client = score.Demande.Client;
    var demande = score.Demande;

    var anciennete = (DateTime.Now - client.DateEntreeEnRelation).TotalDays / 365;

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(50);
            page.Content().Column(col =>
            {
                col.Item().PaddingBottom(10).Text("🧾 Rapport de Scoring").Style(TextStyle.Default.Size(20).Bold());


                col.Item().Text(text =>
                {
                    text.Span("🧍 Client: ").SemiBold();
                    text.Span($"{client.Nom} {client.Prenom}");
                });

                col.Item().Text(text =>
                {
                    text.Span("🆔 CIN: ").SemiBold();
                    text.Span(client.CIN);
                });

                col.Item().Text(text =>
                {
                    text.Span("📆 Date entrée en relation: ").SemiBold();
                    text.Span($"{client.DateEntreeEnRelation.ToShortDateString()} ({anciennete:F1} ans)");
                });

                col.Item().Text(text =>
                {
                    text.Span("💼 Profession: ").SemiBold();
                    text.Span(client.Profession);
                });

                col.Item().Text(text =>
                {
                    text.Span("📊 Classification SED: ").SemiBold();
                    text.Span(client.ClassificationSED);
                });

                col.Item().Text(text =>
                {
                    text.Span("⚠️ Incidents: ").SemiBold();
                    text.Span(client.ADesIncidents ? "Oui" : "Non");
                });

                col.Item().PaddingBottom(10).Text(text =>
                {
                    text.Span("📄 Date de la demande: ").SemiBold();
                    text.Span(demande.DateDemande.ToShortDateString());
                });
                col.Item().PaddingBottom(10).Text("🔢 Résultat du Scoring").Style(TextStyle.Default.Size(16).Bold());


                col.Item().Text(text =>
                {
                    text.Span("Score: ").SemiBold();
                    text.Span(score.Valeur.ToString("F2"));
                });

                col.Item().Text(text =>
                {
                    text.Span("Décision: ").SemiBold();
                    text.Span(score.Decision);
                });

                col.Item().PaddingBottom(10).Text(text =>
                {
                    text.Span("Commentaire: ").SemiBold();
                    text.Span(score.Commentaire);
                });

                col.Item().PaddingBottom(10).Text("📈 Critères de calcul").Style(TextStyle.Default.Size(14).Bold());


                col.Item().Text("- Pas d’incidents : +40 points");
                col.Item().Text("- Ancienneté > 3 ans : +30 points");
                col.Item().Text("- Classification SED A/B : +20 points");
                col.Item().Text("- Profession contient 'cadre' : +10 points");
                col.Item().Text("Seuil d’acceptation : 60 points")
                    .Style(TextStyle.Default.Italic().FontColor(Colors.Grey.Darken1));
            });

        });
    });

    return document.GeneratePdf();
}

    }
}
