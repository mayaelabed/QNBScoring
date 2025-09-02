// ClientListViewModel.cs
using System;

namespace QNBScoring.Core.ViewModels
{
    public class ClientListViewModel
    {
        public int Id { get; set; }
        public string AccountNo { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public DateTime? LastImportDate { get; set; }
        public int TransactionCount { get; set; }

        // Propriété calculée pour le nom complet
        public string NomComplet => $"{Nom} {Prenom}";
    }
}