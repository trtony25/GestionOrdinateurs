# =========================
# Build (Blazor WebAssembly)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie des .csproj seuls puis restore
COPY GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
COPY GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.Client

# Copie du reste du code source puis publication (fichiers statiques dans out/wwwroot)
COPY GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
COPY GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.Client -c Release -o out --no-restore

# =========================
# Runtime : nginx sert les fichiers statiques sur le port 5000
# =========================
FROM nginx:alpine AS runtime
COPY GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

EXPOSE 5000
