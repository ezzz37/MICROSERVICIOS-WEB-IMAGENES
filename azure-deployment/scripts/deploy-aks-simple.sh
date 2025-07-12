#!/bin/bash

set -e

RESOURCE_GROUP="microservicios-rg"
ACR_NAME="miregistroacr1751945372"
AKS_CLUSTER_NAME="microservicios-aks"
LOCATION="westus2"
NODE_COUNT=2

az aks create \
    --resource-group $RESOURCE_GROUP \
    --name $AKS_CLUSTER_NAME \
    --node-count $NODE_COUNT \
    --generate-ssh-keys \
    --attach-acr $ACR_NAME \
    --location $LOCATION \
    --node-vm-size Standard_B2s


az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_CLUSTER_NAME --overwrite-existing

kubectl get nodes

