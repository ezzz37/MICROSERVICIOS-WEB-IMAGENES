#!/usr/bin/env bash
set -euo pipefail

SERVICE="frontend"

# Nos aseguramos de estar en la carpeta del compose
cd "$(dirname "$0")"

echo "Reiniciando servicio \`$SERVICE\` sin levantar dependencias..."

# Detener y remover solo el contenedor
docker compose stop "$SERVICE"
docker compose rm -f "$SERVICE"

# Levantar solo ese servicio, sin dependencias
docker compose up -d --no-deps "$SERVICE"

echo "Servicio \`$SERVICE\` reiniciado correctamente."
