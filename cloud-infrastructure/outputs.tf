output "communications_app_client_id" {
  value = "${azuread_application.main.application_id}"
}

output "communications_app_client_secret" {
  sensitive = true
  value     = "${azuread_service_principal_password.main.value}"
}

output "client_key" {
  sensitive = true
  value     = "${azurerm_kubernetes_cluster.main.kube_config.0.client_key}"
}

output "client_certificate" {
  sensitive = true
  value     = "${azurerm_kubernetes_cluster.main.kube_config.0.client_certificate}"
}

output "cluster_ca_certificate" {
  sensitive = true
  value     = "${azurerm_kubernetes_cluster.main.kube_config.0.cluster_ca_certificate}"
}

output "host" {
  value = "${azurerm_kubernetes_cluster.main.kube_config.0.host}"
}

output "acr_username" {
  value = "${azurerm_container_registry.main.admin_username}"
}

output "acr_password" {
  sensitive = true
  value     = "${azurerm_container_registry.main.admin_password}"
}

output "acr_server" {
  value = "${azurerm_container_registry.main.login_server}"
}
