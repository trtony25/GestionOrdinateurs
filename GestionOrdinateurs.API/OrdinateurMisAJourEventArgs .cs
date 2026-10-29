using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Données transmises aux abonnés lorsqu'un ordinateur est mis à jour
public class OrdinateurMisAJourEventArgs : EventArgs
{
    public Ordinateur AncienOrdinateur { get; }
    public Ordinateur NouvelOrdinateur { get; }

    public OrdinateurMisAJourEventArgs(Ordinateur ancienOrdinateur, Ordinateur nouvelOrdinateur)
    {
        AncienOrdinateur = ancienOrdinateur;
        NouvelOrdinateur = nouvelOrdinateur;
    }
}