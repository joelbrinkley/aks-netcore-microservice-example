# notificationapp


# ACR Login
az acr login --name acr12934

docker login acr12934.azurecr.io

docker tag <image> acr12934.azurecr.io/<image>

docker push acr12934.azurecr.io/<image>


## k8s secret
kubectl create secret docker-registry <SECRET_NAME> 
  --docker-server <REGISTRY_NAME>.azurecr.io 
  --docker-email <YOUR_MAIL> 
  --docker-username=<SERVICE_PRINCIPAL_ID> 
  --docker-password <YOUR_PASSWORD>

  