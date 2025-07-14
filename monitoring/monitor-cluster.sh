#!/bin/bash

clear
echo "=== MONITOR DE KUBERNETES - MICROSERVICIOS AZURE ==="
echo ""

while true; do
    clear
    echo "=== MONITOR DE KUBERNETES - MICROSERVICIOS AZURE ==="
    echo "$(date)"
    echo ""
    
    echo "ESTADO DEL CLUSTER:"
    echo "----------------------------------------"
    kubectl get nodes -o wide
    echo ""
    
    echo "PODS EN MICROSERVICIOS:"
    echo "----------------------------------------"
    kubectl get pods --namespace microservicios-web-imagenes -o wide
    echo ""
    
    echo "SERVICIOS Y IPs EXTERNAS:"
    echo "----------------------------------------"
    kubectl get services --namespace microservicios-web-imagenes -o wide
    echo ""
    
    echo "USO DE RECURSOS:"
    echo "----------------------------------------"
    kubectl top nodes 2>/dev/null || echo "Metrics server no disponible"
    echo ""
    
    echo "PODS CON MAYOR USO:"
    echo "----------------------------------------"
    kubectl top pods --namespace microservicios-web-imagenes 2>/dev/null || echo "Metrics server no disponible"
    echo ""
    
    echo "EVENTOS RECIENTES:"
    echo "----------------------------------------"
    kubectl get events --namespace microservicios-web-imagenes --sort-by='.lastTimestamp' | tail -5
    echo ""
    
    echo "DASHBOARD URLs:"
    echo "• Frontend: http://$(kubectl get svc frontend-service -n microservicios-web-imagenes -o jsonpath='{.status.loadBalancer.ingress[0].ip}')"
    echo "• Gateway: http://$(kubectl get svc gateway-service -n microservicios-web-imagenes -o jsonpath='{.status.loadBalancer.ingress[0].ip}'):8080"
    echo "• Azure Portal: https://portal.azure.com"
    echo ""
    
    echo "Press Ctrl+C to exit | Actualizando en 10 segundos..."
    sleep 10
done
