using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.DTOs;
using QNBScoring.Core.Models;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ClientRequest> ClientRequests { get; set; }
    public DbSet<ClientScore> ClientScores { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClientRequest>()
                .Property(c => c.Solde)
                .HasColumnType("decimal(18,2)");
    }

}
