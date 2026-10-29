using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;

public class User
{
    // Clé primaire
    public int Id { get; set; }

    // Le Login est unique en base (index unique défini dans le DbContext)
    [Required]
    public string Login { get; set; } = string.Empty;

    public RoleEnum Role { get; set; }
}
