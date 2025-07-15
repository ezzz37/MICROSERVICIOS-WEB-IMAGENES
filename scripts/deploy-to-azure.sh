#!/bin/bash

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}Iniciando despliegue completo en Azure...${NC}"
echo -e "${YELLOW}Este proceso incluye:${NC}"
echo -e "${YELLOW}  1. Creación de Azure Container Registry${NC}"
echo -e "${YELLOW}  2. Subida de imágenes Docker${NC}"
echo -e "${YELLOW}  3. Creación de cluster AKS${NC}"
echo -e "${YELLOW}  4. Creación de Azure SQL Database${NC}"
echo -e "${YELLOW}  5. Despliegue de aplicación${NC}"
echo ""

echo -e "${YELLOW}Verificando autenticación en Azure...${NC}"
if ! az account show > /dev/null 2>&1; then
    echo -e "${RED}No estás autenticado en Azure.${NC}"
    echo -e "${YELLOW}Ejecuta: az login${NC}"
    exit 1
fi

echo -e "${YELLOW}Verificando Docker...${NC}"
if ! docker ps > /dev/null 2>&1; then
    echo -e "${RED}Docker no está corriendo.${NC}"
    echo -e "${YELLOW}Por favor, inicia Docker Desktop.${NC}"
    exit 1
fi

echo -e "${GREEN}Verificaciones completadas${NC}"
echo ""

echo -e "${BLUE}==== PASO 1: Container Registry e Imágenes ====${NC}"
./azure-deployment/scripts/simple-deploy.sh

echo -e "${BLUE}==== PASO 2: Cluster AKS ====${NC}"
./azure-deployment/scripts/deploy-aks-simple.sh

echo -e "${BLUE}==== PASO 3: Azure SQL Database ====${NC}"
./azure-deployment/scripts/deploy-azure-sql.sh

echo -e "${BLUE}==== PASO 4: Aplicación ====${NC}"
./azure-deployment/scripts/deploy-app.sh

echo ""
echo -e "${GREEN}¡Despliegue completado exitosamente!${NC}"
echo -e "${BLUE}Para verificar el estado:${NC}"
echo -e "${YELLOW}  kubectl get pods --namespace microservicios-web-imagenes${NC}"
echo -e "${YELLOW}  kubectl get services --namespace microservicios-web-imagenes${NC}"
echo ""
echo -e "${BLUE}URLs de acceso se mostrarán en la salida anterior.${NC}"

