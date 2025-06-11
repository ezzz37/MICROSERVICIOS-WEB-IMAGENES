# --------- 1) Base (runtime) ---------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Exponer solo HTTP en el puerto 8080\ nEXPOSE 8080

# Forzar a ASP.NET Core a escuchar únicamente en HTTP
ENV ASPNETCORE_URLS=http://+:8080

# --------- 2) Build (SDK) ------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el .csproj y restaurar dependencias
COPY ["AuthService.csproj", "./"]
RUN dotnet restore "AuthService.csproj"

# Copiar el resto del código y compilar
COPY . .
RUN dotnet build "AuthService.csproj" -c Release -o /app/build

# --------- 3) Publish ----------------
FROM build AS publish
RUN dotnet publish "AuthService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# --------- 4) Final ------------------
FROM base AS final
WORKDIR /app

# Copiar los artefactos ya publicados
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "AuthService.dll"]
