locals {
  contact_publisher_name    = "contacts-notificationpublisher"
  contact_publisher_version = "v1"
}

resource "kubernetes_deployment" "contact_publisher" {
  metadata {
    name = "${local.contact_publisher_name}-deployment"

    labels {
      name       = "${local.contact_publisher_name}"
      version    = "${local.contact_publisher_version}"
      component  = "publisher"
      part-of    = "notifyapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels {
        name    = "${local.contact_publisher_name}"
        version = "${local.contact_publisher_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.contact_publisher_name}"
          version = "${local.contact_publisher_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image_pull_policy = "Always"
          image             = "${data.terraform_remote_state.infra.acr_server}/communications-app_contacts-notificationpublisher:${local.contact_publisher_version}"
          name              = "communications-app_contacts-notificationpublisher"

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
