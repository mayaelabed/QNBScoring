using System.Formats.Asn1;
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