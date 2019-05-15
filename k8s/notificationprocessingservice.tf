locals {
  notify_processing_svc_name = "notification-processing-svc"
  notify_processing_version  = "v3"
}

resource "kubernetes_deployment" "notification_processing_service" {
  metadata {
    name = "${local.notify_processing_svc_name}-deployment"

    labels {
      name       = "${local.notify_processing_svc_name}"
      version    = "${local.notify_processing_version}"
      component  = "service"
      part-of    = "notifyapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 3

    selector {
      match_labels {
        name    = "${local.notify_processing_svc_name}"
        version = "${local.notify_processing_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.notify_processing_svc_name}"
          version = "${local.notify_processing_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-notificationprocessingservice:${local.notify_processing_version}"
          name  = "notifyapp-notification-processing-service"

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
