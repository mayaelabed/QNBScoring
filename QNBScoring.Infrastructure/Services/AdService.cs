using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QNBScoring.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Net;

namespace QNBScoring.Infrastructure.Services
{
    public class AdService : IAdService
    {
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private readonly ILogger<AdService> _logger;
        private readonly string _domainControllerIp;

        public AdService(IConfiguration configuration, ILogger<AdService> logger)
        {
            _domain = configuration["Ad:Domain"] ?? throw new ArgumentNullException("Ad:Domain");
            _username = configuration["Ad:Username"];
            _password = configuration["Ad:Password"];
            _domainControllerIp = configuration["Ad:DomainControllerIp"]; // Optionnel : IP du DC
            _logger = logger;
        }

        public string GetUserDistinguishedName(string samAccountName)
        {
            try
            {
                // Essayer d'abord avec le domaine principal
                var result = TryGetUserDN(_domain, samAccountName);
                if (result != null) return result;

                _logger.LogWarning("Utilisateur {User} non trouvé dans le domaine principal {Domain}",
                    samAccountName, _domain);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du DN pour {User}", samAccountName);
                throw new ApplicationException("Erreur d'accès à Active Directory", ex);
            }
        }

        private string TryGetUserDN(string domain, string samAccountName)
        {
            try
            {
                using var context = CreatePrincipalContext(domain);

                var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samAccountName);
                if (user == null)
                {
                    _logger.LogDebug("Utilisateur {User} non trouvé dans le domaine {Domain}",
                        samAccountName, domain);
                    return null;
                }

                var directoryEntry = (DirectoryEntry)user.GetUnderlyingObject();
                return directoryEntry.Properties["distinguishedName"]?.Value?.ToString();
            }
            catch (PrincipalServerDownException ex)
            {
                _logger.LogError(ex, "Serveur Active Directory inaccessible pour le domaine {Domain}", domain);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erreur avec le domaine {Domain}", domain);
                return null;
            }
        }

        private PrincipalContext CreatePrincipalContext(string domain)
        {
            // Si une IP de contrôleur de domaine est spécifiée, l'utiliser
            if (!string.IsNullOrEmpty(_domainControllerIp))
            {
                _logger.LogDebug("Connexion au DC via IP: {IP}", _domainControllerIp);
                return new PrincipalContext(
                    ContextType.Domain,
                    _domainControllerIp,
                    null,
                    ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing,
                    _username,
                    _password
                );
            }

            // Sinon utiliser le nom de domaine normal
            return new PrincipalContext(
                ContextType.Domain,
                domain,
                null,
                ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing,
                _username,
                _password
            );
        }

        public List<string> GetUserOrganizationalUnits(string samAccountName)
        {
            try
            {
                var dn = GetUserDistinguishedName(samAccountName);
                if (string.IsNullOrEmpty(dn))
                {
                    _logger.LogWarning("DN non trouvé pour l'utilisateur {User}", samAccountName);
                    return new List<string>();
                }

                return ExtractOUsFromDN(dn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'extraction des OUs pour {User}", samAccountName);
                throw;
            }
        }

        private List<string> ExtractOUsFromDN(string distinguishedName)
        {
            var ous = new List<string>();

            foreach (var part in distinguishedName.Split(','))
            {
                var trimmed = part.Trim();
                if (trimmed.StartsWith("OU=", StringComparison.OrdinalIgnoreCase))
                {
                    ous.Add(trimmed[3..]); // Enlever "OU="
                }
            }

            ous.Reverse(); // Pour avoir l'ordre hiérarchique correct
            return ous;
        }

        // Méthode de test de connexion
        public bool TestConnection()
        {
            try
            {
                using var context = CreatePrincipalContext(_domain);
                return context.ValidateCredentials(_username, _password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec du test de connexion AD");
                return false;
            }
        }
    }
}