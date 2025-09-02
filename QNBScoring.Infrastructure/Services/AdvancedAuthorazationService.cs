using Microsoft.Extensions.Configuration;
using QNBScoring.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace QNBScoring.Infrastructure.Services
{
    public interface IAdvancedAuthorizationService
    {
        bool HasAccessToPage(string username, string controller, string action);
        string GetDefaultPage(string username);
        List<string> GetAllowedPages(string username);
        string GetUserRole(string username);
    }

    public class AdvancedAuthorizationService : IAdvancedAuthorizationService
    {
        private readonly IAdService _adService;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, List<string>> _ouMappings;
        private readonly Dictionary<string, string> _defaultPages;

        public AdvancedAuthorizationService(IAdService adService, IConfiguration configuration)
        {
            _adService = adService;
            _configuration = configuration;
            _ouMappings = GetOUMappings();
            _defaultPages = GetDefaultPages();
        }

        public bool HasAccessToPage(string username, string controller, string action)
        {
            var page = $"{controller}/{action}";
            var allowedPages = GetAllowedPages(username);

            // Accès complet pour les wildcards
            if (allowedPages.Contains("*")) return true;

            // Accès à tout un contrôleur
            if (allowedPages.Contains($"{controller}/*")) return true;

            // Accès spécifique
            return allowedPages.Contains(page);
        }

        public string GetDefaultPage(string username)
        {
            var role = GetUserRole(username);
            return _defaultPages.GetValueOrDefault(role, "Home/Index");
        }

        public List<string> GetAllowedPages(string username)
        {
            var ous = _adService.GetUserOrganizationalUnits(username);
            var allowedPages = new List<string>();

            foreach (var ou in ous)
            {
                if (_ouMappings.TryGetValue(ou, out var pages))
                {
                    allowedPages.AddRange(pages);
                }
            }

            return allowedPages.Distinct().ToList();
        }

        public string GetUserRole(string username)
        {
            var ous = _adService.GetUserOrganizationalUnits(username);

            if (ous.Contains("Admin")) return "Admin";
            if (ous.Contains("Responsables")) return "Responsables";
            if (ous.Contains("IT")) return "IT";
            if (ous.Contains("Agents")) return "Agents";
            if (ous.Contains("Applications")) return "Applications";

            return "Default";
        }

        private Dictionary<string, List<string>> GetOUMappings()
        {
            var mappings = new Dictionary<string, List<string>>();
            var configSection = _configuration.GetSection("Authorization:OUMappings");

            foreach (var ou in configSection.GetChildren())
            {
                mappings[ou.Key] = ou.Get<List<string>>();
            }

            return mappings;
        }

        private Dictionary<string, string> GetDefaultPages()
        {
            var pages = new Dictionary<string, string>();
            var configSection = _configuration.GetSection("Authorization:DefaultPage");

            foreach (var role in configSection.GetChildren())
            {
                pages[role.Key] = role.Value;
            }

            return pages;
        }
    }
}