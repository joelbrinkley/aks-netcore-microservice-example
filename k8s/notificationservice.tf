locals {
  notify_svc_name = "notification-svc"
  notify_version  = "v2"
}

resource "kubernetes_deployment" "notification_service" {
  metadata {
    name = "${local.notify_svc_name}-deployment"

    labels {
      name       = "${local.notify_svc_name}"
      version    = "${local.notify_version}"
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
        version = "${local.notify_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.notify_svc_name}"
          version = "${local.notify_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-notificationservice:${local.notify_version}"
          name  = "notifyapp-notification-service"

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

resource "kubernetes_service" "notification_service" {
  metadata {
    name = "${local.notify_svc_name}-service"
  }

  spec {
    selector {
      name    = "${local.notify_svc_name}"
      version = "${local.notify_version}"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "ClusterIP"
  }
}
