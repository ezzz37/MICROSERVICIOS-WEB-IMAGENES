# 🚀 Microservicios Web Imágenes - Azure AKS

Aplicación de microservicios desplegada en Azure Kubernetes Service (AKS) para procesamiento de imágenes.

## 📁 Estructura del Proyecto

```
.
├── 🎯 microservicios-admin.sh     # Script principal de administración
├── 📊 monitoring/                 # Herramientas de monitoreo
│   ├── monitor-cluster.sh         # Monitor en tiempo real
│   ├── open-dashboard-fixed.sh    # Dashboard web K8s
│   ├── open-grafana.sh           # Grafana con métricas
│   └── kubernetes-monitor-menu.sh # Menú de monitoreo
├── 🛠️ tools/                     # Herramientas externas
│   ├── k9s.exe                   # Terminal UI para K8s
│   └── helm.exe                  # Gestor de paquetes K8s
├── 📜 scripts/                   # Scripts de despliegue
│   ├── deploy-to-azure.sh        # Desplegar a Azure
│   ├── cleanup-azure.sh          # Limpiar recursos
│   └── start-app.sh              # Iniciar localmente
├── ⚙️ config/                    # Configuraciones
│   ├── k8s-deployment.yaml      # Manifiestos K8s
│   ├── azure-sql-secret.yaml    # Secrets de BD
│   └── dashboard-admin-user.yaml # Usuario dashboard
├── 🗂️ temp/                     # Archivos temporales
├── 📂 src/                      # Código fuente
│   ├── AuthService/             # Microservicio autenticación
│   ├── ImagenService/           # Microservicio imágenes
│   ├── Gateway/                 # API Gateway
│   └── react-frontend/          # Frontend React
├── 🔧 k8s/                      # Manifiestos Kubernetes
└── ☁️ azure-deployment/         # Scripts Azure
```

## 🎯 Uso Rápido

### Script Principal
```bash
# Administrador completo del proyecto
./microservicios-admin.sh
```

### Monitoreo Rápido
```bash
# Monitor en tiempo real
./monitoring/monitor-cluster.sh

# Dashboard web de Kubernetes
./monitoring/open-dashboard-fixed.sh

# Grafana con métricas avanzadas
./monitoring/open-grafana.sh

# Terminal UI interactivo
./tools/k9s.exe
```

### Administración
```bash
# Desplegar a Azure
./scripts/deploy-to-azure.sh

# Limpiar recursos
./scripts/cleanup-azure.sh

# Estado del cluster
kubectl get pods --namespace microservicios-web-imagenes
```

## 📊 Herramientas de Monitoreo

### 1. Monitor en Tiempo Real
- **Script**: `./monitoring/monitor-cluster.sh`
- **Descripción**: Vista actualizada cada 10 segundos
- **Funciones**: Estado pods, servicios, recursos, eventos

### 2. Dashboard Web Kubernetes
- **Script**: `./monitoring/open-dashboard-fixed.sh`
- **URL**: `http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/`
- **Credenciales**: Se generan automáticamente

### 3. Grafana
- **Script**: `./monitoring/open-grafana.sh`
- **URL**: `http://localhost:3000`
- **Usuario**: `admin`
- **Contraseña**: `prom-operator`

### 4. Terminal UI (k9s)
- **Ejecutable**: `./tools/k9s.exe`
- **Descripción**: Interfaz interactiva de terminal
- **Comandos útiles**:
  - `:pods` - Ver pods
  - `:svc` - Ver servicios
  - `:ns` - Cambiar namespace

## 🎮 Administrador Principal

El script `microservicios-admin.sh` proporciona un menú interactivo con todas las funciones:

```bash
./microservicios-admin.sh
```

### Funciones Disponibles:

#### 📊 Monitoreo
1. Monitor en tiempo real
2. Dashboard web (Kubernetes)
3. Grafana (Gráficos avanzados)
4. Terminal UI (k9s)
5. Azure Portal

#### 🔧 Administración
6. Estado del cluster
7. Logs de pods
8. Reiniciar pods
9. Escalar servicios

#### ☁️ Despliegue
10. Desplegar a Azure
11. Limpiar recursos
12. Actualizar imágenes

#### ⚙️ Configuración
13. Ver configuración
14. Actualizar secrets
15. Verificar conexiones

## 🌐 URLs de Acceso

### Aplicación
- **Frontend**: `http://172.179.63.132`
- **Gateway**: `http://4.149.142.85:8080`

### Monitoreo
- **Kubernetes Dashboard**: `http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/`
- **Grafana**: `http://localhost:3000`
- **Azure Portal**: [Enlace directo al cluster](https://portal.azure.com/#@/resource/subscriptions/d8b528f6-ef19-4011-9b19-135571f3fa37/resourcegroups/microservicios-rg/providers/Microsoft.ContainerService/managedClusters/microservicios-aks)

## 📦 Servicios Desplegados

### Microservicios
- **AuthService**: Autenticación (2 réplicas)
- **ImagenService**: Procesamiento de imágenes (2 réplicas)
- **Gateway**: API Gateway (2 réplicas)
- **Frontend**: React frontend (2 réplicas)

### Infraestructura
- **Azure AKS**: Cluster Kubernetes
- **Azure SQL**: Base de datos
- **Azure Container Registry**: Registro de imágenes
- **Prometheus + Grafana**: Monitoreo

## 🔧 Comandos Útiles

### Kubectl
```bash
# Ver pods
kubectl get pods --namespace microservicios-web-imagenes

# Ver servicios
kubectl get services --namespace microservicios-web-imagenes

# Ver logs
kubectl logs [pod-name] --namespace microservicios-web-imagenes

# Escalar servicio
kubectl scale deployment [service-name] --replicas=3 --namespace microservicios-web-imagenes
```

### Azure CLI
```bash
# Ver recursos
az resource list --resource-group microservicios-rg

# Estado del cluster
az aks show --resource-group microservicios-rg --name microservicios-aks
```

## 🆘 Solución de Problemas

### Si los pods no aparecen en Grafana:
1. Verifica que el namespace sea `microservicios-web-imagenes`
2. Asegúrate de que Prometheus esté recolectando métricas
3. Revisa los dashboards de Kubernetes en Grafana

### Si k9s no funciona:
```bash
# Ejecutar desde la carpeta tools
./tools/k9s.exe
```

### Si el dashboard web no carga:
```bash
# Reiniciar el proxy
./monitoring/open-dashboard-fixed.sh
```

## 🚀 Inicio Rápido

1. **Ejecutar el administrador**:
   ```bash
   ./microservicios-admin.sh
   ```

2. **Ver estado del cluster** (opción 6)

3. **Abrir Grafana** (opción 3) para monitoreo gráfico

4. **Usar k9s** (opción 4) para administración interactiva

## 📝 Notas

- Todos los scripts están organizados por funcionalidad
- Los archivos temporales están en `temp/`
- Las herramientas externas están en `tools/`
- Las configuraciones están en `config/`
- El monitoreo está centralizado en `monitoring/`
