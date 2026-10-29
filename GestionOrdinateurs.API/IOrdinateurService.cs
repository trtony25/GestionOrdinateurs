using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public interface IOrdinateurService
{
    Task<List<Ordinateur>> GetAllAsync();
    Task<Ordinateur?> GetByIdAsync(int id);
    Task<bool> AddAsync(Ordinateur ordinateur);
    Task<bool> UpdateAsync(int id, Ordinateur ordinateur);
    Task<bool> DeleteAsync(int id);
    Task<List<Ordinateur>> GetDisponiblesAsync();
    Task<List<Ordinateur>> GetByMarqueAsync(string marque);
    Task<List<Ordinateur>> GetParPrixDecroissantAsync();

    string Diagnostiquer(Ordinateur ordinateur);
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
}