using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _context;

    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // GET api/users : récupérer tous les utilisateurs
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Users.ToListAsync());
    }

    // GET api/users/bylogin/{login} : chercher un utilisateur par login, 404 si inconnu
    [HttpGet("bylogin/{login}")]
    public async Task<IActionResult> GetByLogin(string login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user is null)
            return NotFound();

        return Ok(user);
    }
}
