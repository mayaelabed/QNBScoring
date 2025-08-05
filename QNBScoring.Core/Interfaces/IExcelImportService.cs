using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QNBScoring.Core.Interfaces
{
    public interface IExcelImportService
    {
        Task<(bool success, string message)> ImportAsync(IFormFile file);
 
        Task<(bool success, string message)> ImportTransactionsAsync(
            Stream fileStream,
            bool overrideExisting = false,
            DateTime? startDate = null,
            DateTime? endDate = null);

    Task<IEnumerable<string>> ValidateExcelFileAsync(Stream fileStream);

    }   
}