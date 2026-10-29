# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie des .csproj seuls puis restore (couche mise en cache tant que les dépendances ne changent pas)
COPY GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj ./GestionOrdinateurs.Gateway/
RUN dotnet restore ./GestionOrdinateurs.Gateway

# Copie du reste du code source puis publication
COPY GestionOrdinateurs.Gateway/ ./GestionOrdinateurs.Gateway/
RUN dotnet publish ./GestionOrdinateurs.Gateway -c Release -o out --no-restore

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Variables d'environnement (surchargées par docker-compose)
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5010

# Port exposé
EXPOSE 5010

ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
