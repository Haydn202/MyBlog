terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "tfstaterubberduckdiaries"
    container_name       = "tfstate"
  }
}
