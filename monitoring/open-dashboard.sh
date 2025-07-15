#!/bin/bash

echo "=== DASHBOARD DE KUBERNETES - MICROSERVICIOS EN AZURE ==="
echo ""
echo "🔑 Token de acceso (copia este token):"
echo ""
kubectl -n kubernetes-dashboard create token admin-user
echo ""
echo "📊 Abriendo dashboard en el navegador..."
echo ""
echo "🌐 URL del dashboard: http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/"
echo ""
echo "⚠️  INSTRUCCIONES:"
echo "1. Selecciona 'Token' como método de autenticación"
echo "2. Pega el token mostrado arriba"
echo "3. Haz clic en 'Sign In'"
echo ""
echo "🚀 Iniciando proxy..."
kubectl proxy
