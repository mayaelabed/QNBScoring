using QNBScoring.Core.DTOs;
<<<<<<< HEAD
namespace QNBScoring.Core.Interfaces;

public interface IExcelImportService
{
    IEnumerable<ClientRequestDto> Import(string filePath);
}
=======

namespace QNBScoring.Core.Interfaces
{
    public interface IExcelImportService
    {
        List<ClientRequestDto> Import(string filePath);
    }
}
>>>>>>> 42f6f51 (additionnal fuctionnality)
