/*using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false)
                  .AddEnvironmentVariables();

builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
//builder.Services.AddScoped<IPdfService, PdfService>();

// Authentification Negotiate
builder.Services.AddAuthentication("Negotiate")
    .AddNegotiate();


builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// Injection de ILdapService
//builder.Services.AddSingleton<ILdapService, LdapService>();
//activer l'authentification windows
builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();*/

var builder = WebApplication.CreateBuilder(args);

// Authentification Windows
builder.Services.AddAuthentication("Negotiate")
    .AddNegotiate();

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Middleware d'authentification
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
