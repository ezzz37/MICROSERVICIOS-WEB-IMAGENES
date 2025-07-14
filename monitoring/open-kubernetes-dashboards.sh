#!/bin/bash

echo "=== HERRAMIENTAS DE VISUALIZACIÓN DE KUBERNETES ==="
echo ""
echo "Tienes las siguientes opciones para visualizar tu cluster:"
echo ""
echo "1. 🌐 Azure Portal (Recomendado para Azure AKS)"
echo "2. 📊 Kubernetes Dashboard (Web)"
echo "3. 💻 k9s (Terminal UI)"
echo "4. 🖥️  Lens (Desktop GUI)"
echo "5. 📈 Grafana + Prometheus (Monitoreo avanzado)"
echo ""

read -p "Selecciona una opción (1-5): " choice

case $choice in
    1)
        echo "🌐 Abriendo Azure Portal..."
        echo ""
        echo "URL directa del cluster:"
        echo "https://portal.azure.com/#@/resource/subscriptions/d8b528f6-ef19-4011-9b19-135571f3fa37/resourcegroups/microservicios-rg/providers/Microsoft.ContainerService/managedClusters/microservicios-aks"
        echo ""
        echo "O navega a: Azure Portal > Resource Groups > microservicios-rg > microservicios-aks"
        start "https://portal.azure.com/#@/resource/subscriptions/d8b528f6-ef19-4011-9b19-135571f3fa37/resourcegroups/microservicios-rg/providers/Microsoft.ContainerService/managedClusters/microservicios-aks"
        ;;
    2)
        echo "📊 Iniciando Kubernetes Dashboard..."
        echo ""
        echo "🔑 Token de acceso:"
        kubectl -n kubernetes-dashboard create token admin-user
        echo ""
        echo "🌐 URL: http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/"
        echo ""
        echo "⚠️  Instrucciones:"
        echo "1. Copia el token de arriba"
        echo "2. Abre la URL en tu navegador"
        echo "3. Selecciona 'Token' e introduce el token"
        echo ""
        echo "🚀 Iniciando proxy..."
        kubectl proxy
        ;;
    3)
        echo "💻 Iniciando k9s..."
        echo ""
        echo "⚠️  Comandos útiles en k9s:"
        echo "  :pods - Ver pods"
        echo "  :svc - Ver servicios"
        echo "  :ns - Cambiar namespace"
        echo "  :q - Salir"
        echo ""
        k9s
        ;;
    4)
        echo "🖥️  Iniciando Lens..."
        echo ""
        echo "⚠️  Lens se abrirá automáticamente y detectará tu cluster"
        echo "Si no aparece, agrega el cluster manualmente con el kubeconfig"
        echo ""
        start lens
        ;;
    5)
        echo "📈 Instalando Grafana + Prometheus..."
        echo ""
        echo "Esto instalará un stack completo de monitoreo"
        read -p "¿Continuar? (y/N): " confirm
        if [[ $confirm == [yY] ]]; then
            helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
            helm repo update
            helm install prometheus prometheus-community/kube-prometheus-stack --namespace monitoring --create-namespace
            echo ""
            echo "Instalación completada. Para acceder a Grafana:"
            echo "kubectl port-forward svc/prometheus-grafana 3000:80 -n monitoring"
            echo "Usuario: admin, Contraseña: prom-operator"
        fi
        ;;
    *)
        echo "❌ Opción no válida"
        ;;
esac
