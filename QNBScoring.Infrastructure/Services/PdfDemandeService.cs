using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QNBScoring.Core.Entities;

namespace QNBScoring.Infrastructure.Services
{
    public class PdfDemandeService
    {
        public byte[] Générer(DemandeChequier demande, Client client)
        {
            // Configuration obligatoire pour QuestPDF 2023.12+
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(2, Unit.Centimetre);

                    // En-tête
                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .AlignCenter()
                                .Text(text =>
                                {
                                    text.Line("Demande de Chéquier").Bold().FontSize(16);
                                });
                        });

                    // Contenu principal
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Section Client
                            column.Item().Text(text =>
                            {
                                text.Line("Client:").SemiBold();
                                text.Line($"{client.Nom} {client.Prenom}");
                            });

                            column.Item().Text(text =>
                            {
                                text.Line("Numéro de compte:").SemiBold();
                                text.Line(client.AccountNo);
                            });

                            // Section Demande
                            column.Item().Text(text =>
                            {
                                text.Line("Type de chéquier:").SemiBold();
                                text.Line(demande.TypeChequier);
                            });

                            column.Item().Text(text =>
                            {
                                text.Line("Nombre de chéquiers:").SemiBold();
                                text.Line(demande.NombreChequiers.ToString());
                            });
                        });

                    // Pied de page
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Généré le ").FontSize(10);
                            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(10);
                        });
                });
            }).GeneratePdf();
        }
    }
}