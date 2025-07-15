# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copia sólo el .csproj y restaura paquetes
COPY ["Gateway.csproj", "./"]
RUN dotnet restore "Gateway.csproj"

# 2) Copia el resto del código y publica directo en Release
COPY . .
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 3) Copiamos los artefactos publicados
COPY --from=build /app/publish .

# 4) Copiamos los JSON de configuración
COPY ocelot.Production.json ocelot.json
COPY appsettings.json .

# 5) Configuramos Kestrel
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 6) Arrancamos la DLL
ENTRYPOINT ["dotnet", "Gateway.dll"]
