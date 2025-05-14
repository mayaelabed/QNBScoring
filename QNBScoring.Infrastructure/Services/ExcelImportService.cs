<<<<<<< HEAD
﻿using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using QNBScoring.Core.DTOs;
using QNBScoring.Core.Interfaces;

namespace QNBScoring.Infrastructure.Services;

public class ExcelImportService : IExcelImportService
{
    public IEnumerable<ClientRequestDto> Import(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });
        var records = csv.GetRecords<ClientRequestDto>().ToList();
        return records;
    }
}
=======
﻿
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.DTOs;

namespace QNBScoring.Infrastructure.Services
{
    public class ExcelImportService : IExcelImportService
    {
        public List<ClientRequestDto> Import(string filePath)
        {
            // Simuler l'import (tu peux améliorer plus tard)
            return new List<ClientRequestDto>
                {
                    new() { ClientId = "C001", ClientName = "Ali", Score = (int)(decimal)78.2, Decision = "Acceptée" },
                    new() { ClientId = "C002", ClientName = "Yasmine", Score = (int)(decimal)42.7, Decision = "Refusée" }
                };
        }
    }
}
>>>>>>> 42f6f51 (additionnal fuctionnality)
