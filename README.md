# Overview
The purpose of this project is to create an example project to explore ideas using .Net Core, Microservices, and AKS.

Please note that this cloud infrastructure will incur Azure costs.

## Deploy Cloud Infrastructure
This project uses Terraform to deploy cloud infrastructure.

### Create a service principal for Terraform
To setup a service principal for Terraform follow these instructions:

https://www.terraform.io/docs/providers/azurerm/auth/service_principal_client_secret.html

### Create Terraform Params file
Create a terraform.tfvars file in the cloud-infrastructure directory.

Provide variable values in the file

```
// your apps name
prefix = "

// terraform service principal id
client_id = ""

// terraform service principal secret
client_secret = ""

// a dns prefix for the aks cluster
dns_prefix = ""

// password for your app service registration
service_principal_pw = ""

//password for azure sql server
sql_password = ""

// your azure tenant id
tenant_id = ""

// the desired number of virtual machines for your AKS cluster
agent_count = 2

// A list of allowed IPs for Azure Sql Server, put your IP here
sql_allowed_ips = ["154.64.112.433"]
```

### Execute Terraform

```
cd cloud-infrastructure

terraform init

terraform apply
```

type 'yes' when prompted to deploy

## User Secrets
Add AzureAD:ClientId and AzureAD:ClientSecret as user secrets
- Contacts.Api
- Contacts.NotificationPublisher
- Communications.Api
- Communications.Backend

For more information on Safe Storage of app secrets
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows

## Deploy the Database Migrations
This step is dependent on ensuring that your IP is allowed in Azure Sql Server and that you have added the appropriate user secrets

### Contacts
Navigate to Contacts.Api and use the cli to update the database

```
cd Contacts/Contacts.Api

dotnet ef database update
```

### Communications
Navigate to Communications.Backend and use the cli to update the database

```
cd Communications/Communications.Backend

dotnet ef database update
```


## Build Docker Images and push to Azure Container Registry

### Login

Login to Azure Container Registry using the Azure CLI

```
az acr login -n MyRegistry
```

Use the Powershell Scripts build-tag-all.ps1 to build and tag images in Azure Container Registry

```
// subsitute acr78890 with your azure container registry name
./build-tag-all.ps1 -version v1 -acr acr78890
```



  
