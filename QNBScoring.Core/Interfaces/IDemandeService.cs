// QNBScoring.Core/Interfaces/IDemandeService.cs
using Microsoft.AspNetCore.Http;
using QNBScoring.Core.Entities;

public interface IDemandeService
{
    Task<(IEnumerable<DemandeChequier> Demandes, DemandeStats Stats)> GetDemandesAsync(string search, string type);
    Task<int> CreateDemandeAsync(DemandeChequier model, IFormFile pieceIdentiteFile,
                                   IFormFile justificatifFile, string accountNo); Task<DemandeChequier> GetDemandeDetailsAsync(int id);
    Task<byte[]> GeneratePdfAsync(int id);
    Task<bool> UpdateDemandeAsync(DemandeChequier model, IFormFile? pieceIdentiteFile, IFormFile? justificatifFile);

}