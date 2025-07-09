# Guía de Despliegue a Azure 🚀

Esta guía te ayudará a desplegar tu aplicación de microservicios a Microsoft Azure usando Azure Container Registry y Azure Container Instances.

## Prerrequisitos 📝

1. **Azure CLI**: Ya lo tienes instalado (versión 2.75.0) ✓
2. **Docker**: Debe estar instalado y ejecutándose
3. **Cuenta de Azure**: Con una suscripción activa
4. **Permisos**: Tu cuenta debe tener permisos para crear recursos en Azure

## Arquitectura de la Aplicación 🏢

Tu aplicación consta de los siguientes servicios:

- **AuthService**: Servicio de autenticación (.NET 8.0)
- **ImagenService**: Servicio de procesamiento de imágenes (.NET 8.0)
- **Gateway**: API Gateway con Ocelot (.NET 8.0)
- **Frontend**: Aplicación React
- **Base de Datos**: Azure SQL Database (reemplaza SQL Server local)

## Recursos que se crearán en Azure 🏗️

1. **Grupo de Recursos**: `microservicios-rg`
2. **Azure Container Registry**: Para almacenar las imágenes Docker
3. **Azure SQL Database**: Base de datos gestionada
4. **Azure Container Instances**: Para ejecutar los contenedores
5. **IP Pública**: Para acceder a la aplicación

## Instrucciones de Despliegue 🚀

### Paso 1: Autenticación en Azure

```bash
# Iniciar sesión en Azure
az login

# Verificar que estás autenticado
az account show
```

### Paso 2: Ejecutar el Script de Despliegue

```bash
# Hacer el script ejecutable
chmod +x deploy-to-azure.sh

# Ejecutar el despliegue
./deploy-to-azure.sh
```

### Paso 3: Monitorear el Progreso

El script mostrará el progreso paso a paso:

1. ⚙️ **Verificación de autenticación**
2. 📦 **Creación del grupo de recursos**
3. 🏗️ **Configuración del Container Registry**
4. 🗄️ **Creación de la base de datos SQL**
5. 🔨 **Construcción y subida de imágenes**
6. 🚀 **Despliegue de contenedores**

## Tiempo Estimado ⏱️

- **Creación de recursos**: 5-10 minutos
- **Construcción de imágenes**: 10-15 minutos
- **Despliegue de contenedores**: 5-10 minutos
- **Total**: 20-35 minutos

## Costos Estimados 💰

### Recursos básicos (por mes):
- **Azure Container Registry (Basic)**: ~$5 USD
- **Azure SQL Database (Basic)**: ~$15 USD
- **Azure Container Instances**: ~$30-50 USD
- **Total aproximado**: $50-70 USD/mes

> ⚠️ **Importante**: Los costos pueden variar según el uso y la región.

## Acceso a la Aplicación 🌍

Después del despliegue, tendrás acceso a:

```
Frontend: http://[IP_PUBLICA]:80
Gateway: http://[IP_PUBLICA]:8080
AuthService: http://[IP_PUBLICA]:8080 (interno)
ImagenService: http://[IP_PUBLICA]:8081 (interno)
```

## Verificación del Despliegue 🔍

### Verificar el estado de los contenedores:
```bash
az container show --resource-group microservicios-rg --name microservicios-containers
```

### Ver los logs de un contenedor:
```bash
az container logs --resource-group microservicios-rg --name microservicios-containers --container-name authservice
```

### Verificar la base de datos:
```bash
az sql db show --resource-group microservicios-rg --server [SQL_SERVER_NAME] --name MicroserviciosDB
```

## Solución de Problemas 🔧

### Error: "No estás autenticado"
```bash
az login
```

### Error: "Docker no está ejecutándose"
- Asegúrate de que Docker Desktop esté iniciado
- Verifica con: `docker ps`

### Error: "Nombre de ACR ya existe"
- El script genera nombres únicos con timestamp
- Si persiste, edita la variable `ACR_NAME` en el script

### Error: "Faltan permisos"
- Verifica que tu cuenta tenga permisos de `Contributor` o `Owner`
- Contacta a tu administrador de Azure

## Actualizaciones 🔄

### Para actualizar una imagen:
```bash
# Reconstruir y subir la imagen
docker build -t [ACR_LOGIN_SERVER]/[SERVICE_NAME]:latest ./src/[SERVICE_NAME]
docker push [ACR_LOGIN_SERVER]/[SERVICE_NAME]:latest

# Reiniciar el contenedor
az container restart --resource-group microservicios-rg --name microservicios-containers
```

## Limpieza de Recursos 🧹

### Para eliminar todos los recursos:
```bash
# Hacer el script ejecutable
chmod +x cleanup-azure.sh

# Ejecutar la limpieza
./cleanup-azure.sh
```

> ⚠️ **Advertencia**: Esto eliminará TODOS los recursos y datos. No se puede deshacer.

## Mejores Prácticas 💫

1. **Monitoreo**: Configura alertas para monitorear el uso de recursos
2. **Respaldos**: Configura respaldos automáticos para la base de datos
3. **Seguridad**: Revisa las reglas de firewall y acceso
4. **Costos**: Monitorea los costos regularmente
5. **Actualizaciones**: Mantén las imágenes actualizadas

## Escalabilidad 📈

### Para mayor escalabilidad, considera:
- **Azure Kubernetes Service (AKS)** para orquestación avanzada
- **Azure Application Gateway** para balanceo de carga
- **Azure Redis Cache** para caché distribuido
- **Azure Storage** para archivos estáticos

## Soporte 📞

Si encuentras problemas:
1. Revisa los logs de los contenedores
2. Verifica la configuración de red
3. Consulta la documentación oficial de Azure
4. Contacta al soporte técnico si es necesario

---

¡Tu aplicación de microservicios estará lista para producción en Azure! 🎉

