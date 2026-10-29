using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Client nommé vers l'API d'authentification (URL lue dans wwwroot/appsettings.json)
builder.Services.AddHttpClient("Auth", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuthApiUrl"]!);
});

// Client nommé vers la Gateway : tous les appels aux APIs métier passent par elle
builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GatewayUrl"]!);
});

// Authentification OIDC avec Keycloak (flux "authorization code" + PKCE, avec redirection vers Keycloak)
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = builder.Configuration["Oidc:Authority"];
    options.ProviderOptions.ClientId = builder.Configuration["Oidc:ClientId"];
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
});

await builder.Build().RunAsync();
