using QNBScoring.Core.DTOs;
namespace QNBScoring.Core.Interfaces;

public interface IExcelImportService
{
    IEnumerable<ClientRequestDto> Import(string filePath);
}