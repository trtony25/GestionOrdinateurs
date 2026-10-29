var builder = WebApplication.CreateBuilder(args);

// YARP : les routes et clusters sont lus dans la section "ReverseProxy" de appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Redirige les requêtes vers les APIs selon les routes configurées.
// Pas de CORS ici : les requêtes (et le preflight OPTIONS) sont transmises à l'API, qui gère le CORS.
app.MapReverseProxy();

app.Run();
