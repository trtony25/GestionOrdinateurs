namespace GestionOrdinateurs.Shared;

public static class ParcInitializer
{
    public static List<Ordinateur> Ordinateurs { get; } = new()
    {
        new Ordinateur { Id = 1, Nom = "PC-Dev-01", Marque = "Dell", Etat = EtatOrdinateur.EnService, Stock = 5, Prix = 1200,
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable, "Intel Core i7") },
        new Ordinateur { Id = 2, Nom = "PC-Compta-01", Marque = "HP", Etat = EtatOrdinateur.EnMaintenance, Stock = 2, Prix = 800,
            Caracteristiques = new Caracteristiques(8, 256, TypeOrdinateur.Portable, "Intel Core i5") },
        new Ordinateur { Id = 3, Nom = "PC-Direction-01", Marque = "Apple", Etat = EtatOrdinateur.EnService, Stock = 1, Prix = 2000,
            Caracteristiques = new Caracteristiques(32, 1024, TypeOrdinateur.Portable, "Apple M2") },
        new Ordinateur { Id = 4, Nom = "PC-Design-01", Marque = "Lenovo", Etat = EtatOrdinateur.Reparable, Stock = 3, Prix = 1500,
            Caracteristiques = new Caracteristiques(32, 1024, TypeOrdinateur.Fixe, "AMD Ryzen 9") },
        new Ordinateur { Id = 5, Nom = "PC-Accueil-01", Marque = "Asus", Etat = EtatOrdinateur.Lent, Stock = 0, Prix = 500,
            Caracteristiques = new Caracteristiques(4, 500, TypeOrdinateur.Fixe, "Intel Core i3") },
        new Ordinateur { Id = 6, Nom = "PC-Serveur-01", Marque = "HP", Etat = EtatOrdinateur.HorsService, Stock = 0, Prix = 3000,
            Caracteristiques = new Caracteristiques(64, 2000, TypeOrdinateur.Server, "Intel Xeon") }
    };
}