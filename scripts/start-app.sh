#!/bin/bash

echo "Verificando estado de Minikube"
if ! minikube status > /dev/null 2>&1; then
    echo "Minikube no esta corriendo. Iniciando Minikube"
    minikube start
else
    echo "Minikube ya esta corriendo"
fi

echo "Verificando estado de los pods"
kubectl get pods -n microservicios-web-imagenes

echo "Esperando a que todos los servicios estén listos"
kubectl wait --for=condition=ready pod -l app=react-frontend -n microservicios-web-imagenes --timeout=120s
kubectl wait --for=condition=ready pod -l app=gateway -n microservicios-web-imagenes --timeout=120s
kubectl wait --for=condition=ready pod -l app=auth-service -n microservicios-web-imagenes --timeout=120s

echo "Limpiando port-forwards anteriores"
pkill -f "kubectl port-forward" 2>/dev/null || true

echo "Configurando acceso al frontend en puerto 3001"
kubectl port-forward service/react-frontend 3001:80 -n microservicios-web-imagenes &
FRONTEND_PID=$!

echo "Configurando acceso al gateway en puerto 8080"
kubectl port-forward service/gateway 8080:8080 -n microservicios-web-imagenes &
GATEWAY_PID=$!

sleep 3

echo "Verificando conectividad"
if curl -s http://localhost:3001 > /dev/null; then
    echo "Frontend disponible en: http://localhost:3001"
    echo "Login disponible en: http://localhost:3001/login"
else
    echo "Error: No se puede acceder al frontend"
fi

if curl -s http://localhost:8080 > /dev/null; then
    echo "Gateway disponible en: http://localhost:8080"
else
    echo "Gateway puede tardar un momento en estar disponible"
fi

echo ""
echo "Aplicación iniciada exitosamente"
echo "Accede a tu aplicacion en: http://localhost:3001"
echo "Accede al login en: http://localhost:3001/login"
echo "Para detener la aplicacion, ejecuta: ./stop-app.sh"
echo "Los port-forwards seguirán corriendo en segundo plano"
echo "PIDs: Frontend=$FRONTEND_PID, Gateway=$GATEWAY_PID"

echo $FRONTEND_PID > .frontend.pid
echo $GATEWAY_PID > .gateway.pid

