#!/bin/bash

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${YELLOW}Recursos que serán eliminados:${NC}"
echo -e "${YELLOW}  • Grupo de recursos: microservicios-rg${NC}"
echo -e "${YELLOW}  • Cluster AKS${NC}"
echo -e "${YELLOW}  • Container Registry${NC}"
echo -e "${YELLOW}  • Azure SQL Database${NC}"
echo -e "${YELLOW}  • Todas las imágenes y datos${NC}"

read -p "\u00bfEstás seguro de que quieres continuar? (escribe 'SI' para confirmar): " confirmacion

if [ "$confirmacion" != "SI" ]; then
    echo -e "${GREEN}Operación cancelada.${NC}"
    exit 0
fi

echo -e "${BLUE}Eliminando recursos de Azure...${NC}"


if ! az account show > /dev/null 2>&1; then
    echo -e "${RED}No estás autenticado en Azure.${NC}"
    echo -e "${YELLOW}Ejecuta: az login${NC}"
    exit 1
fi

echo -e "${YELLOW}Eliminando grupo de recursos...${NC}"
az group delete --name microservicios-rg --yes --no-wait

echo -e "${GREEN}Proceso de eliminación iniciado${NC}"
echo -e "${BLUE}Los recursos se están eliminando en segundo plano.${NC}"
echo -e "${BLUE}Puedes verificar el progreso con:${NC}"
echo -e "${YELLOW}  az group show --name microservicios-rg${NC}"
echo ""
echo -e "${GREEN}Limpieza completada${NC}"

