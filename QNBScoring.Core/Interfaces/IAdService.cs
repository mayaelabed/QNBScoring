using System.Collections.Generic;

namespace QNBScoring.Core.Interfaces
{
    public interface IAdService
    {
        /// <summary>
        /// Retourne le distinguishedName (DN) de l'utilisateur (ex: CN=maya...,OU=Apps,OU=Finance,DC=contoso,DC=com)
        /// </summary>
        /// </summary>
        string GetUserDistinguishedName(string samAccountName);

        /// <summary>
        /// Retourne les OU extraites du DN (liste).
        /// </summary>
        List<string> GetUserOrganizationalUnits(string samAccountName);
    }
}