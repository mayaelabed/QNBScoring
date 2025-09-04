using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
// using QNBScoring.Core.Configurations;   // ?? EMAIL: contient EmailSettings (désactivé)
// using QNBScoring.Infrastructure.Services; // ?? EMAIL: contient EmailService (désactivé)
using QNBScoring.Core.Interfaces;
using QNBScoring.Core.Security;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Repositories;
using QNBScoring.Infrastructure.Security;
using QNBScoring.Infrastructure.Services;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Net;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// 1) Auth Windows (Negotiate) - UNIQUEMENT UNE FOIS !
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate(options =>
    {
        options.PersistKerberosCredentials = true;
    });

// 2) Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FinanceOU", policy =>
        policy.Requirements.Add(new OuRequirement("Finance")));

    // ?? NOUVELLE POLITIQUE POUR ADMIN
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
        {
            var identity = context.User.Identity as WindowsIdentity;
            var username = identity?.Name ?? "";
            var sam = username.Contains("\\") ? username.Split('\\').Last() : username;
            return sam.Equals("maya", StringComparison.OrdinalIgnoreCase);
        }));
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
    });

    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

// 3) DI
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QNB Scoring API",
        Version = "v1",
        Description = "API for QNB Scoring Application with AD Integration"
    });
});

// ?? CONFIGURATION CRITIQUE : Choix du service AD
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IAdService, MockAdService>();
    builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication.Negotiate", LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication.NegotiateHandler", LogLevel.Warning);
}
else
{
    builder.Services.AddScoped<IAdService, AdService>();
}

// Configuration CORS pour le développement
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// MVC
builder.Services.AddControllersWithViews();
System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

// EF Core
builder.Services.AddDbContext<QNBScoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IDemandeChequierRepository, DemandeChequierRepository>();
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<ITransactionBancaireRepository, TransactionBancaireRepository>();
builder.Services.AddScoped<IActiviteService, ActiviteService>();

// Services
builder.Services.AddScoped<IDemandeService, DemandeService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<ExcelImportService>();
builder.Services.AddScoped<PdfDemandeService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<IAuthorizationHandler, OuRequirementHandler>();

// ?? EMAIL: injection et configuration désactivées
// builder.Services.AddScoped<IEmailService, EmailService>();
// builder.Services.Configure<EmailSettings>(
//     builder.Configuration.GetSection("EmailSettings"));
// builder.Services.AddScoped<IEmailService, EmailService>();

System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, errors) => true;

// Add authorization with OU policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OuPolicy", policy =>
        policy.Requirements.Add(new OuRequirement("RequiredOUName")));
});

// Other configurations
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QNB Scoring API v1");
        c.RoutePrefix = "swagger";
    });

    app.UseCors("AllowAll");

    // Middleware de simulation d'authentification en développement
    app.Use(async (context, next) =>
    {
        // Simuler un utilisateur Windows authentifié
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            try
            {
                var windowsIdentity = new System.Security.Principal.WindowsIdentity("maya@qnb.local");
                context.User = new System.Security.Principal.WindowsPrincipal(windowsIdentity);
                app.Logger.LogInformation("?? [MOCK] Utilisateur simulé: maya@qnb.local");
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(ex, "?? [MOCK] Erreur lors de la simulation utilisateur");
            }
        }
        await next();
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// ?? Point de contrôle pour vérifier la configuration
app.Logger.LogInformation("? Configuration terminée - Mode: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("? Service AD utilisé: {ServiceType}",
    app.Environment.IsDevelopment() ? "MockAdService" : "AdService");

// Endpoints
app.MapGet("/whoami", (HttpContext ctx) =>
{
    return $"Hello {ctx.User.Identity?.Name}, AuthType: {ctx.User.Identity?.AuthenticationType}";
}).RequireAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "import",
    pattern: "Import/{action=Index}",
    defaults: new { controller = "Import" });

app.Run();
