using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class ReparationService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journal;

    public ReparationService(ParcInformatiqueContext context, JournalService journal)
    {
        _context = context;
        _journal = journal;
    }

    public record HistoriqueReparationRecord(
        int OrdinateurId,
        string NomOrdinateur,
        string Technicien,
        string TypeReparation,
        DateTime DateIntervention
    );

    public async Task<List<Reparation>> GetReparationsAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();
    }

    // Historique des réparations sous forme de records
    public async Task<List<HistoriqueReparationRecord>> GetHistoriqueAsync()
    {
        return await _context.Reparations
            .Select(r => new HistoriqueReparationRecord(
                r.OrdinateurId,
                r.UnOrdinateur.Nom,
                r.Technicien,
                r.TypeReparation,
                r.DateDebut))
            .ToListAsync();
    }

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = await CreerReparationAsync(ordinateur);

        await Task.Delay(2000);

        reparation.Statut = "Terminée";
        await _context.SaveChangesAsync();

        Console.WriteLine($"{ordinateur.Nom} réparé");
        _journal.Ecrire("Reparations", $"{ordinateur.Nom} réparé");
    }

    private async Task<Reparation> CreerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Remplacement SSD",
            DateDebut = DateTime.Now,
            Technicien = ordinateur.Id % 2 == 0 ? "Alice" : "Marc",
            Statut = "En cours"
        };

        _context.Reparations.Add(reparation);
        await _context.SaveChangesAsync();
        return reparation;
    }
}