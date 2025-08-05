using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QNBScoring.Infrastructure.Data;
public class ActivitesController : Controller
{
    private readonly QNBScoringDbContext _context;

    public ActivitesController(QNBScoringDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var activites = await _context.Activites
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return View(activites);
    }
}


/*
public class ActivitesController : Controller
{
    private readonly QNBScoringDbContext _context;

    public ActivitesController(QNBScoringDbContext context)
    {
        _context = context;
    }

    // GET: /Activites/Index
    public async Task<IActionResult> Index(string searchUser = null)
    {
        var query = _context.Activites.AsQueryable();

        if (!string.IsNullOrEmpty(searchUser))
        {
            query = query.Where(a => a.Utilisateur.Contains(searchUser));
        }

        // Charger depuis la base de données (sans appel à GetRelativeTime)
        var activitesBrutes = await _context.Activites
            .OrderByDescending(a => a.Date)
            .Take(20)
            .ToListAsync();

        // Transformer en mémoire
        var activites = activitesBrutes.Select(a => new RecentActivity
        {
            Action = a.Action,
            Utilisateur = a.Utilisateur,
            Temps = GetRelativeTime(a.Date),
            Status = a.Status
        }).ToList();

        ViewBag.SearchUser = searchUser;

        return View(activites);
    }

    private static string GetRelativeTime(DateTime date)
    {
        var span = DateTime.Now - date;

        if (span.TotalMinutes < 1)
            return "À l'instant";
        if (span.TotalMinutes < 60)
            return $"Il y a {Math.Floor(span.TotalMinutes)} minute(s)";
        if (span.TotalHours < 24)
            return $"Il y a {Math.Floor(span.TotalHours)} heure(s)";
        return $"Il y a {Math.Floor(span.TotalDays)} jour(s)";
    }

}
}
*/