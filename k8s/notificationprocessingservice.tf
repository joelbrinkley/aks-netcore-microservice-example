locals {
  notify_processing_svc_name = "notification-processing-svc"
}

resource "kubernetes_deployment" "notification_processing_service" {
  metadata {
    name = "${local.notify_processing_svc_name}-deployment"

    labels {
      name       = "${local.notify_processing_svc_name}"
      version    = "v1"
      component  = "service"
      part-of    = "notifyapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 2

    selector {
      match_labels {
        name    = "${local.notify_processing_svc_name}"
        version = "v1"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.notify_processing_svc_name}"
          version = "v1"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-notificationprocessingservice:v1"
          name  = "notifyapp-notification-processing-service"

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