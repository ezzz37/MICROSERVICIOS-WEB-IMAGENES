#!/bin/bash

clear
echo "=============================================="
echo "🚀 ADMINISTRADOR DE MICROSERVICIOS - AZURE"
echo "=============================================="
echo ""
echo "Proyecto: Microservicios Web Imágenes"
echo "Entorno: Azure AKS"
echo "Namespace: microservicios-web-imagenes"
echo ""

# Función para mostrar el menú principal
show_menu() {
    echo "Selecciona una opción:"
    echo ""
    echo "📊 MONITOREO:"
    echo "  1. Monitor en tiempo real"
    echo "  2. Dashboard web (Kubernetes)"
    echo "  3. Grafana (Gráficos avanzados)"
    echo "  4. Terminal UI (k9s)"
    echo "  5. Azure Portal"
    echo ""
    echo "🔧 ADMINISTRACIÓN:"
    echo "  6. Estado del cluster"
    echo "  7. Logs de pods"
    echo "  8. Reiniciar pods"
    echo "  9. Escalar servicios"
    echo ""
    echo "☁️ DESPLIEGUE:"
    echo "  10. Desplegar a Azure"
    echo "  11. Limpiar recursos"
    echo "  12. Actualizar imágenes"
    echo ""
    echo "⚙️ CONFIGURACIÓN:"
    echo "  13. Ver configuración"
    echo "  14. Actualizar secrets"
    echo "  15. Verificar conexiones"
    echo ""
    echo "  0. Salir"
    echo ""
}

# Función para ejecutar comandos con verificación
run_command() {
    local cmd=$1
    local desc=$2
    
    echo "🔄 $desc..."
    if eval "$cmd"; then
        echo "✅ $desc completado"
    else
        echo "❌ Error en: $desc"
        read -p "Presiona Enter para continuar..."
    fi
}

# Función para mostrar estado del cluster
show_cluster_status() {
    echo "📊 Estado del Cluster:"
    echo "===================="
    echo ""
    echo "🔧 Nodos:"
    kubectl get nodes -o wide
    echo ""
    echo "📦 Pods:"
    kubectl get pods --namespace microservicios-web-imagenes -o wide
    echo ""
    echo "🌐 Servicios:"
    kubectl get services --namespace microservicios-web-imagenes -o wide
    echo ""
    echo "💾 Uso de recursos:"
    kubectl top nodes 2>/dev/null || echo "Metrics server no disponible"
    echo ""
    echo "📈 Pods - Uso de recursos:"
    kubectl top pods --namespace microservicios-web-imagenes 2>/dev/null || echo "Metrics server no disponible"
    echo ""
}

# Menú principal
while true; do
    show_menu
    read -p "Opción: " choice
    
    case $choice in
        1)
            echo "📊 Iniciando monitor en tiempo real..."
            ./monitoring/monitor-cluster.sh
            ;;
        2)
            echo "🌐 Abriendo Dashboard web..."
            ./monitoring/open-dashboard-fixed.sh
            ;;
        3)
            echo "📊 Abriendo Grafana..."
            ./monitoring/open-grafana.sh
            ;;
        4)
            echo "🔍 Iniciando k9s..."
            ./tools/k9s.exe
            ;;
        5)
            echo "🌐 Abriendo Azure Portal..."
            start "https://portal.azure.com/#@/resource/subscriptions/d8b528f6-ef19-4011-9b19-135571f3fa37/resourcegroups/microservicios-rg/providers/Microsoft.ContainerService/managedClusters/microservicios-aks"
            ;;
        6)
            clear
            show_cluster_status
            read -p "Presiona Enter para continuar..."
            ;;
        7)
            echo "📝 Selecciona un pod para ver logs:"
            kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk '{print NR". "$1}'
            echo ""
            read -p "Número del pod: " pod_num
            pod_name=$(kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk "NR==$pod_num {print \$1}")
            if [ ! -z "$pod_name" ]; then
                echo "📝 Logs de $pod_name:"
                kubectl logs $pod_name --namespace microservicios-web-imagenes --tail=100 -f
            fi
            ;;
        8)
            echo "🔄 Reiniciar pods:"
            kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk '{print NR". "$1}'
            echo ""
            read -p "Número del pod a reiniciar: " pod_num
            pod_name=$(kubectl get pods --namespace microservicios-web-imagenes --no-headers | awk "NR==$pod_num {print \$1}")
            if [ ! -z "$pod_name" ]; then
                kubectl delete pod $pod_name --namespace microservicios-web-imagenes
                echo "✅ Pod $pod_name reiniciado"
            fi
            read -p "Presiona Enter para continuar..."
            ;;
        9)
            echo "📈 Escalar servicios:"
            echo "1. authservice"
            echo "2. imagenservice"
            echo "3. gateway"
            echo "4. frontend"
            read -p "Servicio a escalar: " service_num
            read -p "Número de réplicas: " replicas
            
            case $service_num in
                1) kubectl scale deployment authservice --replicas=$replicas --namespace microservicios-web-imagenes ;;
                2) kubectl scale deployment imagenservice --replicas=$replicas --namespace microservicios-web-imagenes ;;
                3) kubectl scale deployment gateway --replicas=$replicas --namespace microservicios-web-imagenes ;;
                4) kubectl scale deployment frontend --replicas=$replicas --namespace microservicios-web-imagenes ;;
                *) echo "❌ Opción inválida" ;;
            esac
            read -p "Presiona Enter para continuar..."
            ;;
        10)
            echo "☁️ Desplegando a Azure..."
            ./scripts/deploy-to-azure.sh
            ;;
        11)
            echo "⚠️ Limpiando recursos de Azure..."
            ./scripts/cleanup-azure.sh
            ;;
        12)
            echo "🔄 Actualizando imágenes..."
            echo "Esta función actualizará las imágenes Docker en ACR"
            read -p "¿Continuar? (y/N): " confirm
            if [[ $confirm == [yY] ]]; then
                run_command "docker-compose build" "Construyendo imágenes"
                run_command "./azure-deployment/scripts/simple-deploy.sh" "Subiendo imágenes a ACR"
                run_command "kubectl rollout restart deployment --namespace microservicios-web-imagenes" "Reiniciando deployments"
            fi
            ;;
        13)
            echo "⚙️ Configuración actual:"
            echo "====================="
            kubectl get configmaps --namespace microservicios-web-imagenes
            echo ""
            kubectl get secrets --namespace microservicios-web-imagenes
            echo ""
            read -p "Presiona Enter para continuar..."
            ;;
        14)
            echo "🔐 Actualizando secrets..."
            kubectl get secrets --namespace microservicios-web-imagenes
            echo ""
            read -p "Presiona Enter para continuar..."
            ;;
        15)
            echo "🔍 Verificando conexiones..."
            run_command "kubectl cluster-info" "Conexión a cluster"
            run_command "az account show" "Autenticación Azure"
            run_command "docker ps" "Docker"
            read -p "Presiona Enter para continuar..."
            ;;
        0)
            echo "👋 ¡Hasta luego!"
            exit 0
            ;;
        *)
            echo "❌ Opción inválida"
            read -p "Presiona Enter para continuar..."
            ;;
    esac
    
    clear
done
