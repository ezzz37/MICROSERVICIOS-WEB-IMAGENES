kubernetes/
├── app-configs/                   # Directorio para ConfigMaps y Secrets compartidos o globales
│   ├── app-common-configmap.yaml  # Configuración común para microservicios (ej. URLs de otros servicios)
│   ├── db-credentials-secret.yaml # Credenciales para la(s) base(s) de datos (SQL Server)
│   └── jwt-secret.yaml            # Secret para la clave JWT (usado por AuthService)
│
├── auth-service/
│   ├── auth-service-deployment.yaml
│   ├── auth-service-service.yaml
│   # Nota: Los secretos específicos de este servicio irán en app-configs/ o se montarán desde allí
│
├── gateway/
│   ├── gateway-deployment.yaml
│   ├── gateway-service.yaml
│   # Nota: Configuración de Ocelot (ocelot.json) se inyectaría via ConfigMap si es dinámica
│
├── imagen-service/
│   ├── imagen-service-deployment.yaml
│   ├── imagen-service-service.yaml
│   # Nota: Los secretos específicos de este servicio irán en app-configs/ o se montarán desde allí
│
├── react-frontend/
│   ├── react-frontend-deployment.yaml
│   ├── react-frontend-service.yaml
│   ├── react-frontend-configmap.yaml # Para variables de entorno del frontend (URL del Gateway, etc.)
│
├── ingress/
│   └── main-ingress.yaml          # Define cómo el tráfico externo llega a Gateway y ReactFrontend
│
├── sqlserver-db/                  # **CAMBIO CLAVE: Directorio específico para recursos de SQL Server**
│   # Si usas SQL Server auto-administrado en K8s (NO recomendado para prod)
│   ├── sqlserver-statefulset.yaml # Define la instancia de SQL Server
│   ├── sqlserver-service.yaml     # Servicio para acceder a SQL Server internamente
│   ├── sqlserver-pvc.yaml         # PersistentVolumeClaim para los datos de SQL Server
│
├── monitoring/                    # (Opcional, pero muy recomendado) Para Grafana, Prometheus, etc.
│   ├── prometheus-deployment.yaml
│   ├── grafana-deployment.yaml
│   └── ...
│
├── rbac/                          # (Opcional) Roles y RoleBindings si necesitas control de acceso granular
│   └── ...
│
├── kustomization.yaml             # Opcional, pero muy útil para gestionar overlays para diferentes entornos
└── README.md