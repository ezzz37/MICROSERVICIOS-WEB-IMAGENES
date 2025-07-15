#!/bin/bash

set -e

ACR_NAME="miregistroacr1752465648"
ACR_LOGIN_SERVER="$ACR_NAME.azurecr.io"

kubectl create namespace microservicios-web-imagenes --dry-run=client -o yaml | kubectl apply -f -

cat > k8s-deployment.yaml << EOF
apiVersion: apps/v1
kind: Deployment
metadata:
  name: authservice
  namespace: microservicios-web-imagenes
  labels:
    app: authservice
spec:
  replicas: 2
  selector:
    matchLabels:
      app: authservice
  template:
    metadata:
      labels:
        app: authservice
    spec:
      containers:
      - name: authservice
        image: $ACR_LOGIN_SERVER/authservice:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:8080"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: authservice-service
  namespace: microservicios-web-imagenes
spec:
  selector:
    app: authservice
  ports:
  - protocol: TCP
    port: 8080
    targetPort: 8080
  type: ClusterIP
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: imagenservice
  namespace: microservicios-web-imagenes
  labels:
    app: imagenservice
spec:
  replicas: 2
  selector:
    matchLabels:
      app: imagenservice
  template:
    metadata:
      labels:
        app: imagenservice
    spec:
      containers:
      - name: imagenservice
        image: $ACR_LOGIN_SERVER/imagenservice:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:8080"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: imagenservice-service
  namespace: microservicios-web-imagenes
spec:
  selector:
    app: imagenservice
  ports:
  - protocol: TCP
    port: 8080
    targetPort: 8080
  type: ClusterIP
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gateway
  namespace: microservicios-web-imagenes
  labels:
    app: gateway
spec:
  replicas: 2
  selector:
    matchLabels:
      app: gateway
  template:
    metadata:
      labels:
        app: gateway
    spec:
      containers:
      - name: gateway
        image: $ACR_LOGIN_SERVER/gateway:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:8080"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: gateway-service
  namespace: microservicios-web-imagenes
spec:
  selector:
    app: gateway
  ports:
  - protocol: TCP
    port: 8080
    targetPort: 8080
  type: LoadBalancer
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: frontend
  namespace: microservicios-web-imagenes
  labels:
    app: frontend
spec:
  replicas: 2
  selector:
    matchLabels:
      app: frontend
  template:
    metadata:
      labels:
        app: frontend
    spec:
      containers:
      - name: frontend
        image: $ACR_LOGIN_SERVER/frontend:latest
        ports:
        - containerPort: 80
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "256Mi"
            cpu: "200m"
---
apiVersion: v1
kind: Service
metadata:
  name: frontend-service
  namespace: microservicios-web-imagenes
spec:
  selector:
    app: frontend
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
EOF

kubectl apply -f k8s-deployment.yaml

kubectl get pods --namespace microservicios-web-imagenes
kubectl get services --namespace microservicios-web-imagenes

echo "Aplicación desplegada!"
echo "Para monitorear en tiempo real:"
echo "kubectl get pods --namespace microservicios-web-imagenes --watch"
echo "Para obtener las IPs externas (puede tomar unos minutos):"
echo "kubectl get services --namespace microservicios-web-imagenes"
echo "Para ver logs:"
echo "kubectl logs -l app=gateway --namespace microservicios-web-imagenes"

echo "¡Tu aplicación está desplegada en Azure AKS!"

