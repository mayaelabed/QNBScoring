using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using QNBScoring.Infrastructure.Services;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Core.Entities;
using QNBScoring.Infrastructure.Repositories;
using ClosedXML.Excel;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace QNBScoring.Infrastructure.Tests.Services
{
    public class ExcelImportServiceIntegrationTests : IDisposable
    {
        private readonly QNBScoringDbContext _dbContext;
        private readonly TransactionBancaireRepository _transactionRepo;
        private readonly ClientRepository _clientRepo;
        private readonly ExcelImportService _service;

        public ExcelImportServiceIntegrationTests()
        {
            // Configuration de la base en mémoire
            var options = new DbContextOptionsBuilder<QNBScoringDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Nom unique pour chaque test
                .Options;

            _dbContext = new QNBScoringDbContext(options);

            // Initialisation des repositories
            _transactionRepo = new TransactionBancaireRepository(_dbContext);
            _clientRepo = new ClientRepository(_dbContext);
            _service = new ExcelImportService(_transactionRepo, _clientRepo);

            // Nettoyer et initialiser la base
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Ajouter des clients de test avec tous les champs requis
            var clients = new List<Client>
            {
                new Client
                {
                    CIN = "12345678",
                    Nom = "Doe",
                    Prenom = "John",
                    Email = "john.doe@email.com",
                    DateEntreeEnRelation = DateTime.Now.AddYears(-2),
                    Profession = "Engineer",
                    ClassificationSED = "A",
                    ADesIncidents = false,
                    AccountNo = "ACC001",
                    Telephone = 12345678,
                    //CreatedDate = DateTime.Now,
                    //CreatedBy = "TEST"
                },
                new Client
                {
                    CIN = "87654321",
                    Nom = "Smith",
                    Prenom = "Jane",
                    Email = "jane.smith@email.com",
                    DateEntreeEnRelation = DateTime.Now.AddYears(-1),
                    Profession = "Doctor",
                    ClassificationSED = "B",
                    ADesIncidents = true,
                    AccountNo = "ACC002",
                    Telephone = 87654321,
                    //CreatedDate = DateTime.Now,
                    //CreatedBy = "TEST"
                }
            };

            _dbContext.Clients.AddRange(clients);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task ImportAsync_ValidFile_ShouldImportTransactionsAndUpdateClient()
        {
            // Arrange
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Transactions");

            // En-têtes avec toutes les colonnes attendues
            var headers = new[]
            {
                "AccountNo", "TranDate", "ValueDate", "TranType", "TranAmount", "Balance",
                "TranCode", "TranDescEng", "ReconcileRef", "Narrative1", "Narrative2",
                "Narrative3", "Narrative4", "PostGrpUserId"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Données de transaction valides
            worksheet.Cell(2, 1).Value = "ACC001"; // AccountNo existant
            worksheet.Cell(2, 2).Value = "2024-01-15";
            worksheet.Cell(2, 3).Value = "2024-01-16";
            worksheet.Cell(2, 4).Value = "Credit";
            worksheet.Cell(2, 5).Value = 1500.50;
            worksheet.Cell(2, 6).Value = 5000.75;
            worksheet.Cell(2, 7).Value = "CHQ";
            worksheet.Cell(2, 8).Value = "Cheque Deposit";
            worksheet.Cell(2, 9).Value = "REF20240115001";
            worksheet.Cell(2, 10).Value = "Salary payment";
            worksheet.Cell(2, 11).Value = "Monthly salary";
            worksheet.Cell(2, 12).Value = "Company XYZ";
            worksheet.Cell(2, 13).Value = "Completed";
            worksheet.Cell(2, 14).Value = "USER001";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "transactions", "transactions.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            // Act
            var result = await _service.ImportAsync(file);

            // Assert
            Assert.True(result.success, $"Import failed: {result.message}");
            Assert.Contains("1 transactions importées", result.message);
            Assert.Contains("1 clients", result.message);

            // Vérifier la transaction importée
            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.AccountNo == "ACC001");

            Assert.NotNull(transaction);
            Assert.Equal(1500.50m, transaction.TranAmount);
            Assert.Equal("Credit", transaction.TranType);
            Assert.Equal("CHQ", transaction.TranCode);
            Assert.Equal("Cheque Deposit", transaction.TranDescEng);

            // Vérifier la mise à jour du client
            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(c => c.AccountNo == "ACC001");

            Assert.NotNull(client);
            Assert.NotNull(client.LastTransactionImport);
            Assert.True((DateTime.Now - client.LastTransactionImport.Value).TotalSeconds < 5);
        }

        [Fact]
        public async Task ImportAsync_MultipleTransactionsSameClient_ShouldUpdateClientOnce()
        {
            // Arrange
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Transactions");

            // En-têtes
            var headers = new[]
            {
                "AccountNo", "TranDate", "ValueDate", "TranType", "TranAmount", "Balance",
                "TranCode", "TranDescEng", "ReconcileRef", "Narrative1", "Narrative2",
                "Narrative3", "Narrative4", "PostGrpUserId"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // 3 transactions pour le même client
            for (int i = 0; i < 3; i++)
            {
                worksheet.Cell(i + 2, 1).Value = "ACC001";
                worksheet.Cell(i + 2, 2).Value = $"2024-01-{15 + i}";
                worksheet.Cell(i + 2, 3).Value = $"2024-01-{16 + i}";
                worksheet.Cell(i + 2, 4).Value = i % 2 == 0 ? "Credit" : "Debit";
                worksheet.Cell(i + 2, 5).Value = 100 * (i + 1);
                worksheet.Cell(i + 2, 6).Value = 5000 + (100 * (i + 1));
                worksheet.Cell(i + 2, 7).Value = "CHQ";
                worksheet.Cell(i + 2, 8).Value = $"Transaction {i + 1}";
                worksheet.Cell(i + 2, 9).Value = $"REF2024011500{i + 1}";
                worksheet.Cell(i + 2, 10).Value = $"Narrative {i + 1}";
                worksheet.Cell(i + 2, 11).Value = $"Details {i + 1}";
                worksheet.Cell(i + 2, 12).Value = $"Info {i + 1}";
                worksheet.Cell(i + 2, 13).Value = $"Status {i + 1}";
                worksheet.Cell(i + 2, 14).Value = "USER001";
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "transactions", "transactions.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            // Act
            var result = await _service.ImportAsync(file);

            // Assert
            Assert.True(result.success);
            Assert.Contains("3 transactions importées", result.message);
            Assert.Contains("1 clients", result.message); // Un seul client mis à jour

            // Vérifier les transactions
            var transactions = await _dbContext.Transactions
                .Where(t => t.AccountNo == "ACC001")
                .ToListAsync();

            Assert.Equal(3, transactions.Count);

            // Vérifier que le client n'a été mis à jour qu'une seule fois
            var clientUpdates = await _dbContext.Clients
                .Where(c => c.AccountNo == "ACC001")
                .Select(c => c.LastTransactionImport)
                .ToListAsync();

            Assert.Single(clientUpdates.Distinct());
        }

        [Fact]
        public async Task ImportAsync_InvalidAccountNo_ShouldSkipTransaction()
        {
            // Arrange
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Transactions");

            // En-têtes
            var headers = new[]
            {
                "AccountNo", "TranDate", "ValueDate", "TranType", "TranAmount", "Balance",
                "TranCode", "TranDescEng", "ReconcileRef", "Narrative1", "Narrative2",
                "Narrative3", "Narrative4", "PostGrpUserId"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Transaction avec AccountNo inexistant
            worksheet.Cell(2, 1).Value = "INVALID_ACC";
            worksheet.Cell(2, 2).Value = "2024-01-15";
            worksheet.Cell(2, 3).Value = "2024-01-16";
            worksheet.Cell(2, 4).Value = "Credit";
            worksheet.Cell(2, 5).Value = 1000;
            worksheet.Cell(2, 6).Value = 5000;
            worksheet.Cell(2, 7).Value = "CHQ";
            worksheet.Cell(2, 8).Value = "Test Transaction";
            worksheet.Cell(2, 9).Value = "REF001";
            worksheet.Cell(2, 10).Value = "Test Narrative";
            worksheet.Cell(2, 11).Value = "Test Details";
            worksheet.Cell(2, 12).Value = "Test Info";
            worksheet.Cell(2, 13).Value = "Test Status";
            worksheet.Cell(2, 14).Value = "USER001";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "transactions", "transactions.xlsx");

            // Act
            var result = await _service.ImportAsync(file);

            // Assert
            Assert.True(result.success);
            Assert.Contains("0 transactions importées", result.message);

            // Aucune transaction ne devrait être ajoutée
            var transactions = await _dbContext.Transactions.ToListAsync();
            Assert.Empty(transactions);
        }

        //[Fact]
        /*public async Task ImportAsync_EmptyFile_ShouldReturnError()
        {
            // Arrange - fichier avec seulement les en-têtes
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Transactions");

            // Seulement l'en-tête
            worksheet.Cell(1, 1).Value = "AccountNo";

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "transactions", "transactions.xlsx");

            // Act
            var result = await _service.ImportAsync(file);

            // Assert
            Assert.False(result.success);
            Assert.Contains("Feuille vide", result.message);
        }*/

        [Fact]
        public async Task ImportAsync_NullFile_ShouldReturnError()
        {
            // Arrange
            IFormFile file = null;

            // Act
            var result = await _service.ImportAsync(file);

            // Assert
            Assert.False(result.success);
            Assert.Contains("Fichier invalide", result.message);
        }

        [Fact]
        public async Task ImportAsync_MultipleClients_ShouldUpdateAllClients()
        {
            // Arrange
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Transactions");

            // En-têtes
            var headers = new[]
            {
                "AccountNo", "TranDate", "ValueDate", "TranType", "TranAmount", "Balance",
                "TranCode", "TranDescEng", "ReconcileRef", "Narrative1", "Narrative2",
                "Narrative3", "Narrative4", "PostGrpUserId"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Transactions pour différents clients
            worksheet.Cell(2, 1).Value = "ACC001";
            worksheet.Cell(3, 1).Value = "ACC002";

            // Remplir les autres colonnes pour les deux transactions
            for (int row = 2; row <= 3; row++)
            {
                worksheet.Cell(row, 2).Value = "2024-01-15";
                worksheet.Cell(row, 3).Value = "2024-01-16";
                worksheet.Cell(row, 4).Value = "Credit";
                worksheet.Cell(row, 5).Value = 1000;
                worksheet.Cell(row, 6).Value = 5000;
                worksheet.Cell(row, 7).Value = "CHQ";
                worksheet.Cell(row, 8).Value = "Transaction";
                worksheet.Cell(row, 9).Value = $"REF{row}";
                worksheet.Cell(row, 10).Value = "Narrative";
                worksheet.Cell(row, 11).Value = "Details";
                worksheet.Cell(row, 12).Value = "Info";
                worksheet.Cell(row, 13).Value = "Status";
                worksheet.Cell(row, 14).Value = "USER001";
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var file = new FormFile(stream, 0, stream.Length, "transactions", "transactions.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            // Act
            var result = await _service.ImportAsync(file);

            // Assert
            Assert.True(result.success);
            Assert.Contains("2 transactions importées", result.message);
            Assert.Contains("2 clients", result.message);

            // Vérifier que les deux clients ont été mis à jour
            var client1 = await _dbContext.Clients.FirstAsync(c => c.AccountNo == "ACC001");
            var client2 = await _dbContext.Clients.FirstAsync(c => c.AccountNo == "ACC002");

            Assert.NotNull(client1.LastTransactionImport);
            Assert.NotNull(client2.LastTransactionImport);

            // Vérifier les transactions
            var transactions = await _dbContext.Transactions.ToListAsync();
            Assert.Equal(2, transactions.Count);
            Assert.Contains(transactions, t => t.AccountNo == "ACC001");
            Assert.Contains(transactions, t => t.AccountNo == "ACC002");
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}