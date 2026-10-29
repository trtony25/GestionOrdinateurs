using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class ParcInformatiqueContext : DbContext
{
    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    public DbSet<Ordinateur> Ordinateurs => Set<Ordinateur>();
    public DbSet<Reparation> Reparations => Set<Reparation>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Le Login doit être unique en base de données
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<Ordinateur>()
            .Property(o => o.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur);
    }
}
