using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

// Corps de la requête de connexion : seulement le login (pas de mot de passe dans ce TP)
public record LoginRequest(string Login);

// Corps de la requête de connexion Keycloak : login + mot de passe
public record KeycloakLoginRequest(string Login, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // POST api/authentication/login : renvoie un token JWT valable 15 minutes
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Récupère l'utilisateur auprès de GestionOrdinateurs.API via HttpClientFactory
        var client = _httpClientFactory.CreateClient("GestionOrdinateursApi");
        var response = await client.GetAsync($"api/users/bylogin/{Uri.EscapeDataString(request.Login)}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return NotFound("L'utilisateur n'existe pas");

        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user is null)
            return NotFound("L'utilisateur n'existe pas");

        // Claims : nom et rôle de l'utilisateur
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }

    // POST api/authentication/keycloak-login : échange login/mot de passe contre un token Keycloak
    // (flux "password" OIDC). Le login historique ci-dessus est conservé.
    [HttpPost("keycloak-login")]
    public async Task<IActionResult> KeycloakLogin([FromBody] KeycloakLoginRequest request)
    {
        var tokenUrl = $"{_configuration["Keycloak:BaseUrl"]}/realms/{_configuration["Keycloak:Realm"]}/protocol/openid-connect/token";

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _configuration["Keycloak:ClientId"]!,
            ["username"] = request.Login,
            ["password"] = request.Password
        }));

        if (!response.IsSuccessStatusCode)
            return Unauthorized("Identifiants Keycloak invalides");

        // On renvoie uniquement l'access_token, comme le login historique renvoie le JWT
        var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        return Ok(json.GetProperty("access_token").GetString());
    }
}
