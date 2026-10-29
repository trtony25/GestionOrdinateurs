using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddDbContext<ParcInformatiqueContext>(options =>
    options.UseSqlite("Data Source=parcinfo.db"));
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();

builder.Services.AddEndpointsApiExplorer();

// Swagger avec le bouton "Authorize" pour saisir le token JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {token}"
    });

    // Sans cette exigence, Swagger n'enverrait pas le token avec les requêtes
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddScoped<ReparationService>();
builder.Services.AddScoped<StockService>();

builder.Services.AddSingleton<JournalService>();

// Configuration JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Utilisateur utilisant un Token Valide --- {context.Principal?.Identity?.Name ?? "Anonymous"}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Token Invalide");
            return Task.CompletedTask;
        }
    };
});

// CORS : la Gateway transmet l'origine du navigateur (client Blazor) à l'API, qui doit donc l'autoriser.
// (Les appels serveur à serveur, comme celui de l'API d'authentification, n'ont pas besoin de CORS.)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5200") // URL du client Blazor
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Activer CORS
app.UseCors("AllowBlazorClient");

// L'authentification doit précéder l'autorisation
app.UseAuthentication();
app.UseAuthorization();

// Active le routage vers les contrôleurs; par ex vers OrdinateursController
app.MapControllers();

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();

    context.Database.Migrate();

    if (!context.Ordinateurs.Any())
    {
        context.Ordinateurs.AddRange(ParcInitializer.Ordinateurs);
        context.SaveChanges();
    }

    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User { Login = "user1", Role = RoleEnum.Admin },
            new User { Login = "user2", Role = RoleEnum.Gestionnaire },
            new User { Login = "user3", Role = RoleEnum.Gestionnaire }
        );

        context.SaveChanges();
    }
}

app.Run();
