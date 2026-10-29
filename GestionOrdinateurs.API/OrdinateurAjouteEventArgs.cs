using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

// Données transmises aux abonnés lorsqu'un ordinateur est ajouté
public class OrdinateurAjouteEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurAjouteEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}