# Etapa base: entorno de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Etapa build: compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY ["ImagenService.csproj", "./"]
RUN dotnet restore "ImagenService.csproj"

# Copiar todo el proyecto
COPY . .

# Compilar en modo Release
RUN dotnet build "ImagenService.csproj" -c Release -o /app/build

# Etapa publish: generar archivos para producción
FROM build AS publish
RUN dotnet publish "ImagenService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final: imagen de producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ejecutar la app
ENTRYPOINT ["dotnet", "ImagenService.dll"]
