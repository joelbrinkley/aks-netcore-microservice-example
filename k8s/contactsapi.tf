locals {
  contact_api_name = "contacts-api"
  contact_api_version  = "v1"
}

resource "kubernetes_deployment" "contacts_api" {
  metadata {
    name = "${local.contact_api_name}-deployment"

    labels {
      name       = "${local.contact_api_name}"
      version    = "${local.contact_api_version}"
      component  = "api"
      part-of    = "communications-app"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels {
        name    = "${local.contact_api_name}"
        version = "${local.contact_api_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.contact_api_name}"
          version = "${local.contact_api_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image_pull_policy = "Always"
          image = "${data.terraform_remote_state.infra.acr_server}/communications-app_contacts-api:${local.contact_api_version}"
          name  = "communications-app_contacts-api"

          liveness_probe {
            http_get {
              path = "/liveness"
              port = 80
            }

            initial_delay_seconds = 30
            timeout_seconds       = 10
            period_seconds        = 15
            failure_threshold     = 3
          }

          readiness_probe {
            http_get {
              path = "/health"
              port = 80
            }

            initial_delay_seconds = 15
            timeout_seconds       = 10
            period_seconds        = 10
            failure_threshold     = 3
          }

          env {
            name  = "ASPNETCORE_ENVIRONMENT"
            value = "AKS"
          }

          env_from {
            secret_ref {
              name = "clientsecrets"
            }
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "contacts_api" {
  metadata {
    name = "${local.contact_api_name}-service"
  }

  spec {
    selector {
      name    = "${local.contact_api_name}"
      version = "${local.contact_api_version}"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "ClusterIP"
  }
}
