locals {
  frontend_name = "frontend"
  frontend_version = "v2"
}

resource "kubernetes_deployment" "frontend" {
  metadata {
    name = "${local.frontend_name}-deployment"

    labels {
      name       = "${local.frontend_name}"
      version    = "${local.frontend_version}"
      component  = "frontend"
      part-of    = "notifyapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels {
        name    = "${local.frontend_name}"
        version = "${local.frontend_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.frontend_name}"
          version = "${local.frontend_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-frontend:${local.frontend_version}"
          name  = "notifyapp-frontend-service"

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

          /*           readiness_probe {
            http_get {
              path = "/health"
              port = 8080
            }

            initial_delay_seconds = 120
            timeout_seconds       = 10
            period_seconds        = 10
            failure_threshold     = 3
          } */

          env {
            name  = "ASPNETCORE_ENVIRONMENT"
            value = "AKS"
          }
          env {
            name  = "ServiceEndpoints__ContactsService"
            value = "http://contact-svc-service:8080"
          }
          env {
            name  = "ServiceEndpoints__NotificationService"
            value = "http://notification-svc-service:8080"
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "frontend" {
  metadata {
    name = "${local.frontend_name}-service"
  }

  spec {
    selector {
      name    = "${local.frontend_name}"
      version = "${local.frontend_version}"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "LoadBalancer"
  }
}
