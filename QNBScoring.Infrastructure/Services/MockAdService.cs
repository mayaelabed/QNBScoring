using Microsoft.Extensions.Logging;
using QNBScoring.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace QNBScoring.Infrastructure.Services
{
    public class MockAdService : IAdService
    {
        private readonly ILogger<MockAdService> _logger;
        private readonly Dictionary<string, MockUser> _mockUsers;

        public MockAdService(ILogger<MockAdService> logger)
        {
            _logger = logger;

            // Base de données mock avec utilisateurs de test
            _mockUsers = new Dictionary<string, MockUser>(StringComparer.OrdinalIgnoreCase)
            {
                ["maya"] = new MockUser
                {
                    DistinguishedName = "CN=Maya Doe,OU=Finance,OU=Applications,DC=qnb,DC=local",
                    OrganizationalUnits = new List<string> { "Finance", "Applications", "QNB_Users" }
                },
                ["administrateur"] = new MockUser
                {
                    DistinguishedName = "CN=Administrateur,OU=Admin,OU=IT,DC=qnb,DC=local",
                    OrganizationalUnits = new List<string> { "Admin", "IT", "Domain_Admins" }
                },
                ["testuser"] = new MockUser
                {
                    DistinguishedName = "CN=Test User,OU=Development,OU=IT,DC=qnb,DC=local",
                    OrganizationalUnits = new List<string> { "Development", "IT", "QNB_Users" }
                },
                ["john"] = new MockUser
                {
                    DistinguishedName = "CN=John Smith,OU=HR,OU=Departments,DC=qnb,DC=local",
                    OrganizationalUnits = new List<string> { "HR", "Departments", "QNB_Users" }
                }
            };
        }

        public string GetUserDistinguishedName(string samAccountName)
        {
            _logger.LogInformation("🔧 [MOCK] Recherche DN pour l'utilisateur: {User}", samAccountName);

            if (string.IsNullOrEmpty(samAccountName) || samAccountName.Equals("unknown", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("🔧 [MOCK] Utilisateur inconnu ou non spécifié");
                return null;
            }

            if (_mockUsers.TryGetValue(samAccountName, out var user))
            {
                _logger.LogInformation("🔧 [MOCK] Utilisateur trouvé: {DN}", user.DistinguishedName);
                return user.DistinguishedName;
            }

            // Fallback pour tout nouvel utilisateur
            var newUserDn = $"CN={samAccountName},OU=Users,OU=QNB,DC=qnb,DC=local";
            _logger.LogInformation("🔧 [MOCK] Nouvel utilisateur mocké: {DN}", newUserDn);
            return newUserDn;
        }

        public List<string> GetUserOrganizationalUnits(string samAccountName)
        {
            _logger.LogInformation("🔧 [MOCK] Recherche OUs pour l'utilisateur: {User}", samAccountName);

            if (string.IsNullOrEmpty(samAccountName) || samAccountName.Equals("unknown", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("🔧 [MOCK] Utilisateur inconnu - retour OUs par défaut");
                return new List<string> { "Default_OU", "QNB_Users" };
            }

            if (_mockUsers.TryGetValue(samAccountName, out var user))
            {
                _logger.LogInformation("🔧 [MOCK] OUs trouvées: {OUs}", string.Join(", ", user.OrganizationalUnits));
                return user.OrganizationalUnits;
            }

            // OUs par défaut pour nouveaux utilisateurs
            _logger.LogInformation("🔧 [MOCK] OUs par défaut pour nouvel utilisateur");
            return new List<string> { "Users", "QNB", "Default_Group" };
        }

        // Méthode bonus pour le debug
        public List<string> GetAllMockUsers()
        {
            return new List<string>(_mockUsers.Keys);
        }

        private class MockUser
        {
            public string DistinguishedName { get; set; }
            public List<string> OrganizationalUnits { get; set; }
        }
    }
}