locals {
  communications_api_name = "communications-api"
  communications_api_version  = "v1"
}

resource "kubernetes_deployment" "communications_api" {
  metadata {
    name = "${local.communications_api_name}-deployment"

    labels {
      name       = "${local.communications_api_name}"
      version    = "${local.communications_api_version}"
      component  = "api"
      part-of    = "communications-app"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels {
        name    = "${local.communications_api_name}"
        version = "${local.communications_api_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.communications_api_name}"
          version = "${local.communications_api_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image_pull_policy = "Always"
          image             = "${data.terraform_remote_state.infra.acr_server}/communications-app_communications-api:${local.communications_api_version}"
          name              = "communications-app_communications-api"

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

resource "kubernetes_service" "communications_service" {
  metadata {
    name = "${local.communications_api_name}-service"
  }

  spec {
    selector {
      name    = "${local.communications_api_name}"
      version = "${local.communications_api_version}"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "ClusterIP"
  }
}
