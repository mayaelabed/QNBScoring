<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNBScoring.Infrastructure.Services
{
    class PdfService
    {
        public PdfService() { }
=======
﻿// Fichier : Infrastructure/Services/PdfService.cs

using QNBScoring.Core.DTOs;
using QNBScoring.Core.Interfaces;
using System.Text;

namespace QNBScoring.Infrastructure.Services;

public class PdfService : IPdfService
{
    public string GenerateReport(List<ClientRequestDto> results)
    {
        var fileName = $"report_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/reports", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        // Exemple basique : créer un fichier texte renommé .pdf (à remplacer par une vraie lib plus tard)
        var content = new StringBuilder();
        content.AppendLine("Rapport de scoring des demandes de chéquiers");
        content.AppendLine("---------------------------------------------");
        foreach (var r in results)
        {
            content.AppendLine($"Client : {r.ClientName} ({r.ClientId})");
            content.AppendLine($"Score : {r.Score}");
            content.AppendLine($"Décision : {r.Decision}");
            content.AppendLine();
        }

        File.WriteAllText(filePath, content.ToString());

        return filePath;
>>>>>>> 42f6f51 (additionnal fuctionnality)
    }
}
