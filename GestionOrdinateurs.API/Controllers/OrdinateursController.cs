using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API.Controllers;

[Authorize] // tout utilisateur authentifié (Gestionnaire ou Admin)
[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _service;
    private readonly ILogger<OrdinateursController> _logger;

    public OrdinateursController(IOrdinateurService service, ILogger<OrdinateursController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET api/ordinateurs
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Ordinateur>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Récupération de la liste de tous les ordinateurs.");
        return Ok(await _service.GetAllAsync());
    }

    // GET api/ordinateurs/disponibles : uniquement ceux dont le stock est > 0
    [HttpGet("disponibles")]
    [ProducesResponseType(typeof(IEnumerable<Ordinateur>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisponibles()
    {
        _logger.LogInformation("Récupération des ordinateurs disponibles.");
        return Ok(await _service.GetDisponiblesAsync());
    }

    // GET api/ordinateurs/marque/{marque}
    [HttpGet("marque/{marque}")]
    [ProducesResponseType(typeof(IEnumerable<Ordinateur>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMarque(string marque)
    {
        _logger.LogInformation("Recherche des ordinateurs de la marque {Marque}.", marque);
        return Ok(await _service.GetByMarqueAsync(marque));
    }

    // GET api/ordinateurs/par-prix : du plus cher au moins cher
    [HttpGet("par-prix")]
    [ProducesResponseType(typeof(IEnumerable<Ordinateur>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetParPrix()
    {
        _logger.LogInformation("Récupération des ordinateurs du plus cher au moins cher.");
        return Ok(await _service.GetParPrixDecroissantAsync());
    }

    // GET api/ordinateurs/{id} : 404 si non trouvé
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Ordinateur), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComputerById(int id)
    {
        _logger.LogInformation("Récupération de l'ordinateur avec l'Id {Id}.", id);

        var ordinateur = await _service.GetByIdAsync(id);
        if (ordinateur is null)
        {
            _logger.LogWarning("Ordinateur avec l'Id {Id} introuvable.", id);
            return NotFound();
        }

        return Ok(ordinateur);
    }

    // POST api/ordinateurs : 201 si créé, 409 si l'Id existe déjà
    [HttpPost]
    [ProducesResponseType(typeof(Ordinateur), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] Ordinateur ordinateur)
    {
        _logger.LogInformation("Tentative de création d'un ordinateur avec l'Id {Id}.", ordinateur.Id);

        if (ordinateur.Id <= 0)
        {
            _logger.LogWarning("Création refusée : Id invalide ({Id}).", ordinateur.Id);
            return BadRequest("L'Id doit être strictement positif.");
        }

        if (ordinateur.Stock < 0 || ordinateur.Prix < 0)
        {
            _logger.LogWarning("Création refusée : stock ou prix négatif.");
            return BadRequest("Le stock et le prix ne peuvent pas être négatifs.");
        }

        if (!await _service.AddAsync(ordinateur))
        {
            _logger.LogWarning("Création refusée : l'Id {Id} existe déjà.", ordinateur.Id);
            return Conflict($"Un ordinateur avec l'Id {ordinateur.Id} existe déjà.");
        }

        _logger.LogInformation("Ordinateur {Id} créé avec succès.", ordinateur.Id);
        return CreatedAtAction(nameof(GetComputerById), new { id = ordinateur.Id }, ordinateur);
    }

    // PUT api/ordinateurs/{id} : 200 si mis à jour, 404 si non trouvé
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Ordinateur), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Ordinateur ordinateur)
    {
        _logger.LogInformation("Tentative de mise à jour de l'ordinateur {Id}.", id);

        if (ordinateur.Id != id)
        {
            _logger.LogWarning("Mise à jour refusée : Id de l'URL ({UrlId}) différent de l'Id du corps ({BodyId}).", id, ordinateur.Id);
            return BadRequest("L'Id de l'URL doit correspondre à l'Id du corps de la requête.");
        }

        if (ordinateur.Stock < 0 || ordinateur.Prix < 0)
        {
            _logger.LogWarning("Mise à jour refusée : stock ou prix négatif.");
            return BadRequest("Le stock et le prix ne peuvent pas être négatifs.");
        }

        if (!await _service.UpdateAsync(id, ordinateur))
        {
            _logger.LogWarning("Mise à jour refusée : ordinateur {Id} introuvable.", id);
            return NotFound();
        }

        _logger.LogInformation("Ordinateur {Id} mis à jour avec succès.", id);
        return Ok(ordinateur);
    }

    // DELETE api/ordinateurs/{id} : 204 si supprimé, 404 si non trouvé
    [Authorize(Roles = "Admin")] // seul un Admin peut supprimer
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Tentative de suppression de l'ordinateur {Id}.", id);

        if (!await _service.DeleteAsync(id))
        {
            _logger.LogWarning("Suppression refusée : ordinateur {Id} introuvable.", id);
            return NotFound();
        }

        _logger.LogInformation("Ordinateur {Id} supprimé avec succès.", id);
        return NoContent();
    }

        // GET api/ordinateurs/{id}/diagnostic
    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> GetDiagnostic(int id)
    {
        var ordinateur = await _service.GetByIdAsync(id);
        if (ordinateur is null)
            return NotFound();

        return Ok(_service.Diagnostiquer(ordinateur));
    }

    // GET api/ordinateurs/statistiques
    [HttpGet("statistiques")]
    public IActionResult GetStatistiques()
    {
        var (total, enStock, valeurStock, nbReparations) = _service.CalculerStatistiques();

        return Ok(new { total, enStock, valeurStock, nbReparations });
    }

    // GET api/ordinateurs/inventaire/{nom} : recherche avec l'indexeur
    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> Chercher(string nom)
    {
        var inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = await _service.GetAllAsync();

        var ordinateur = inventaire[nom];
        if (ordinateur is null)
            return NotFound();

        return Ok(ordinateur);
    }
}
