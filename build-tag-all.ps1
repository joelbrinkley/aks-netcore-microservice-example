param(    
    [string]$acr = "",
    [string]$version = "v1"
)

docker build -f ./src/Frontend/dockerfile . -t "$acr.azurecr.io/communications-app_frontend:$version"
docker push "$acr.azurecr.io/communications-app_frontend:$version"

docker build -f ./src/Contacts/Contacts.Api/dockerfile . -t "$acr.azurecr.io/communications-app_contacts-api:$version"
docker push "$acr.azurecr.io/communications-app_contacts-api:$version"

docker build -f ./src/Contacts/Contacts.NotificationHandler/dockerfile . -t "$acr.azurecr.io/communications-app_contacts-notificationpublisher:$version"
docker push "$acr.azurecr.io/communications-app_contacts-notificationpublisher:$version"

docker build -f ./src/Communications/Communications.Api/dockerfile . -t "$acr.azurecr.io/communications-app_communications-api:$version"
docker push "$acr.azurecr.io/communications-app_communications-api:$version"

docker build -f ./src/Communications/Communications.Backend/dockerfile . -t "$acr.azurecr.io/communications-app_communications-backend:$version"
docker push "$acr.azurecr.io/communications-app_communications-backend:$version"
