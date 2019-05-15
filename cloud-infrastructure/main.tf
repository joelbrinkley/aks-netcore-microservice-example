locals {
  tags = {
    owner   = "Joel Brinkley"
    creator = "Joel Brinkley"
    source  = "Terraform"
    app     = "notificationapp"
  }
}

provider "azurerm" {
  version = "=1.27.0"
}

provider "azuread" {
  version = "0.3"
}

provider "random" {
  version = "2.1"
}

resource "azurerm_resource_group" "main" {
  name     = "${var.prefix}-resources"
  location = "${var.location}"
}

resource "azuread_application" "main" {
  name                       = "${var.prefix}"
  homepage                   = "https://homepage"
  identifier_uris            = ["https://uri"]
  reply_urls                 = ["https://replyurl"]
  available_to_other_tenants = false
  oauth2_allow_implicit_flow = false
}

resource "azuread_service_principal" "main" {
  application_id = "${azuread_application.main.application_id}"
}

resource "azuread_service_principal_password" "main" {
  service_principal_id = "${azuread_service_principal.main.id}"
  value                = "${var.service_principal_pw}"
  end_date             = "2021-01-01T01:02:03Z"
}

resource "azurerm_kubernetes_cluster" "main" {
  name                = "${var.prefix}-aks"
  location            = "${azurerm_resource_group.main.location}"
  resource_group_name = "${azurerm_resource_group.main.name}"
  dns_prefix          = "${var.dns_prefix}"

  agent_pool_profile {
    name                      = "default${count.index}"
    count                     = "${var.agent_count}"
    vm_size                   = "Standard_D1_v2"
    os_type                   = "Linux"
    os_disk_size_gb           = 30
  }

  service_principal {
    client_id     = "${var.client_id}"
    client_secret = "${var.client_secret}"
  }

  tags = "${local.tags}"
}

resource "azurerm_servicebus_namespace" "main" {
  name                = "${var.prefix}sb"
  location            = "${azurerm_resource_group.main.location}"
  resource_group_name = "${azurerm_resource_group.main.name}"
  sku                 = "Standard"

  tags = "${local.tags}"
}

resource "azurerm_servicebus_queue" "main" {
  name                = "notifications"
  resource_group_name = "${azurerm_resource_group.main.name}"
  namespace_name      = "${azurerm_servicebus_namespace.main.name}"

  enable_partitioning = false
}

resource "random_integer" "main" {
  min = 10000
  max = 99999
}

resource "azurerm_cosmosdb_account" "main" {
  name                = "${var.prefix}-cosmos-db-${random_integer.main.result}"
  location            = "${azurerm_resource_group.main.location}"
  resource_group_name = "${azurerm_resource_group.main.name}"
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  enable_automatic_failover = true

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 10
    max_staleness_prefix    = 200
  }

  geo_location {
    location          = "${var.failover_location}"
    failover_priority = 1
  }

  geo_location {
    prefix            = "${var.prefix}-cosmos-db-${random_integer.main.result}-customid"
    location          = "${azurerm_resource_group.main.location}"
    failover_priority = 0
  }
}

resource "azurerm_container_registry" "main" {
  name                = "acr${random_integer.main.result}"
  resource_group_name = "${azurerm_resource_group.main.name}"
  location            = "${azurerm_resource_group.main.location}"
  sku                 = "Basic"
  admin_enabled       = true
}

resource "azurerm_key_vault" "main" {
  name                        = "${var.prefix}-akv"
  location                    = "${azurerm_resource_group.main.location}"
  resource_group_name         = "${azurerm_resource_group.main.name}"
  enabled_for_disk_encryption = false
  tenant_id                   = "${var.tenant_id}"

  sku {
    name = "standard"
  }

  network_acls {
    default_action = "Allow"
    bypass         = "None"
  }

  access_policy {
    tenant_id = "${var.tenant_id}"
    object_id = "${azuread_service_principal.main.id}"

    key_permissions = [
      "get",
    ]

    secret_permissions = [
      "get",
      "list",
    ]

    storage_permissions = [
      "get",
    ]
  }

  access_policy {
    tenant_id = "${var.tenant_id}"
    object_id = "${var.key_vault_admin}"

    key_permissions = [
      "get",
      "list",
      "update",
      "create",
      "import",
      "delete",
      "recover",
      "backup",
      "restore",
    ]

    secret_permissions = [
      "get",
      "list",
      "delete",
      "recover",
      "backup",
      "restore",
      "set",
    ]

    storage_permissions = [
      "get",
      "set",
      "list",
      "update",
      "delete",
      "recover",
      "backup",
      "restore",
    ]
  }

  tags = "${local.tags}"
}

resource "azurerm_key_vault_secret" "main" {
  name         = "CosmosdbConnection"
  value        = "AccountEndpoint=${azurerm_cosmosdb_account.main.endpoint};AccountKey=${azurerm_cosmosdb_account.main.primary_master_key};"
  key_vault_id = "${azurerm_key_vault.main.id}"

  tags = "${local.tags}"
}

resource "azurerm_key_vault_secret" "notification_connection_string" {
  name         = "NotificationQueueConnectionString"
  value        = "${azurerm_servicebus_namespace.main.default_primary_connection_string}"
  key_vault_id = "${azurerm_key_vault.main.id}"

  tags = "${local.tags}"
}
