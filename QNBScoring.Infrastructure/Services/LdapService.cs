using Microsoft.AspNetCore.Authentication;
using QNBScoring.Core.Interfaces;
using System.DirectoryServices.AccountManagement;

namespace QNBScoring.Infrastructure.Services
{
    
        public class LdapService : ILdapService
        {
            public bool VerifierMotDePasse(string username, string password)
            {
                // Format DN complet de l'utilisateur LDAP
                string userDn = $"uid={username},ou=users,dc=qnb,dc=tn";

                using var context = new PrincipalContext(
                    ContextType.ApplicationDirectory,
                    "ldap://localhost:389",          // ou "ldap://qnb-ldap:389" dans Docker
                    "dc=qnb,dc=tn",                  // Base DN
                    "cn=admin,dc=qnb,dc=tn",         // DN admin
                    "admin"                          // Mot de passe admin
                );

                return context.ValidateCredentials(userDn, password, ContextOptions.SimpleBind);
            }
        }
    

}
