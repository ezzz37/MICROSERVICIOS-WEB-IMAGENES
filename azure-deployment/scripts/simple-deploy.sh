#!/bin/bash

set -e

RESOURCE_GROUP="microservicios-rg"
ACR_NAME="miregistroacr$(date +%s)"
LOCATION="westus2"

az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --location $LOCATION

az acr update --name $ACR_NAME --admin-enabled true

az acr login --name $ACR_NAME

ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query loginServer -o tsv)

docker tag microservicios-web-imagenes-authservice:latest $ACR_LOGIN_SERVER/authservice:latest
docker push $ACR_LOGIN_SERVER/authservice:latest

docker tag microservicios-web-imagenes-imagenservice:latest $ACR_LOGIN_SERVER/imagenservice:latest
docker push $ACR_LOGIN_SERVER/imagenservice:latest

docker tag microservicios-web-imagenes-gateway:latest $ACR_LOGIN_SERVER/gateway:latest
docker push $ACR_LOGIN_SERVER/gateway:latest

docker tag microservicios-web-imagenes-frontend:latest $ACR_LOGIN_SERVER/frontend:latest
docker push $ACR_LOGIN_SERVER/frontend:latest

