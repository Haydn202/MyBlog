# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "RG-RubberDuckDiaries"
  location = "australiaeast"
}

module "my_acr" {
  source              = "./modules/acr"
  name                = "rubberduckdiaries-acr"
  resource_group_name = "RG-RubberDuckDiaries"
  location            = "Australia East"
  sku                 = "Basic"
  admin_enabled       = true
  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}
