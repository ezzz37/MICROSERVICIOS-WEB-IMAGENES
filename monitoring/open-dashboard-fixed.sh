#!/bin/bash

echo "=== KUBERNETES DASHBOARD - MICROSERVICIOS AZURE ==="
echo ""

# Verificar que kubectl esté funcionando
if ! kubectl cluster-info &> /dev/null; then
    echo "❌ Error: kubectl no está configurado o no hay conexión al cluster"
    echo "Ejecuta: az aks get-credentials --resource-group microservicios-rg --name microservicios-aks"
    exit 1
fi

# Verificar que el dashboard esté instalado
if ! kubectl get deployment kubernetes-dashboard -n kubernetes-dashboard &> /dev/null; then
    echo "❌ Dashboard no instalado. Instalando..."
    kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
    echo "⏳ Esperando a que el dashboard esté listo..."
    kubectl wait --for=condition=ready pod -l k8s-app=kubernetes-dashboard -n kubernetes-dashboard --timeout=300s
fi

# Verificar que el usuario admin existe
if ! kubectl get serviceaccount admin-user -n kubernetes-dashboard &> /dev/null; then
    echo "❌ Usuario admin no existe. Creando..."
    kubectl apply -f dashboard-admin-user.yaml
fi

# Obtener el token
echo "🔑 Obteniendo token de acceso..."
TOKEN=$(kubectl -n kubernetes-dashboard create token admin-user)

if [ -z "$TOKEN" ]; then
    echo "❌ Error al obtener el token"
    exit 1
fi

echo ""
echo "✅ Dashboard listo!"
echo ""
echo "🔑 TOKEN DE ACCESO (copia todo el texto siguiente):"
echo "=================================================="
echo "$TOKEN"
echo "=================================================="
echo ""

# Iniciar el proxy en segundo plano
echo "Iniciando proxy de Kubernetes..."
kubectl proxy --port=8001 &
PROXY_PID=$!

# Esperar un poco para que el proxy inicie
sleep 3

# Abrir el navegador
DASHBOARD_URL="http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/"
echo "🌐 Abriendo dashboard en: $DASHBOARD_URL"

# Intentar abrir el navegador (funciona en Windows)
start "$DASHBOARD_URL" 2>/dev/null || echo "Por favor, abre manualmente: $DASHBOARD_URL"

echo ""
echo "📝 INSTRUCCIONES:"
echo "1. Se abrirá el navegador automáticamente"
echo "2. Selecciona 'Token' como método de autenticación"
echo "3. Pega el token mostrado arriba"
echo "4. Haz clic en 'Sign In'"
echo ""
echo "⚠️  Para detener el proxy, presiona Ctrl+C"
echo ""

# Mantener el proxy corriendo
wait $PROXY_PID
