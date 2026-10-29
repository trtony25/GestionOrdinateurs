using System.Text.Json.Serialization;

namespace GestionOrdinateurs.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EtatOrdinateur
{
    EnService,
    EnPanne,
    EnMaintenance,
    HorsService,
    Lent,
    Reparable
}