#!/bin/bash

clear
echo "==============================================="
echo "🚀 MONITOREO DE KUBERNETES - MICROSERVICIOS"
echo "==============================================="
echo ""
echo "Selecciona una opción:"
echo ""
echo "1. 📊 Monitor en tiempo real (Terminal)"
echo "2. 🌐 Dashboard web de Kubernetes"
echo "3. 🔍 k9s (Terminal UI)"
echo "4. 🌐 Azure Portal"
echo "5. 📈 Estado actual del cluster"
echo "6. 🔄 Reiniciar pods"
echo "7. 📝 Ver logs de un pod"
echo "8. ❌ Salir"
echo ""
read -p "Opción (1-8): " choice

case $choice in
    1)
        echo "🚀 Iniciando monitor en tiempo real..."
        ./monitor-cluster.sh
        ;;
    2)
        echo "🌐 Iniciando Dashboard web..."
        ./open-dashboard-fixed.sh
        ;;
    3)
        echo "🔍 Iniciando k9s..."
        if [ -f "k9s.exe" ]; then
            ./k9s.exe
        else
            echo "❌ k9s no encontrado. Instalando..."
            curl -L https://github.com/derailed/k9s/releases/download/v0.32.4/k9s_Windows_amd64.zip -o k9s.zip
            unzip k9s.zip
            chmod +x k9s.exe
            ./k9s.exe
        fi
        ;;
    4)
        echo "🌐 Abriendo Azure Portal..."
        start "https://portal.azure.com/#@/resource/subscriptions/d8b528f6-ef19-4011-9b19-135571f3fa37/resourcegroups/microservicios-rg/providers/Microsoft.ContainerService/managedClusters/microservicios-aks"
        ;;
    5)
        echo "📈 Estado actual del cluster:"
        echo ""
        echo "🔧 Nodos:"
        kubectl get nodes
        echo ""
        echo "📦 Pods:"
        kubectl get pods --namespace microservicios-web-imagenes
        echo ""
        echo "🌐 Servicios:"
        kubectl get services --namespace microservicios-web-imagenes
        echo ""
        echo "🔗 URLs de acceso:"
        echo "• Frontend: http://$(kubectl get svc frontend-service -n microservicios-web-imagenes -o jsonpath='{.status.loadBalancer.ingress[0].ip}')"
        echo "• Gateway: http://$(kubectl get svc gateway-service -n microservicios-web-imagenes -o jsonpath='{.status.loadBalancer.ingress[0].ip}'):8080"
        echo ""
        read -p "Presiona Enter para continuar..."
        ;;
    6)
        echo "🔄 Pods disponibles para reiniciar:"
        kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk '{print NR". "$1}'
        echo ""
        read -p "Selecciona el número del pod a reiniciar: " pod_num
        pod_name=$(kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk "NR==$pod_num {print \$1}")
        if [ ! -z "$pod_name" ]; then
            kubectl delete pod $pod_name --namespace microservicios-web-imagenes
            echo "✅ Pod $pod_name reiniciado"
        else
            echo "❌ Selección inválida"
        fi
        read -p "Presiona Enter para continuar..."
        ;;
    7)
        echo "📝 Pods disponibles:"
        kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk '{print NR". "$1}'
        echo ""
        read -p "Selecciona el número del pod: " pod_num
        pod_name=$(kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk "NR==$pod_num {print \$1}")
        if [ ! -z "$pod_name" ]; then
            echo "📝 Logs de $pod_name:"
            kubectl logs $pod_name --namespace microservicios-web-imagenes --tail=50
        else
            echo "❌ Selección inválida"
        fi
        read -p "Presiona Enter para continuar..."
        ;;
    8)
        echo "👋 ¡Hasta luego!"
        exit 0
        ;;
    *)
        echo "❌ Opción inválida"
        read -p "Presiona Enter para continuar..."
        ;;
esac

# Volver al menú
$0
