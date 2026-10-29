# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie des .csproj seuls puis restore (couche mise en cache tant que les dépendances ne changent pas)
COPY AuthentificationApi/AuthentificationApi.csproj ./AuthentificationApi/
COPY GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./AuthentificationApi

# Copie du reste du code source puis publication
COPY AuthentificationApi/ ./AuthentificationApi/
COPY GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./AuthentificationApi -c Release -o out --no-restore

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Variables d'environnement (surchargées par docker-compose)
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5157

# Port exposé
EXPOSE 5157

ENTRYPOINT ["dotnet", "AuthentificationApi.dll"]
