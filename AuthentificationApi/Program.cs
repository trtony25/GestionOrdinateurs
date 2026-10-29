var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClientFactory : client nommé pour appeler GestionOrdinateurs.API (UsersController)
builder.Services.AddHttpClient("GestionOrdinateursApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GestionOrdinateursApi:BaseUrl"]!);
});

// CORS : autorise le client Blazor
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

app.UseAuthorization();

app.MapControllers();

app.Run();
