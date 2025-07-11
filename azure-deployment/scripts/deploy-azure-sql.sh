#!/bin/bash

set -e

RESOURCE_GROUP="microservicios-rg"
SQL_SERVER_NAME="microservicios-sql-$(date +%s)"
DATABASE_NAME="MicroserviciosDB"
LOCATION="westus2"
SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="MySecurePassword123!"

echo "Creando Azure SQL Server: ${SQL_SERVER_NAME}..."
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $SQL_ADMIN_USER \
    --admin-password $SQL_ADMIN_PASSWORD

echo "Creando base de datos: ${DATABASE_NAME}..."
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $DATABASE_NAME \
    --service-objective Basic

echo "Configurando firewall de SQL Server..."
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name AllowAzureServices \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0

SQL_CONNECTION_STRING="Server=$SQL_SERVER_NAME.database.windows.net;Database=$DATABASE_NAME;User Id=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
AUTH_CONNECTION_STRING="Server=$SQL_SERVER_NAME.database.windows.net;Database=AuthServiceBD;User Id=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
IMAGEN_CONNECTION_STRING="Server=$SQL_SERVER_NAME.database.windows.net;Database=DigitalizacionImagenesBD;User Id=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"

echo "Azure SQL Database creado exitosamente!"
echo "Detalles:"
echo "  • SQL Server: $SQL_SERVER_NAME.database.windows.net"
echo "  • Base de datos: $DATABASE_NAME"
echo "  • Usuario admin: $SQL_ADMIN_USER"
echo ""
echo "Connection Strings generados:"
echo "AUTH_CONNECTION_STRING='$AUTH_CONNECTION_STRING'"
echo "IMAGEN_CONNECTION_STRING='$IMAGEN_CONNECTION_STRING'"
echo ""
echo "Ahora actualizando los microservicios para usar Azure SQL..."

echo "Eliminando SQL Server dockerizado..."
kubectl delete deployment sqlserver --namespace microservicios-web-imagenes --ignore-not-found=true
kubectl delete service sqlserver-service --namespace microservicios-web-imagenes --ignore-not-found=true

echo "Creando secrets con connection strings..."
kubectl create secret generic azure-sql-secret \
    --from-literal=auth-connection-string="$AUTH_CONNECTION_STRING" \
    --from-literal=imagen-connection-string="$IMAGEN_CONNECTION_STRING" \
    --namespace microservicios-web-imagenes \
    --dry-run=client -o yaml | kubectl apply -f -

echo "¡Azure SQL Database configurado! Ahora ejecuta el script para actualizar los microservicios."

