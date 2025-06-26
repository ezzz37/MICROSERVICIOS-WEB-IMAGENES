# MICROSERVICIOS-WEB-IMAGENES

## Descripción General

Este proyecto es un sistema web basado en microservicios para la gestión y procesamiento de imágenes, enfocado en autenticación de usuarios, administración de perfiles y almacenamiento/operación sobre imágenes. La arquitectura desacoplada permite escalar cada componente según la demanda y facilita la mantenibilidad y el despliegue tanto en Docker como en Kubernetes.

## Tabla de Contenidos

- [Arquitectura y Componentes](#arquitectura-y-componentes)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Instalación y Ejecución Local](#instalación-y-ejecución-local)
- [Despliegue en Kubernetes](#despliegue-en-kubernetes)
- [Servicios y Endpoints](#servicios-y-endpoints)
- [Base de Datos y Seguridad](#base-de-datos-y-seguridad)
- [Pruebas y Calidad](#pruebas-y-calidad)
- [Contribución](#contribución)
- [Licencia](#licencia)
- [Autor](#autor)

---

## Arquitectura y Componentes

El sistema sigue una arquitectura de microservicios, donde cada servicio se encarga de una responsabilidad concreta:

- **API Gateway**: Centraliza el acceso, enruta solicitudes y valida JWT. Utiliza Ocelot como gateway de API.
- **AuthService**: Servicio de autenticación y generación de tokens JWT, cifrado AES-256 y validación de credenciales.
- **UserService**: Gestión de usuarios y perfiles, asignación de roles.
- **ImagenService**: Maneja la subida, almacenamiento, procesamiento y comparación de imágenes.
- **React Frontend**: Interfaz de usuario moderna, con páginas para login, dashboard y operaciones sobre imágenes.

### Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────┐
│                      Frontend React                    │
│ ┌───────────┐   ┌───────────────┐   ┌──────────────┐   │
│ │ LoginUI   │   │ DashboardUI   │   │ ImageUI      │   │
│ └───────────┘   └───────────────┘   └──────────────┘   │
└────────────▲─────────────────▲────────────────▲────────┘
             │ (JWT)           │ (API Stats)    │ (Upload/Download)
             │                 │                │
┌────────────┴─────────────────┴────────────────┴────────┐
│                    API Gateway (Ocelot)                │
│  - Valida JWT                                          │
│  - Rutea tráfico a microservicios                      │
└────────────┬─────────────────┬─────────────────────────┘
             │                 │
      ┌──────┴───────┐  ┌──────┴────────┐
      │ AuthService  │  │ ImagenService │
      │ (UsuariosDB) │  │ (ImagenesDB)  │
      └──────────────┘  └───────────────┘
             │
      ┌──────┴───────┐
      │ UserService  │
      └──────────────┘
```

---

## Estructura del Proyecto

```
MICROSERVICIOS-WEB-IMAGENES/
│
├── docker-compose.yml
├── k8s/
│   ├── gateway-deployment.yaml
│   ├── authservice-deployment.yaml
│   ├── userservice-deployment.yaml
│   ├── imagenservice-deployment.yaml
│   ├── reactfrontend-deployment.yaml
│   └── namespaces.yaml
│
├── src/
│   ├── Gateway/
│   │   ├── Program.cs, Startup.cs, ocelot.json, Dockerfile, Controllers/
│   ├── AuthService/
│   │   ├── Program.cs, Startup.cs, Dockerfile, Controllers/, Services/, Helpers/
│   ├── UserService/
│   │   ├── Program.cs, Startup.cs, Dockerfile, Controllers/, Services/
│   ├── ImagenService/
│   │   ├── Program.cs, Startup.cs, Dockerfile, Controllers/, Services/, Worker/
│   └── ReactFrontend/
│       ├── src/, public/, Dockerfile, package.json
│
├── infra.txt (documentación técnica y guía de despliegue)
└── README.md
```

---

## Tecnologías Utilizadas

- **Backend**: .NET 8 (C#), ASP.NET Core, Entity Framework Core, TSQL.
- **Frontend**: ReactJS, JavaScript, CSS.
- **Contenedores**: Docker, docker-compose (para desarrollo y pruebas locales).
- **Orquestación**: Kubernetes (manifiestos YAML).
- **Base de Datos**: SQL Server.
- **Seguridad**: JWT para autenticación, AES-256 para cifrado de datos sensibles.
- **CI/CD**: Listo para pipelines de integración y despliegue continuo.
- **Testing**: React Testing Library y Jest en frontend, endpoints de health-check y unit tests en backend.

---

## Instalación y Ejecución Local

### Prerrequisitos

- [Docker](https://docs.docker.com/get-docker/)
- [docker-compose](https://docs.docker.com/compose/)
- [Node.js / npm](https://nodejs.org/) (para desarrollo frontend opcional)
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (para desarrollo backend opcional)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (opcional para desarrollo externo)

### Despliegue con Docker Compose

```sh
# Clona el repositorio
git clone https://github.com/ezzz37/MICROSERVICIOS-WEB-IMAGENES.git
cd MICROSERVICIOS-WEB-IMAGENES

# Levanta todos los servicios y bases de datos
docker-compose up --build
```

Esto inicia todos los microservicios, el frontend y las bases de datos necesarias.

#### Variables de entorno

Cada microservicio puede requerir variables de entorno (por ejemplo, cadenas de conexión a la base de datos, claves secretas para JWT). Revisa cada `Dockerfile` y los archivos `appsettings.json` para los parámetros configurables o sobreescribe usando `docker-compose.override.yml`.

### Ejecución de Servicios Individuales

Puedes levantar cada microservicio de manera independiente para desarrollo:

```sh
# Ejemplo para AuthService
cd src/AuthService
dotnet run
```

### Frontend en modo desarrollo

```sh
cd src/react-frontend
npm install
npm start
```
Accede a [http://localhost:3000](http://localhost:3000)

---

## Despliegue en Kubernetes

Incluye manifiestos de despliegue en el directorio `k8s/` para cada servicio y para el frontend. Puedes aplicar todos con:

```sh
kubectl apply -f k8s/
```

Asegúrate de tener configurado un clúster de Kubernetes y los secretos/configmaps necesarios.

---

## Servicios y Endpoints

### API Gateway

- **Responsabilidad**: Orquestar, enrutar y validar JWT para las rutas `/apiauth`, `/apiusuarios`, `/apiimagen`.
- **Archivo configuración**: `src/Gateway/ocelot.json`

### AuthService

- `POST /apiauth/login`: Login de usuario, retorna JWT.
- **Implementa**: cifrado AES-256, generación de JWT, almacenamiento seguro de contraseñas.

### UserService

- `GET/POST /apiusuarios/`: CRUD de usuarios
- `GET/POST /apiusuarios/perfiles`: CRUD de perfiles

### ImagenService

- `POST /apiimagen/upload`: Subida de imágenes
- `GET /apiimagen/list`: Consulta imágenes del usuario
- `POST /apiimagen/compare`: Comparación de imágenes
- **Procesamiento en background**: Worker para procesado asíncrono de imágenes

---

## Base de Datos y Seguridad

### UsuariosDB

- Tablas: `Usuarios`
- Objetos criptográficos: Master Key, Certificado, Symmetric Key
- Procedimientos almacenados: `sp_ValidarUsuario`

### ImagenesDB

- Tablas: `AlgoritmosCompresion`, `Imagenes`, `ImagenesProcesadas`, `Comparaciones`

### Seguridad

- **JWT**: Todos los endpoints protegidos requieren Bearer Token.
- **Cifrado AES-256**: para datos sensibles en la base de datos.
- **Roles/Perfiles**: Control granular de permisos.

---

## Pruebas y Calidad

### Frontend

- Ejecuta pruebas con:
  ```sh
  npm test
  ```
- Las pruebas usan React Testing Library y jest.

### Backend

- Endpoints de health-check disponibles.
- Recomendado: Agregar pruebas unitarias y de integración con xUnit/NUnit.

---

## Contribución

1. Haz un fork del repositorio.
2. Crea una rama: `git checkout -b feature/mi-feature`
3. Realiza tus cambios y haz commit.
4. Abre un Pull Request describiendo claramente tu aporte.
5. Sigue las buenas prácticas de estilo y documentación.

## Autor

- **ezzz37**  
  [GitHub](https://github.com/ezzz37)
