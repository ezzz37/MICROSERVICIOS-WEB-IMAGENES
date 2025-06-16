# --- Etapa base: runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Exponemos HTTP y HTTPS
EXPOSE 8080
EXPOSE 8081

# Hacemos que Kestrel escuche en todos los interfaces en esos puertos
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081

# --- Etapa build: compilación ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos sólo el .csproj y restauramos paquetes
COPY ["Gateway.csproj", "./"]
RUN dotnet restore "Gateway.csproj"

# Copiamos el resto de la fuente y compilamos
COPY . .
RUN dotnet build "Gateway.csproj" -c Release -o /app/build

# --- Etapa publish: publicación optimizada ---
FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Etapa final: runtime reducido ---
FROM base AS final
WORKDIR /app

# Copiamos los artefactos publicados
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "Gateway.dll"]
