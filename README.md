# MICROSERVICIOS-WEB-IMAGENES

## Descripción

Este proyecto implementa una arquitectura de microservicios para la gestión y procesamiento de imágenes, con autenticación y gestión de usuarios. Utiliza .NET (C#) para los microservicios principales y un frontend en ReactJS. Todo el stack es dockerizable y puede desplegarse en Kubernetes.

### Arquitectura General

- **API Gateway**: Centraliza el acceso y enruta solicitudes a los microservicios.
- **AuthService**: Servicio de autenticación (JWT, validación de usuario, cifrado AES-256).
- **UserService**: Gestión de usuarios y perfiles.
- **ImagenService**: Almacén y procesamiento de imágenes (compresión, comparaciones, procesamiento en background).
- **Frontend React**: Interfaz de usuario para login, dashboard y gestión de imágenes.

```
┌───────────────────────────────────────────────────────┐
│                      Frontend React                  │
│ ┌───────────┐   ┌───────────────┐    ┌────────────┐  │
│ │ LoginUI   │   │ DashboardUI   │    │ ImageUI    │  │
│ └───────────┘   └───────────────┘    └────────────┘  │
└────────────▲─────────────────▲────────────────▲──────┘
             │ (1) BearerToken │ (2) StatsAPI   │ (3) UploadDownload
             │                 │                │
┌────────────┴─────────────────┴────────────────┴──────┐
│                    API Gateway                       │
│  (Valida JWT, enruta apiauth→AuthService,            │
│   apiimagen→ImagenService, apiusuarios→UserService)  │
└────────────┬─────────────────┬──────────────────────┘
             │                 │
      ┌──────┴───────┐  ┌──────┴────────┐
      │ AuthService  │  │ ImagenService │
      │ (UsuariosDB) │  │ (ImagenesDB)  │
      └──────────────┘  └───────────────┘
```

## Estructura del Proyecto

```
MICROSERVICIOS-WEB-IMAGENES/
├── docker-compose.yml
├── k8s/
│   ├── gateway-deployment.yaml
│   ├── authservice-deployment.yaml
│   ├── userservice-deployment.yaml
│   ├── imagenservice-deployment.yaml
│   ├── reactfrontend-deployment.yaml
│   └── namespaces.yaml
├── src/
│   ├── Gateway/
│   ├── AuthService/
│   ├── UserService/
│   ├── ImagenService/
│   └── ReactFrontend/
└── infra.txt (documentación técnica y diagrama de componentes)
```

## Tecnologías principales

- **Backend**: .NET 8, C#, ASP.NET Core, Entity Framework Core
- **Frontend**: ReactJS (Create React App), JavaScript/TypeScript
- **Base de datos**: SQL Server (TSQL)
- **Contenedores**: Docker, docker-compose
- **Orquestador**: Kubernetes (manifiestos de despliegue)
- **Seguridad**: JWT, cifrado AES-256

## Instalación y Ejecución Local

### Prerrequisitos

- Docker y docker-compose
- Node.js (para frontend en desarrollo)
- .NET 8 SDK (para desarrollo backend)

### Levantar todo con Docker Compose

```sh
docker-compose up --build
```

Esto inicia todos los servicios, incluyendo las bases de datos y el frontend.

### Frontend React (desarrollo)

```sh
cd src/react-frontend
npm install
npm start
```
Accede a [http://localhost:3000](http://localhost:3000)

### Backend (desarrollo)

Cada microservicio tiene su propio `Dockerfile` y puede ejecutarse individualmente con:

```sh
cd src/AuthService
dotnet run
```
_Repite para UserService, ImagenService y Gateway._

## Principales Endpoints

- **/apiauth/**: autenticación, login (AuthService)
- **/apiusuarios/**: gestión de usuarios y perfiles (UserService)
- **/apiimagen/**: subida, consulta y comparación de imágenes (ImagenService)
- **/api/gateway/**: gateway de entrada para todo el flujo

## Seguridad

- Autenticación JWT en gateway y servicios.
- Encriptación de contraseñas y datos sensibles con AES-256.
- Validación de usuario a través de procedimientos almacenados en la base de datos.

## Base de Datos

- **UsuariosDB**: usuarios, roles, claves criptográficas.
- **ImagenesDB**: imágenes originales, procesadas, algoritmos de compresión, comparaciones.

## Pruebas

- El frontend incluye pruebas con React Testing Library y jest (`npm test`).
- Los microservicios incluyen endpoints de health-check y pueden extenderse con pruebas unitarias/integración.

## Despliegue en Kubernetes

Hay manifiestos YAML en el directorio `k8s/` para desplegar cada servicio y la aplicación completa en un clúster Kubernetes.

## Contribución

1. Haz un fork del repositorio
2. Crea una rama `feature/mi-feature`
3. Abre un Pull Request describiendo tus cambios

## Buenas Prácticas

- Usa ramas por feature o fix.
- Añade pruebas y documentación a tus cambios.
- Mantén el código limpio y sigue los linters/formatters (eslint para JS, dotnet format para C#).

## Licencia

Este proyecto se entrega sin licencia explícita.

---

**Autor:** [ezzz37](https://github.com/ezzz37)
