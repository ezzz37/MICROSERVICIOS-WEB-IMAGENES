#!/bin/bash

echo "=== GRAFANA - VISUALIZACIÓN GRÁFICA DE KUBERNETES ==="
echo ""

echo "Verificando instalación de Grafana..."
if ! kubectl get pods -n monitoring | grep grafana | grep Running > /dev/null; then
    echo "Esperando a que Grafana esté listo..."
    kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=grafana -n monitoring --timeout=300s
fi

echo "Obteniendo credenciales de Grafana..."
GRAFANA_PASSWORD=$(kubectl --namespace monitoring get secrets prometheus-grafana -o jsonpath="{.data.admin-password}" | base64 -d)

echo ""
echo "Grafana está listo!"
echo ""
echo "CREDENCIALES DE ACCESO:"
echo "=========================="
echo "Usuario: admin"
echo "Contraseña: $GRAFANA_PASSWORD"
echo "=========================="
echo ""

echo "Iniciando port-forward a Grafana..."
kubectl --namespace monitoring port-forward svc/prometheus-grafana 3000:80 &
PORT_FORWARD_PID=$!

echo "Esperando a que el port-forward esté listo..."
sleep 5

echo "Abriendo Grafana en el navegador..."
start "http://localhost:3000" 2>/dev/null || echo "Por favor, abre manualmente: http://localhost:3000"

echo ""
echo "DASHBOARDS DISPONIBLES:"
echo "• Kubernetes / Compute Resources / Cluster"
echo "• Kubernetes / Compute Resources / Namespace (Pods)"
echo "• Kubernetes / Compute Resources / Node (Pods)"
echo "• Kubernetes / Compute Resources / Pod"
echo "• Kubernetes / Networking / Cluster"
echo "• Kubernetes / System / API Server"
echo ""

echo "INSTRUCCIONES:"
echo "1. Abre http://localhost:3000 en tu navegador"
echo "2. Inicia sesión con: admin / $GRAFANA_PASSWORD"
echo "3. Ve a Dashboards > Browse"
echo "4. Busca los dashboards que empiecen con 'Kubernetes'"
echo "5. Para ver TUS pods, selecciona namespace 'microservicios-web-imagenes'"
echo ""

echo "Para detener Grafana, presiona Ctrl+C"
echo ""

# Mantener el port-forward corriendo
wait $PORT_FORWARD_PID
