using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Services;
using System.IO;
using System.Text;
using Xunit;

public class DemandeServiceTests : IDisposable
{
    private readonly QNBScoringDbContext _context;
    private readonly DemandeService _service;
    private const string TestAccountNo = "TEST123";

    public DemandeServiceTests()
    {
        var options = new DbContextOptionsBuilder<QNBScoringDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_DB_" + Guid.NewGuid())
            .Options;

        _context = new QNBScoringDbContext(options);

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(m => m.WebRootPath).Returns(Path.GetTempPath());

        _service = new DemandeService(
            _context,
            mockEnv.Object,
            new Mock<PdfDemandeService>().Object,
            new Mock<ILogger<DemandeService>>().Object,
            new Mock<ITransactionBancaireRepository>().Object);

        // Données de test
        _context.Clients.Add(new Client
        {
            Id = 1,
            AccountNo = TestAccountNo,
            Nom = "Test",
            Prenom = "User"
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateDemandeAsync_ShouldCreateDemande()
    {
        // Arrange
        var demande = new DemandeChequier
        {
            TypeChequier = "Standard",
            NombreChequiers = 1,
            Email = "test@example.com",
            DeclarationVeracite = true,
            ConditionsAcceptees = true
        };

        var mockFile = CreateMockFile();

        // Act
        var result = await _service.CreateDemandeAsync(
            demande, mockFile, mockFile, TestAccountNo);

        // Assert
        Assert.True(result > 0);
        var createdDemande = await _context.Demandes.FindAsync(result);
        Assert.NotNull(createdDemande);
    }

    private IFormFile CreateMockFile()
    {
        var content = "Fake file content";
        var fileName = "test.pdf";
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");

        return fileMock.Object;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}