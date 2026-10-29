using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Données transmises aux abonnés lorsqu'un ordinateur est supprimé
public class OrdinateurSupprimeEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}