using Xunit;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using QNBScoring.Infrastructure.Services;
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.Entities;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;

public class ExcelImportServiceTests
{
    [Fact]
    public async Task ImportAsync_ValidFile_ImportsTransactions()
    {
        // Arrange
        var mockTransactionRepo = new Mock<ITransactionBancaireRepository>();
        var mockClientRepo = new Mock<IClientRepository>();

        // Retourne un client fictif pour tous les AccountNo
        mockClientRepo.Setup(r => r.GetByAccountNoAsync(It.IsAny<string>()))
                      .ReturnsAsync(new Client { AccountNo = "1659-322422-292" });

        var service = new ExcelImportService(mockTransactionRepo.Object, mockClientRepo.Object);

        // Création fichier Excel simulé en mémoire
        var ms = new MemoryStream();
        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.AddWorksheet("Transactions");
            ws.Cell(1, 1).Value = "AccountNo";
            ws.Cell(1, 2).Value = "TranDate";
            ws.Cell(1, 3).Value = "ValueDate";
            ws.Cell(1, 4).Value = "TranType";
            ws.Cell(1, 5).Value = "TranAmount";
            ws.Cell(1, 6).Value = "Balance";
            ws.Cell(1, 7).Value = "TranCode";
            ws.Cell(1, 8).Value = "TranDescEng";
            ws.Cell(1, 9).Value = "ReconcileRef";
            ws.Cell(1, 10).Value = "Narrative1";
            ws.Cell(1, 11).Value = "Narrative2";
            ws.Cell(1, 12).Value = "Narrative3";
            ws.Cell(1, 13).Value = "Narrative4";
            ws.Cell(1, 14).Value = "PostGrpUserId";

            ws.Cell(2, 1).Value = "1659-322422-292";
            ws.Cell(2, 2).Value = "2025-06-11 15:49:58";
            ws.Cell(2, 3).Value = "2025-06-11 15:49:58";
            ws.Cell(2, 4).Value = "DEP";
            ws.Cell(2, 5).Value = "7000.00";
            ws.Cell(2, 6).Value = "15500.00";
            ws.Cell(2, 7).Value = "DEP02";
            ws.Cell(2, 8).Value = "Salary Credit";
            ws.Cell(2, 9).Value = "SAL202";
            ws.Cell(2, 10).Value = "Salaire juin";
            ws.Cell(2, 11).Value = "Compagnie ABC";
            ws.Cell(2, 12).Value = "";
            ws.Cell(2, 13).Value = "";
            ws.Cell(2, 14).Value = "COMPANY";

            workbook.SaveAs(ms);
        }
        ms.Position = 0;

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.FileName).Returns("test.xlsx");
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, System.Threading.CancellationToken>((stream, token) => ms.CopyToAsync(stream, token));

        // Act
        var (success, message) = await service.ImportAsync(fileMock.Object);

        // Assert
        Assert.True(success);
        Assert.Contains("transactions importées", message);
        mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<TransactionBancaire>()), Times.Once);
    }
}
