locals {
    tags = {
        owner = "Joel Brinkley"
        creator = "Joel Brinkley"
        source = "Terraform"
        app = "notificationapp"
    }
}

resource "azurerm_resource_group" "main" {
  name     = "${var.prefix}-resources"
  location = "${var.location}"
}

resource "azurerm_kubernetes_cluster" "main" {
  name                = "${var.prefix}-aks"
  location            = "${azurerm_resource_group.main.location}"
  resource_group_name = "${azurerm_resource_group.main.name}"
  dns_prefix          = "${var.dns_prefix}"

  agent_pool_profile {
    name            = "default${count.index}"
    count           = "${var.agent_count}"
    vm_size         = "Standard_D1_v2"
    os_type         = "Linux"
    os_disk_size_gb = 30
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

resource "azurerm_servicebus_topic" "main" {
  name                = "notifications"
  resource_group_name = "${azurerm_resource_group.main.name}"
  namespace_name      = "${azurerm_servicebus_namespace.main.name}"

  enable_partitioning = true
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
  name                     = "acr${random_integer.main.result}"
  resource_group_name      = "${azurerm_resource_group.main.name}"
  location                 = "${azurerm_resource_group.main.location}"
  sku                      = "Basic"
  admin_enabled            = false
}