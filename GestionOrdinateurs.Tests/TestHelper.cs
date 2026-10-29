using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Tests;

public static class TestHelper
{
    public static ParcInformatiqueContext CreerContexte()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    public static Ordinateur NouvelOrdinateur(int id, string marque = "Test", int stock = 1, double prix = 100)
    {
        return new Ordinateur
        {
            Id = id,
            Nom = "PC-" + id,
            Marque = marque,
            Stock = stock,
            Prix = prix,
            Etat = EtatOrdinateur.EnService,
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable, "Intel i5")
        };
    }

    public static ParcInformatiqueContext CreerContexteAvecDonnees()
    {
        var context = CreerContexte();
        context.Ordinateurs.AddRange(
            NouvelOrdinateur(1, "Dell", 5, 1200),
            NouvelOrdinateur(2, "HP", 2, 800),
            NouvelOrdinateur(3, "HP", 0, 3000),
            NouvelOrdinateur(4, "Apple", 1, 2000)
        );
        context.SaveChanges();
        return context;
    }
}