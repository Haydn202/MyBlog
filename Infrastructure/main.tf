# main.tf

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

# =============================================================================
# Resource Group
# =============================================================================
resource "azurerm_resource_group" "rg" {
  name     = "RG-RubberDuckDiaries"
  location = "australiaeast"
}

# =============================================================================
# Container Registry
# =============================================================================
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

# =============================================================================
# Key Vault
# =============================================================================
module "keyvault" {
  source              = "./modules/keyvault"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = "845c67a0-eb3f-4822-be54-4f78446fc867"
  sp_object_id        = "26a5e7bc-b888-4aa1-b248-a1750a80f0b4"
  keyvault_name       = "rubberduckdiaries-kv"
}

# =============================================================================
# Key Vault Access for GitHub Actions Service Principal
# =============================================================================
resource "azurerm_key_vault_access_policy" "github_actions" {
  key_vault_id = module.keyvault.keyvault_id
  tenant_id    = "845c67a0-eb3f-4822-be54-4f78446fc867"
  object_id    = "14ce4211-fcc7-4e91-9fa3-b4cc20c46eda" # GitHub Actions SP

  secret_permissions = [
    "Get",
    "List"
  ]
}

# =============================================================================
# Generate Token Key for JWT
# =============================================================================
resource "random_password" "token_key" {
  length  = 64
  special = false
}

# =============================================================================
# Application Secrets in Key Vault
# =============================================================================
resource "azurerm_key_vault_secret" "token_key" {
  name         = "TokenKey"
  value        = random_password.token_key.result
  key_vault_id = module.keyvault.keyvault_id
}

resource "azurerm_key_vault_secret" "admin_username" {
  name         = "AdminUsername"
  value        = var.admin_username
  key_vault_id = module.keyvault.keyvault_id
}

resource "azurerm_key_vault_secret" "admin_email" {
  name         = "AdminEmail"
  value        = var.admin_email
  key_vault_id = module.keyvault.keyvault_id
}

resource "azurerm_key_vault_secret" "admin_password" {
  name         = "AdminPassword"
  value        = var.admin_password
  key_vault_id = module.keyvault.keyvault_id
}

# =============================================================================
# SQL Database
# =============================================================================
module "sql_db" {
  source              = "./modules/sql"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sql_server_name     = "rubberduckdiaries-sqlserver"
  sql_db_name         = "RubberDuckDiariesDB"
  sql_admin           = "adminuser"
  keyvault_id         = module.keyvault.keyvault_id
}

# =============================================================================
# API Container Instance
# =============================================================================
module "api_container" {
  source              = "./modules/containerinstance"
  container_name      = "rubberduckdiaries-api"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  dns_name_label      = "rubberduckdiaries-api"
  registry_server     = module.acr.login_server
  registry_username   = module.acr.admin_username
  registry_password   = module.acr.admin_password
  image_name          = "rubberduckdiaries-api"
  image_tag           = "latest"
  cpu                 = 1
  memory              = 1.5
  container_port      = 8080

  environment_variables = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ASPNETCORE_URLS"        = "http://+:8080"
  }

  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}

# =============================================================================
# UI Container Instance
# =============================================================================
module "ui_container" {
  source              = "./modules/containerinstance"
  container_name      = "rubberduckdiaries-ui"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  dns_name_label      = "rubberduckdiaries"
  registry_server     = module.acr.login_server
  registry_username   = module.acr.admin_username
  registry_password   = module.acr.admin_password
  image_name          = "rubberduckdiaries-ui"
  image_tag           = "latest"
  cpu                 = 0.5
  memory              = 0.5
  container_port      = 80

  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}

# =============================================================================
# URL Secrets in Key Vault (depend on containers)
# =============================================================================
resource "azurerm_key_vault_secret" "api_url" {
  name         = "ApiUrl"
  value        = module.api_container.url
  key_vault_id = module.keyvault.keyvault_id
}

resource "azurerm_key_vault_secret" "client_url" {
  name         = "ClientUrl"
  value        = module.ui_container.url
  key_vault_id = module.keyvault.keyvault_id
}

# =============================================================================
# Variables
# =============================================================================
variable "admin_username" {
  description = "Admin username for the application"
  type        = string
  default     = "admin"
}

variable "admin_email" {
  description = "Admin email for the application"
  type        = string
  default     = "admin@rubberduckdiaries.com"
}

variable "admin_password" {
  description = "Admin password for the application"
  type        = string
  sensitive   = true
}

# =============================================================================
# Outputs
# =============================================================================
output "api_url" {
  description = "URL of the API Container"
  value       = module.api_container.url
}

output "ui_url" {
  description = "URL of the UI Container"
  value       = module.ui_container.url
}

output "api_fqdn" {
  description = "FQDN of the API Container"
  value       = module.api_container.fqdn
}

output "ui_fqdn" {
  description = "FQDN of the UI Container"
  value       = module.ui_container.fqdn
}

output "acr_login_server" {
  description = "ACR login server for pushing images"
  value       = module.acr.login_server
}

output "sql_server_name" {
  description = "SQL Server name"
  value       = module.sql_db.sql_server_name
}

output "keyvault_name" {
  description = "Key Vault name for GitHub Actions"
  value       = module.keyvault.keyvault_name
}
