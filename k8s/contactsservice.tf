locals {
  contact_svc_name = "contact-svc"
  contact_version = "v2"
}

resource "kubernetes_deployment" "contact_service" {
  metadata {
    name = "${local.contact_svc_name}-deployment"

    labels {
      name       = "${local.contact_svc_name}"
      version    = "${local.contact_version}"
      component  = "service"
      part-of    = "notifyapp"
      managed-by = "terraform"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels {
        name    = "${local.contact_svc_name}"
        version = "${local.contact_version}"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.contact_svc_name}"
          version = "${local.contact_version}"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-contactsservice:${local.contact_version}"
          name  = "notifyapp-contact-service"

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

resource "kubernetes_service" "contact_service" {
  metadata {
    name = "${local.contact_svc_name}-service"
  }

  spec {
    selector {
      name    = "${local.contact_svc_name}"
      version = "${local.contact_version}"
    }

    port {
      name        = "http"
      port        = 8080
      target_port = 80
    }

    type = "ClusterIP"
  }
}
