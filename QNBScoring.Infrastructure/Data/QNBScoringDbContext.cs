using Microsoft.EntityFrameworkCore;
using QNBScoring.Core.Entities;
using QNBScoring.Core.Interfaces;

namespace QNBScoring.Infrastructure.Data
{
    public class QNBScoringDbContext : DbContext
    {
        public QNBScoringDbContext(DbContextOptions<QNBScoringDbContext> options)
            : base(options)
        {
        }
       public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<DemandeChequier> Demandes { get; set; }
        //public DbSet<Client> Clients => Set<Client>();
        //public DbSet<DemandeChequier> Demandes => Set<DemandeChequier>();
        public DbSet<TransactionBancaire> Transactions => Set<TransactionBancaire>();
        public DbSet<Score> Scores => Set<Score>();
        public DbSet<SessionUtilisateur> SessionsUtilisateurs => Set<SessionUtilisateur>();
        public DbSet<UtilisateurApp> UtilisateursApp { get; set; } = null!;
        public DbSet<Activite> Activites { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Client>()
        .HasKey(c => c.Id); // Clé primaire par défaut

            // ✅ Définir AccountNo comme clé alternative
            modelBuilder.Entity<Client>()
                .HasAlternateKey(c => c.AccountNo);

            modelBuilder.Entity<TransactionBancaire>()
               .HasOne(t => t.Client)
               .WithMany(c => c.Transactions)
               .HasForeignKey(t => t.AccountNo)
               .HasPrincipalKey(c => c.AccountNo) // lien via AccountNo, pas Id
               .OnDelete(DeleteBehavior.Restrict); // ou .Cascade selon ton besoin

            // Ensure AccountNo is unique for Clients
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.AccountNo)
                .IsUnique();
            modelBuilder.Entity<TransactionBancaire>()
                .Property(t => t.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TransactionBancaire>()
                .Property(t => t.TranAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Demandes)
                .WithOne(d => d.Client)
                .HasForeignKey(d => d.ClientId);

            modelBuilder.Entity<DemandeChequier>()
                .HasOne(d => d.Score)
                .WithOne(s => s.Demande)
                .HasForeignKey<Score>(s => s.DemandeChequierId);
        }
    }
}
