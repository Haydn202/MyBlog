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

module "acr" {
  source              = "./modules/acr"
  name                = "rubberduckdiariesacr"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true
  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}

module "keyvault" {
  source              = "./modules/keyvault"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = "845c67a0-eb3f-4822-be54-4f78446fc867"
  sp_object_id        = "26a5e7bc-b888-4aa1-b248-a1750a80f0b4"
  keyvault_name       = "rubberduckdiaries-kv"
}

module "sql_db" {
  source               = "./modules/sql"
  resource_group_name  = azurerm_resource_group.rg.name
  location             = azurerm_resource_group.rg.location
  sql_server_name      = "rubberduckdiaries-sqlserver"
  sql_db_name          = "RubberDuckDiariesDB"
  sql_admin            = "adminuser"
  keyvault_id          = module.keyvault.keyvault_id
}

# module "app_service_plan" {
#   source              = "./modules/appserviceplan"
#   resource_group_name = azurerm_resource_group.rg.name
#   location            = azurerm_resource_group.rg.location
#   plan_name           = "rubberduckdiaries-plan"
#   sku_name            = "B1"
# }

# module "ui_app_service" {
#   source              = "./modules/appservice"
#   app_name            = "rubberduckdiaries-ui"
#   app_service_plan_id = module.app_service_plan.id
#   resource_group_name = azurerm_resource_group.rg.name
#   location            = azurerm_resource_group.rg.location
#   container_image     = "rubberduckdiariesacr.azurecr.io/rubberduckdiaries-ui:latest"
#   container_registry  = module.acr.login_server
#   registry_username   = module.acr.admin_username
#   registry_password   = module.acr.admin_password
#   app_settings = {
#     "API_URL" = "https://${module.api_app_service.default_site_hostname}"
#   }
# }

# module "api_app_service" {
#   source              = "./modules/appservice"
#   app_name            = "rubberduckdiaries-api"
#   resource_group_name = azurerm_resource_group.rg.name
#   location            = azurerm_resource_group.rg.location
#   app_service_plan_id = module.app_service_plan.id
#   container_image     = "rubberduckdiariesacr.azurecr.io/rubberduckdiaries-api:latest"
#   container_registry  = module.acr.login_server
#   registry_username   = module.acr.admin_username
#   registry_password   = module.acr.admin_password

#   app_settings = {
#     "ConnectionStrings__Database" = module.sql_db.db_connection_string
#   }
# }
