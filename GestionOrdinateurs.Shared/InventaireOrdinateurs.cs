namespace GestionOrdinateurs.Shared;

// Collection spécialisée avec deux indexeurs
public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    // inventaire[1] : recherche par Id
    public Ordinateur? this[int ordinateurId]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Id == ordinateurId); }
    }

    // inventaire["PC-Dev-01"] : recherche par nom
    public Ordinateur? this[string nom]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Nom == nom); }
    }
}