using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class OrdinateurService : IOrdinateurService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journal;

    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    public OrdinateurService(ParcInformatiqueContext context, JournalService journal)
    {
        _context = context;
        _journal = journal;

        OrdinateurAjoute += AfficherMessageAjout;
        OrdinateurSupprime += AfficherMessageSuppression;
        OrdinateurMisAJour += AfficherMessageMiseAJour;
    }

    public async Task<List<Ordinateur>> GetAllAsync()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    public async Task<Ordinateur?> GetByIdAsync(int id)
    {
        return await _context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Ordinateur>> GetDisponiblesAsync()
    {
        return await _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    public async Task<List<Ordinateur>> GetByMarqueAsync(string marque)
    {
        return await _context.Ordinateurs
            .Where(o => o.Marque == marque)
            .ToListAsync();
    }

    public async Task<List<Ordinateur>> GetParPrixDecroissantAsync()
    {
        return await _context.Ordinateurs
            .OrderByDescending(o => o.Prix)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(Ordinateur ordinateur)
    {
        if (await _context.Ordinateurs.AnyAsync(o => o.Id == ordinateur.Id))
            return false;

        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        return true;
    }

    public async Task<bool> UpdateAsync(int id, Ordinateur ordinateur)
    {
        var existant = await GetByIdAsync(id);
        if (existant is null)
            return false;

        // copie légère de l'ancien pour l'évènement
        var ancien = new Ordinateur { Id = existant.Id, Nom = existant.Nom };

        existant.Nom = ordinateur.Nom;
        existant.Marque = ordinateur.Marque;
        existant.Etat = ordinateur.Etat;
        existant.Stock = ordinateur.Stock;
        existant.Prix = ordinateur.Prix;
        existant.Caracteristiques.RamGo = ordinateur.Caracteristiques.RamGo;
        existant.Caracteristiques.StockageGo = ordinateur.Caracteristiques.StockageGo;
        existant.Caracteristiques.TypeOrdi = ordinateur.Caracteristiques.TypeOrdi;
        existant.Caracteristiques.Processeur = ordinateur.Caracteristiques.Processeur;
        await _context.SaveChangesAsync();

        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(ancien, existant));
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ordinateur = await GetByIdAsync(id);
        if (ordinateur is null)
            return false;

        _context.Ordinateurs.Remove(ordinateur);
        await _context.SaveChangesAsync();

        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));
        return true;
    }

    // Diagnostic par pattern matching
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService } => "Remplacement",

            { Etat: EtatOrdinateur.Lent, Caracteristiques: { RamGo: < 8 } } => "Ajouter RAM",

            _ => "Aucune action"
        };
    }

    // Statistiques renvoyées sous forme de tuple nommé
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques()
    {
        int total = _context.Ordinateurs.Count();
        int disponibles = _context.Ordinateurs.Count(o => o.Stock > 0);
        decimal valeurStock = (decimal)_context.Ordinateurs.Sum(o => o.Stock * o.Prix);
        int nbReparations = _context.Reparations.Count();

        return (total, disponibles, valeurStock, nbReparations);
    }

    // affichage console + écriture dans le journal

    private void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        string message = $"Ordinateur ajouté : #{e.Ordinateur.Id} {e.Ordinateur.Nom}";
        Console.WriteLine($"[ÉVÈNEMENT] {message}");
        _journal.Ecrire("Ordinateurs", message);
    }

    private void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        string message = $"Ordinateur supprimé : #{e.Ordinateur.Id} {e.Ordinateur.Nom}";
        Console.WriteLine($"[ÉVÈNEMENT] {message}");
        _journal.Ecrire("Ordinateurs", message);
    }

    private void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        string message = $"Ordinateur mis à jour : #{e.NouvelOrdinateur.Id} " +
                         $"\"{e.AncienOrdinateur.Nom}\" -> \"{e.NouvelOrdinateur.Nom}\"";
        Console.WriteLine($"[ÉVÈNEMENT] {message}");
        _journal.Ecrire("Ordinateurs", message);
    }
}