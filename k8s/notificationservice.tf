locals {
  notify_svc_name = "notification-svc"
}

resource "kubernetes_deployment" "notification_service" {
  metadata {
    name = "${local.notify_svc_name}-deployment"

    labels {
      name       = "${local.notify_svc_name}"
      version    = "v1"
      component  = "service"
      part-of    = "notifyapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels {
        name    = "${local.notify_svc_name}"
        version = "v1"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.notify_svc_name}"
          version = "v1"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-notificationservice:v1"
          name  = "notifyapp-notification-service"

          env {
            name  = "ASPNETCORE_ENVIRONMENT"
            value = "AKS"
          }

          env {
            name  = "AzureAD__ClientId"
            value = "${data.terraform_remote_state.infra.notify_app_client_id}"
          }

          env {
            name  = "AzureAD__ClientSecret"
            value = "${data.terraform_remote_state.infra.notify_app_client_secret}"
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "notification_service" {
  metadata {
    name = "${local.notify_svc_name}-service"
  }

  spec {
    selector {
      name    = "${local.notify_svc_name}"
      version = "v1"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "ClusterIP"
  }
}
