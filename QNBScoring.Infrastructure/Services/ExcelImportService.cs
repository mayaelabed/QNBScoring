using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;

namespace QNBScoring.Infrastructure.Services
    {
        public class ExcelImportService
        {
            private readonly ITransactionBancaireRepository _transactionRepo;
            private readonly IClientRepository _clientRepo;

            public ExcelImportService(
                ITransactionBancaireRepository transactionRepo,
                IClientRepository clientRepo)
            {
                _transactionRepo = transactionRepo;
                _clientRepo = clientRepo;
            }

            public async Task<(bool success, string message)> ImportAsync(IFormFile file)
            {
            var importDate = DateTime.Now;

            if (file == null || file.Length == 0)
                    return (false, "Fichier invalide.");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);

                var worksheet = workbook.Worksheets.FirstOrDefault(s => s.Visibility == XLWorksheetVisibility.Visible);
                if (worksheet == null)
                    return (false, "Aucune feuille visible dans le fichier.");

                var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1);
                if (rows == null)
                    return (false, "Feuille vide.");

                int count = 0;
                var updatedClients = new HashSet<string>(); // To avoid updating the same client multiple times

                foreach (var row in rows)
                {
                    var accountNo = row.Cell(1).GetString()?.Trim();
                    if (string.IsNullOrWhiteSpace(accountNo)) continue;

                    var client = await _clientRepo.GetByAccountNoAsync(accountNo);
                    if (client == null) continue;

                    var transaction = new TransactionBancaire
                    {
                        AccountNo = accountNo,
                        TranDate = DateTime.TryParse(row.Cell(2).GetString(), out var td) ? td : DateTime.MinValue,
                        ValueDate = DateTime.TryParse(row.Cell(3).GetString(), out var vd) ? vd : DateTime.MinValue,
                        TranType = row.Cell(4).GetString(),
                        TranAmount = decimal.TryParse(row.Cell(5).GetString(), out var ta) ? ta : 0,
                        Balance = decimal.TryParse(row.Cell(6).GetString(), out var b) ? b : 0,
                        TranCode = row.Cell(7).GetString(),
                        TranDescEng = row.Cell(8).GetString(),
                        ReconcileRef = row.Cell(9).GetString(),
                        Narrative1 = row.Cell(10).GetString(),
                        Narrative2 = row.Cell(11).GetString(),
                        Narrative3 = row.Cell(12).GetString(),
                        Narrative4 = row.Cell(13).GetString(),
                        PostGrpUserId = row.Cell(14).GetString()
                    };

                    await _transactionRepo.AddAsync(transaction);

                /// Update client's last import date
                if (!updatedClients.Contains(accountNo))
                {
                    client.LastTransactionImport = importDate;
                    await _clientRepo.UpdateAsync(client);
                    updatedClients.Add(accountNo);
                }

                count++;
                }

                return (true, $"{count} transactions importées pour {updatedClients.Count} clients.");
            }
        }
}