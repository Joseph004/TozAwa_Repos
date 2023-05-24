provider "azurerm" {
  features {}
}

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.25.0"
    }
  }
  /* backend "azurerm" {
    resource_group_name  = "tf_rg_blobstore"
    storage_account_name = "tfstoragebinarythistle"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  } */
}

/* variable "imagebuild" {
  type        = string
  description = "Latest Image Build"
} */


resource "azurerm_resource_group" "tozawa_nonprod" {
  name     = "tf_tozawa_nonprod_rg"
  location = "West Europe"
}

resource "azurerm_container_group" "container_nonprod" {
  name                = "tozawalanguagesvc"
  location            = azurerm_resource_group.tozawa_nonprod.location
  resource_group_name = azurerm_resource_group.tozawa_nonprod.name

  ip_address_type = "Public"
  dns_name_label  = "dev-tozalanguagesvc"
  os_type         = "Linux"

  container {
    name   = "dev-tozawalangaugesvc"
    image  = "josephluhandu/tozawalanguagesvc"/* :${var.imagebuild} */
    cpu    = "1"
    memory = "1"

    ports {
      port     = 80
      protocol = "TCP"
    }
  }
}
