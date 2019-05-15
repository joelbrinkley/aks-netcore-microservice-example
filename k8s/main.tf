# use remote state to get access to infrastructure that the deplyoment is dependent on
data "terraform_remote_state" "infra" {
  backend = "local"

  config = {
    path = "../cloud-infrastructure/terraform.tfstate"
  }
}

locals {
  # define a dockercfg that will be used to create a secret for pulling images from azure container registry
  dockercfg = {
    "${data.terraform_remote_state.infra.acr_server}" = {
      email    = "joel.brinkley@insight.com"
      username = "${data.terraform_remote_state.infra.acr_username}"
      password = "${data.terraform_remote_state.infra.acr_password}"
    }
  }
}

# use data from the infrasture state file to configure the kubernetes provider
provider "kubernetes" {
  host                   = "${data.terraform_remote_state.infra.host}"
  client_certificate     = "${base64decode(data.terraform_remote_state.infra.client_certificate)}"
  client_key             = "${base64decode(data.terraform_remote_state.infra.client_key)}"
  cluster_ca_certificate = "${base64decode(data.terraform_remote_state.infra.cluster_ca_certificate)}"
  load_config_file       = false
}

# create the image pull secret
resource "kubernetes_secret" "regsecret" {
  metadata {
    name = "regsecret"
  }

  data {
    ".dockercfg" = "${ jsonencode(local.dockercfg) }"
  }

  type = "kubernetes.io/dockercfg"
}

resource "kubernetes_secret" "client_secret" {
  metadata {
    name = "clientsecrets"
  }

  data {
    AzureAD__ClientId     = "${data.terraform_remote_state.infra.notify_app_client_id}"
    AzureAD__ClientSecret = "${data.terraform_remote_state.infra.notify_app_client_secret}"
  }
}
