using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API.Controllers;

[Authorize] // tout utilisateur authentifié (Gestionnaire ou Admin)
[ApiController]
[Route("api/[controller]")]
public class ReparationsController : ControllerBase
{
    private readonly ReparationService _reparService;
    private readonly IOrdinateurService _ordinateurService;
    private readonly StockService _stockService;
    private readonly ILogger<ReparationsController> _logger;

    public ReparationsController(
        ReparationService reparService,
        IOrdinateurService ordinateurService,
        StockService stockService,
        ILogger<ReparationsController> logger)
    {
        _reparService = reparService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
        _logger = logger;
    }

    // POST api/reparations/SimulerReparation : 3 réparations en parallèle
    [HttpPost("SimulerReparation")]
    [Authorize(Roles = "Admin")] // seul un Admin peut lancer une simulation
    public async Task<IActionResult> SimulerReparation()
    {
        var listOrdinateurs = await _ordinateurService.GetAllAsync();
        

        await Task.WhenAll(
            _reparService.SimulerReparationAsync(listOrdinateurs[0]),
            _reparService.SimulerReparationAsync(listOrdinateurs[1]),
            _reparService.SimulerReparationAsync(listOrdinateurs[2])
        );

        return Ok("Réparations terminées");
    }

    // GET api/reparations : réparations + infos de l'ordinateur
    [HttpGet]
    public async Task<IActionResult> GetReparations()
    {
        return Ok(await _reparService.GetReparationsAsync());
    }

    // GET api/reparations/GetStockSSD : 10 consommations en parallèle
    [HttpGet("GetStockSSD")]
    public IActionResult GetStockSSD()
    {
        Parallel.For(0, 10, i => _stockService.ConsommerSSD());

        _logger.LogInformation("StockSSD = {Stock}", _stockService.StockSSD);
        return Ok(_stockService.StockSSD);
    }

        // GET api/reparations/historique
    [HttpGet("historique")]
    public async Task<IActionResult> GetHistorique()
    {
        return Ok(await _reparService.GetHistoriqueAsync());
    }
}
