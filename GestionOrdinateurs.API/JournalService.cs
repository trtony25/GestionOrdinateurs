using System.Globalization;

namespace GestionOrdinateurs.API;

public class JournalService
{
    // Verrou : plusieurs réparations peuvent écrire en même temps dans le même fichier
    private static readonly object _verrou = new();

    public void Ecrire(string dossier, string message)
    {
        string chemin = Path.Combine("Logs", dossier);
        Directory.CreateDirectory(chemin);

        string date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        lock (_verrou)
        {
            using var writer = new StreamWriter(Path.Combine(chemin, dossier + ".log"), append: true);
            writer.WriteLine($"{date} - {message}");
        }
    }
}