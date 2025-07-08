# Despliegue en Azure 🚀

Este directorio contiene todos los archivos necesarios para desplegar la aplicación de microservicios en Azure.

## Estructura del Proyecto

```
azure-deployment/
├── scripts/               # Scripts de despliegue
│   ├── simple-deploy.sh   # 1. Crear ACR y subir imágenes
│   ├── deploy-aks-simple.sh # 2. Crear cluster AKS
│   ├── deploy-azure-sql.sh  # 3. Crear Azure SQL Database
│   └── deploy-app.sh        # 4. Desplegar aplicación
├── k8s-manifests/         # Manifiestos de Kubernetes
│   ├── gateway-config.yaml # Configuración del Gateway
│   ├── final-microservices.yaml # Microservicios principales
│   └── fix-frontend.yaml   # Configuración del Frontend
└── README-AZURE-DEPLOYMENT.md # Documentación detallada
```

## Despliegue Rápido

### Prerrequisitos
- Azure CLI instalado y autenticado (`az login`)
- Docker Desktop ejecutándose
- kubectl instalado

### Comando Único
```bash
./deploy-to-azure.sh
```

### Pasos Manuales (si prefieres control paso a paso)

1. **Container Registry e Imágenes**
   ```bash
   ./azure-deployment/scripts/simple-deploy.sh
   ```

2. **Cluster AKS**
   ```bash
   ./azure-deployment/scripts/deploy-aks-simple.sh
   ```

3. **Azure SQL Database**
   ```bash
   ./azure-deployment/scripts/deploy-azure-sql.sh
   ```

4. **Aplicación**
   ```bash
   ./azure-deployment/scripts/deploy-app.sh
   ```

## Recursos Creados

- **Grupo de Recursos**: `microservicios-rg`
- **Container Registry**: `miregistroacr[timestamp]`
- **Cluster AKS**: `microservicios-aks`
- **SQL Server**: `microservicios-sql-[timestamp]`
- **Base de Datos**: `MicroserviciosDB`

## Verificación del Despliegue

```bash
# Ver estado de los pods
kubectl get pods --namespace microservicios-web-imagenes

# Ver servicios y IPs externas
kubectl get services --namespace microservicios-web-imagenes

# Ver logs del gateway
kubectl logs -l app=gateway --namespace microservicios-web-imagenes
```

## URLs de Acceso

Después del despliegue, obtendrás:
- **Frontend**: IP externa del servicio frontend-service
- **Gateway API**: IP externa del servicio gateway-service

## Limpieza de Recursos

```bash
# Eliminar todo el grupo de recursos (CUIDADO: irreversible)
az group delete --name microservicios-rg --yes --no-wait
```

## Solución de Problemas

Ver `README-AZURE-DEPLOYMENT.md` para documentación detallada y solución de problemas.

