using Xunit;
using Moq;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ScoringServiceTests
{
    [Fact]
    public async Task CalculateScoreAsync_ShouldAssign40Points_ForPerfectClient_NoTransactions()
    {
        // Arrange
        var client = new Client
        {
            ADesIncidents = false,
            DateEntreeEnRelation = DateTime.Now.AddYears(-4),
            ClassificationSED = "A",
            AccountNo = "ACC123"
        };

        var demande = new DemandeChequier
        {
            TypeChequier = "Professionnel",
            NombreChequiers = 2,
            Id = 1
        };

        var transactionRepoMock = new Mock<ITransactionBancaireRepository>();
        transactionRepoMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<TransactionBancaire>());

        var scoringService = new ScoringService(transactionRepoMock.Object);

        // Act
        var result = await scoringService.CalculerScoreAsync(client, demande);

        // Assert
        Assert.Equal(40, result.Valeur);
        Assert.Equal("Refusé", result.Decision);
        Assert.Equal("Profil à risque - demande refusée", result.Commentaire);
    }

    [Fact]
    public async Task CalculateScoreAsync_ShouldPenalizeClient_WithNegativeTransaction()
    {
        // Arrange
        var client = new Client
        {
            ADesIncidents = false,
            DateEntreeEnRelation = DateTime.Now.AddYears(-2),
            ClassificationSED = "B",
            AccountNo = "ACC789"
        };

        var demande = new DemandeChequier
        {
            TypeChequier = "Personnel",
            NombreChequiers = 1,
            Id = 3
        };

        var transactions = new List<TransactionBancaire>
        {
            new TransactionBancaire
            {
                AccountNo = "ACC789",
                TranDate = DateTime.Now.AddDays(-1),
                ValueDate = DateTime.Now.AddDays(-1),
                TranType = "DEBIT",
                TranAmount = -5000, // Retrait important
                Balance = 300,
                TranCode = "002",
                TranDescEng = "Withdrawal",
                ReconcileRef = "REF002",
                Narrative1 = "ATM"
            }
        };

        var transactionRepoMock = new Mock<ITransactionBancaireRepository>();
        transactionRepoMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(transactions);

        var scoringService = new ScoringService(transactionRepoMock.Object);

        // Act
        var result = await scoringService.CalculerScoreAsync(client, demande);

        // Assert
        Assert.True(result.Valeur <= 40); // Retrait important = risque
        Assert.Equal("Refusé", result.Decision);
    }
}
