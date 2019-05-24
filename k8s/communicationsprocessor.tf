locals {
  communications_processor_name = "communications-processor"
  communications_processor_version  = "v1"
}

resource "kubernetes_deployment" "communications_processing_service" {
  metadata {
    name = "${local.communications_processor_name}-deployment"

    labels {
      name       = "${local.communications_processor_name}"
      version    = "${local.communications_processor_version}"
      component  = "service"
      part-of    = "communicationsapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 3

    selector {
      match_labels {
        name    = "${local.communications_processor_name}"
        version = "${local.communications_processor_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.communications_processor_name}"
          version = "${local.communications_processor_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image_pull_policy = "Always"
          image             = "${data.terraform_remote_state.infra.acr_server}/communicationsapp-CommunicationsProcessor:${local.communications_processor_version}"
          name              = "communicationsapp-communications-processing-service"

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
