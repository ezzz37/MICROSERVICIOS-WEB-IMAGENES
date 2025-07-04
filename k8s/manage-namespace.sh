#!/bin/bash

NAMESPACE="microservices"
K8S_DIR="$(dirname "$0")"

case "$1" in
  "create")
    echo "Creating namespace and resources..."
    kubectl create namespace $NAMESPACE || true
    kubectl apply -f $K8S_DIR/secrets/
    kubectl apply -f $K8S_DIR/configmaps/
    echo "Resources created successfully!"
    ;;
  "delete")
    echo "Deleting namespace and all resources..."
    kubectl delete namespace $NAMESPACE
    echo "Namespace deleted!"
    ;;
  "status")
    echo "=== Namespace Status ==="
    kubectl get namespace $NAMESPACE
    echo ""
    echo "=== Secrets ==="
    kubectl get secrets -n $NAMESPACE
    echo ""
    echo "=== ConfigMaps ==="
    kubectl get configmaps -n $NAMESPACE
    ;;
  "describe")
    echo "=== Secrets Details ==="
    kubectl describe secrets -n $NAMESPACE
    echo ""
    echo "=== ConfigMaps Details ==="
    kubectl describe configmaps -n $NAMESPACE
    ;;
  *)
    echo "Usage: $0 {create|delete|status|describe}"
    echo "  create   - Create namespace and apply all resources"
    echo "  delete   - Delete the entire namespace"
    echo "  status   - Show current status of resources"
    echo "  describe - Show detailed information about resources"
    exit 1
    ;;
esac

