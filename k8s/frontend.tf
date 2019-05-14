locals {
  frontend_name = "frontend"
}

resource "kubernetes_deployment" "frontend_service" {
  metadata {
    name = "${local.frontend_name}-deployment"

    labels {
      name       = "${local.frontend_name}"
      version    = "v1"
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
        version = "v1"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.frontend_name}"
          version = "v1"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-frontend:v1"
          name  = "notifyapp-frontend-service"

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

resource "kubernetes_service" "frontend_service" {
  metadata {
    name = "${local.frontend_name}-service"
  }

  spec {
    selector {
      name    = "${local.frontend_name}"
      version = "v1"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "LoadBalancer"
  }
}
