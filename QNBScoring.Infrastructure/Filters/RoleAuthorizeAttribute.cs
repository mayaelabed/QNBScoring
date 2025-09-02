using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Services;
using System;

namespace QNBScoring.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _requiredRole;
        private readonly string _requiredPage;

        public RoleAuthorizeAttribute(string requiredRole = null, string requiredPage = null)
        {
            _requiredRole = requiredRole;
            _requiredPage = requiredPage;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authService = context.HttpContext.RequestServices.GetService<IAdvancedAuthorizationService>();
            var adService = context.HttpContext.RequestServices.GetService<IAdService>();

            var identity = context.HttpContext.User.Identity;
            if (identity == null || !identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var username = identity.Name;
            var sam = username.Contains("\\") ? username.Split('\\').Last() : username;

            // Vérification du rôle
            if (!string.IsNullOrEmpty(_requiredRole))
            {
                var userRole = authService.GetUserRole(sam);
                if (userRole != _requiredRole)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            // Vérification de la page
            if (!string.IsNullOrEmpty(_requiredPage))
            {
                var controller = context.RouteData.Values["controller"]?.ToString();
                var action = context.RouteData.Values["action"]?.ToString();

                if (!authService.HasAccessToPage(sam, controller, action))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }

            // Redirection vers la page par défaut si accès non autorisé
            var currentController = context.RouteData.Values["controller"]?.ToString();
            var currentAction = context.RouteData.Values["action"]?.ToString();
            var currentPage = $"{currentController}/{currentAction}";

            if (!authService.HasAccessToPage(sam, currentController, currentAction))
            {
                var defaultPage = authService.GetDefaultPage(sam);
                var parts = defaultPage.Split('/');

                context.Result = new RedirectToActionResult(
                    parts[1],
                    parts[0],
                    null
                );
            }
        }
    }
}