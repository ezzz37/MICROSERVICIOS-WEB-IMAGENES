# Kubernetes Manifests para Microservicios Web Imágenes

Este directorio contiene todos los manifiestos de Kubernetes necesarios para desplegar la aplicación de microservicios de gestión de imágenes web.

## Estructura del Directorio

```
k8s/
├── base/                     # Manifiestos base para todos los entornos
│   ├── auth-service/         # Servicio de autenticación
│   │   ├── deployment.yaml   # Despliegue del servicio
│   │   ├── service.yaml      # Service ClusterIP
│   │   ├── hpa.yaml          # Horizontal Pod Autoscaler
│   │   ├── ingress.yaml      # Ingress para acceso externo
│   │   ├── secret.yaml       # Secretos específicos del servicio
│   │   └── kustomization.yaml
│   ├── imagen-service/       # Servicio de gestión de imágenes
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── hpa.yaml
│   │   ├── ingress.yaml
│   │   ├── secret.yaml
│   │   └── kustomization.yaml
│   ├── gateway/              # API Gateway (Ocelot)
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── hpa.yaml
│   │   ├── ingress.yaml
│   │   ├── configmap.yaml
│   │   └── kustomization.yaml
│   ├── frontend/             # Frontend React
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── hpa.yaml
│   │   ├── ingress.yaml
│   │   ├── configmap.yaml
│   │   └── kustomization.yaml
│   ├── sqlserver/            # Base de datos SQL Server
│   │   ├── statefulset.yaml
│   │   ├── service.yaml
│   │   ├── pvc-template.yaml
│   │   ├── secret.yaml
│   │   └── kustomization.yaml
│   └── ingress/              # Ingress global (alternativo)
│       ├── ingress.yaml
│       └── kustomization.yaml
├── overlays/                 # Configuraciones específicas por entorno
│   ├── dev/                  # Entorno de desarrollo
│   │   ├── auth-service-nodeport.yaml
│   │   ├── imagen-service-nodeport.yaml
│   │   ├── gateway-nodeport.yaml
│   │   ├── frontend-nodeport.yaml
│   │   └── kustomization.yaml
│   └── prod/                 # Entorno de producción
│       ├── hpa.yaml          # HPA específico para producción
│       └── kustomization.yaml
├── configmaps/               # ConfigMaps globales
├── secrets/                  # Secretos globales
├── namespace.yaml            # Namespace del proyecto
├── manage-namespace.sh       # Script de gestión de namespace
└── README.md                 # Documentación base
```

## Componentes de la Aplicación

### 1. Auth Service (Servicio de Autenticación)
- **Puerto**: 8080
- **Replicas**: 2-5 (con HPA)
- **Recursos**: CPU: 100m-300m, Memoria: 128Mi-256Mi
- **Base de datos**: AuthServiceBD
- **Health checks**: `/health`

### 2. Imagen Service (Servicio de Gestión de Imágenes)
- **Puerto**: 8080
- **Replicas**: 2-8 (con HPA)
- **Recursos**: CPU: 100m-300m, Memoria: 128Mi-256Mi
- **Base de datos**: DigitalizacionImagenesBD
- **Health checks**: `/health`
- **Características**: Soporte para archivos grandes (50MB)

### 3. Gateway (API Gateway - Ocelot)
- **Puerto**: 8080
- **Replicas**: 2-6 (con HPA)
- **Configuración**: ConfigMap con ocelot.json y appsettings.json
- **Health checks**: `/health`

### 4. Frontend (React Application)
- **Puerto**: 80
- **Replicas**: 2-4 (con HPA)
- **Tipo**: Aplicación estática React

### 5. SQL Server
- **Puerto**: 1433
- **Tipo**: StatefulSet
- **Almacenamiento**: Persistent Volume Claims
- **Health checks**: SQL command

## Tipos de Services

### ClusterIP (Predeterminado)
Todos los servicios incluyen Services tipo ClusterIP para comunicación interna:
- `auth-service:8080`
- `imagen-service:8080`
- `gateway:8080`
- `frontend:80`
- `sqlserver:1433`

### NodePort (Desarrollo)
Para el entorno de desarrollo, se incluyen Services NodePort:
- `auth-service-nodeport:30080`
- `imagen-service-nodeport:30081`
- `gateway-nodeport:30082`
- `frontend-nodeport:30083`

## Horizontal Pod Autoscaler (HPA)

Cada microservicio incluye configuración HPA con:
- **Métricas**: CPU (70%) y Memoria (80%)
- **Comportamiento de escalado**:
  - Scale Up: 100% cada 15 segundos, estabilización 60s
  - Scale Down: 50% cada 60 segundos, estabilización 300s

### Límites de Replicas por Servicio:
- **Auth Service**: 2-5 replicas
- **Imagen Service**: 2-8 replicas (más demanda esperada)
- **Gateway**: 2-6 replicas
- **Frontend**: 2-4 replicas

## Ingress Configuration

### Hosts Configurados:
- `microservices.local` - Host principal
- `auth.microservices.local` - Acceso directo al auth service
- `imagen.microservices.local` - Acceso directo al imagen service
- `gateway.microservices.local` - Acceso directo al gateway
- `frontend.microservices.local` - Acceso directo al frontend

### Rutas:
- `/api/auth/*` → auth-service
- `/api/imagen/*` → imagen-service
- `/api/gateway/*` → gateway
- `/api/*` → gateway (fallback)
- `/` → frontend

### Características del Ingress:
- **Controller**: NGINX
- **SSL Redirect**: Deshabilitado para desarrollo
- **Proxy Body Size**: 50MB para imagen-service
- **Regex Support**: Habilitado

## Deployment

### Aplicar manifiestos base:
```bash
# Crear namespace
kubectl apply -f k8s/namespace.yaml

# Aplicar configuración base
kubectl apply -k k8s/base/sqlserver
kubectl apply -k k8s/base/auth-service
kubectl apply -k k8s/base/imagen-service
kubectl apply -k k8s/base/gateway
kubectl apply -k k8s/base/frontend
```

### Desarrollo (con NodePort):
```bash
kubectl apply -k k8s/overlays/dev
```

### Producción:
```bash
kubectl apply -k k8s/overlays/prod
```

## Monitoreo y Verificación

### Verificar pods:
```bash
kubectl get pods -n microservicios-web-imagenes
```

### Verificar services:
```bash
kubectl get svc -n microservicios-web-imagenes
```

### Verificar HPA:
```bash
kubectl get hpa -n microservicios-web-imagenes
```

### Verificar ingress:
```bash
kubectl get ingress -n microservicios-web-imagenes
```

### Logs de los servicios:
```bash
kubectl logs -f deployment/auth-service -n microservicios-web-imagenes
kubectl logs -f deployment/imagen-service -n microservicios-web-imagenes
kubectl logs -f deployment/gateway -n microservicios-web-imagenes
```

## Configuración de DNS Local

Para desarrollo local, agregar al archivo `/etc/hosts` (Linux/Mac) o `C:\Windows\System32\drivers\etc\hosts` (Windows):

```
127.0.0.1 microservices.local
127.0.0.1 auth.microservices.local
127.0.0.1 imagen.microservices.local
127.0.0.1 gateway.microservices.local
127.0.0.1 frontend.microservices.local
```

## Secretos y ConfigMaps

### Secretos incluidos:
- Database passwords
- API keys
- TLS certificates

### ConfigMaps incluidos:
- Gateway configuration (ocelot.json)
- Application settings
- Environment-specific configurations

## Notas Importantes

1. **Dependencias**: SQL Server debe estar listo antes de los servicios de aplicación
2. **Health Checks**: Todos los servicios incluyen readiness y liveness probes
3. **Recursos**: Límites y requests definidos para optimizar el uso de recursos
4. **Escalabilidad**: HPA configurado para responder a la carga automáticamente
5. **Seguridad**: Secretos gestionados por Kubernetes Secrets
6. **Networking**: Services ClusterIP para comunicación interna, Ingress para acceso externo

