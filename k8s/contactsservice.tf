locals {
  contact_svc_name = "contact-svc"
}

resource "kubernetes_deployment" "contact_service" {
  metadata {
    name = "${local.contact_svc_name}-deployment"

    labels {
      name       = "${local.contact_svc_name}"
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
        name    = "${local.contact_svc_name}"
        version = "v1"
      }
    }

    template {
      metadata {
        labels {
          name    = "${local.contact_svc_name}"
          version = "v1"
        }
      }

      spec {
        image_pull_secrets = [{
          name = "${kubernetes_secret.regsecret.metadata.0.name}"
        }]

        container {
          image = "${data.terraform_remote_state.infra.acr_server}/notifyapp-contactsservice:v1"
          name  = "notifyapp-contact-service"

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

resource "kubernetes_service" "contact_service" {
  metadata {
    name = "${local.contact_svc_name}-service"
  }

  spec {
    selector {
      name    = "${local.contact_svc_name}"
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
