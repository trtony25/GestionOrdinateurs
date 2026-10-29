using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Stockage du token JWT dans la session du navigateur
builder.Services.AddBlazoredSessionStorage();

// Client nommé vers l'API d'authentification (connexion)
builder.Services.AddHttpClient("Auth", client =>
{
    client.BaseAddress = new Uri("http://localhost:5142/");
});

// Client nommé vers la Gateway : tous les appels aux APIs métier passent par elle
builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/");
});

await builder.Build().RunAsync();
