param(    
    [string]$version = "v1"
)

docker-compose build

docker tag notificationapp_notifyapp-frontend acr65099.azurecr.io/notifyapp-frontend:$version
Write-Output "docker tag notificationapp_notifyapp-frontend acr65099.azurecr.io/notifyapp-frontend:$version"
docker tag notificationapp_notification-processing-service acr65099.azurecr.io/notifyapp-notificationprocessingservice:$version
Write-Output "docker tag notificationapp_notification-processing-service acr65099.azurecr.io/notifyapp-notificationprocessingservice:$version"
docker tag notificationapp_notification-service acr65099.azurecr.io/notifyapp-notificationservice:$version
Write-Output "docker tag notificationapp_notification-service acr65099.azurecr.io/notifyapp-notificationservice:$version"
docker tag notificationapp_contacts-publisher acr65099.azurecr.io/notifyapp-contactspublisher:$version
Write-Output "docker tag notificationapp_contacts-publisher acr65099.azurecr.io/notifyapp-contactspublisher:$version"
docker tag notificationapp_contacts-service acr65099.azurecr.io/notifyapp-contactsservice:$version
Write-Output "docker tag notificationapp_contacts-service acr65099.azurecr.io/notifyapp-contactsservice:$version"

docker push acr65099.azurecr.io/notifyapp-frontend:$version
docker push acr65099.azurecr.io/notifyapp-notificationprocessingservice:$version
docker push acr65099.azurecr.io/notifyapp-notificationservice:$version
docker push acr65099.azurecr.io/notifyapp-contactspublisher:$version
docker push acr65099.azurecr.io/notifyapp-contactsservice:$version
