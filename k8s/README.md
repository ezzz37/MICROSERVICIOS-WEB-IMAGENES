k8s/  
├── base/                              # Recursos “puros”, sin valores de entorno  
│   ├── namespace.yaml                 # Declara el Namespace donde se desplegará todo  
│   ├── sqlserver/                     # Carpeta con manifiestos para SQL Server en-cluster (solo dev/demo)  
│   │   ├── statefulset.yaml           # StatefulSet que define la instancia de SQL Server  
│   │   ├── service.yaml               # Service ClusterIP para exponer SQL Server internamente  
│   │   └── pvc-template.yaml          # Plantilla de PVC para almacenamiento persistente de datos  
│   ├── auth-service/                  # Manifiestos del microservicio AuthService  
│   │   ├── deployment.yaml            # Deployment (réplicas, contenedor, probes, recursos)  
│   │   ├── service.yaml               # Service ClusterIP que expone el puerto 8080  
│   │   └── secret.yaml                # Secret Opaque (SA_PASSWORD o connString)  
│   ├── imagen-service/                # Manifiestos del microservicio ImagenService  
│   │   ├── deployment.yaml            # Deployment de ImagenService  
│   │   ├── service.yaml               # Service ClusterIP para ImagenService  
│   │   └── secret.yaml                # Secret con credenciales (BD, tokens, etc.)  
│   ├── gateway/                       # Manifiestos del API Gateway (Ocelot)  
│   │   ├── deployment.yaml            # Deployment del Gateway (.NET, probes, recursos)  
│   │   ├── service.yaml               # Service interno del Gateway  
│   │   └── configmap.yaml             # ConfigMap con ocelot.json y appsettings.json  
│   ├── frontend/                      # Manifiestos del frontend React servido por Nginx  
│   │   ├── deployment.yaml            # Deployment del contenedor Nginx + build de React  
│   │   ├── service.yaml               # Service para exponer el frontend (puerto 80)  
│   │   └── configmap.yaml             # ConfigMap con variables de entorno para React  
│   └── ingress/                       # Manifiestos de Ingress NGINX y TLS  
│       └── ingress.yaml               # Reglas de ruta (/api, /) y configuración TLS  
│  
└── overlays/                          # Parches específicos de cada entorno  
    ├── dev/                           # Overlays para entorno de desarrollo  
    │   ├── kustomization.yaml         # Aplica base/, namespace=myapp-dev, replicas=1, imageTag=dev-…  
    │   └── patch-resources.yaml       # Ajusta requests/limits bajos para dev  
    │  
    └── prod/                          # Overlays para entorno de producción  
        ├── kustomization.yaml         # Aplica base/, namespace=myapp-prod, versions semánticas  
        ├── hpa.yaml                   # HorizontalPodAutoscalers para cada Deployment  
        ├── networkpolicy.yaml         # NetworkPolicies para aislar comunicación intra-cluster  
        └── external-db-secret.yaml    # Secret con credenciales de BD gestionada (Azure SQL/RDS)  
