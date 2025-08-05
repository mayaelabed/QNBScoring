using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Interfaces;
using QNBScoring.Infrastructure.Data;
using QNBScoring.Infrastructure.Repositories;
using QNBScoring.Infrastructure.Services;
using QuestPDF.Infrastructure;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

// EF Core
builder.Services.AddDbContext<QNBScoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories (register each only once)
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IDemandeChequierRepository, DemandeChequierRepository>();
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
builder.Services.AddScoped<ITransactionBancaireRepository, TransactionBancaireRepository>();
builder.Services.AddScoped<IActiviteService, ActiviteService>();

// Services (register each only once)
builder.Services.AddScoped<IDemandeService, DemandeService>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<ExcelImportService>();
builder.Services.AddScoped<PdfDemandeService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<ILdapService, LdapService>();
builder.Services.AddScoped<ISessionLogger, SessionLogger>();


// Other configurations
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
// Authentication 
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
    });



var app = builder.Build();
QuestPDF.Settings.License = LicenseType.Community;

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseCors("AllowAll");
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Demande}/{action=Index}/{id?}");*/
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "import",
    pattern: "Import/{action=Index}",
    defaults: new { controller = "Import" });
app.Run();
