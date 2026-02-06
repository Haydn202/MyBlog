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
  location = "centralus"
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
  tenant_id           = var.tenant_id
  sp_object_id        = var.service_principal_object_id
  keyvault_name       = "rubberduckdiaries-kv"
}

# =============================================================================
# Key Vault Access for GitHub Actions Service Principal
# =============================================================================
resource "azurerm_key_vault_access_policy" "github_actions" {
  key_vault_id = module.keyvault.keyvault_id
  tenant_id    = var.tenant_id
  object_id    = var.github_actions_object_id

  secret_permissions = [
    "Get",
    "List"
  ]
}

# =============================================================================
# Key Vault Access for Current User (Terraform)
# =============================================================================
resource "azurerm_key_vault_access_policy" "current_user" {
  key_vault_id = module.keyvault.keyvault_id
  tenant_id    = var.tenant_id
  object_id    = var.current_user_object_id

  secret_permissions = [
    "Get",
    "List",
    "Set",
    "Delete",
    "Recover",
    "Backup",
    "Restore"
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
  cpu                 = 0.5
  memory              = 0.5
  container_port      = 80

  environment_variables = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ASPNETCORE_URLS"        = "http://+:80"
  }

  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}

# =============================================================================
# UI Static Web App (Free tier - replaces container)
# =============================================================================
module "ui_static_webapp" {
  source              = "./modules/staticwebapp"
  app_name            = "rubberduckdiaries-ui"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  custom_domain       = var.custom_domain

  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}

# =============================================================================
# URL Secrets in Key Vault (using custom domain via Cloudflare)
# =============================================================================
resource "azurerm_key_vault_secret" "api_url" {
  name         = "ApiUrl"
  value        = var.custom_domain != "" ? "https://api.${var.custom_domain}" : module.api_container.url
  key_vault_id = module.keyvault.keyvault_id
}

resource "azurerm_key_vault_secret" "client_url" {
  name         = "ClientUrl"
  value        = var.custom_domain != "" ? "https://${var.custom_domain}" : module.ui_static_webapp.url
  key_vault_id = module.keyvault.keyvault_id
}

resource "azurerm_key_vault_secret" "static_webapp_api_key" {
  name         = "StaticWebAppApiKey"
  value        = module.ui_static_webapp.api_key
  key_vault_id = module.keyvault.keyvault_id
  depends_on   = [module.ui_static_webapp]
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

variable "custom_domain" {
  description = "Custom domain for Cloudflare (e.g., rubberduckdiaries.com). Leave empty to use container URLs directly."
  type        = string
  default     = ""
}

variable "tenant_id" {
  description = "Azure AD tenant ID"
  type        = string
  sensitive   = true
}

variable "service_principal_object_id" {
  description = "Object ID of the service principal for Key Vault access"
  type        = string
  sensitive   = true
}

variable "github_actions_object_id" {
  description = "Object ID of the GitHub Actions service principal"
  type        = string
  sensitive   = true
}

variable "current_user_object_id" {
  description = "Object ID of the current user running Terraform"
  type        = string
  sensitive   = true
}

# =============================================================================
# Outputs
# =============================================================================
output "ui_url" {
  description = "URL of the UI (custom domain or Static Web App)"
  value       = var.custom_domain != "" ? "https://${var.custom_domain}" : module.ui_static_webapp.url
}

output "api_url" {
  description = "URL of the API (custom domain or container)"
  value       = var.custom_domain != "" ? "https://api.${var.custom_domain}" : module.api_container.url
}

output "ui_static_webapp_hostname" {
  description = "Hostname of the Static Web App (for Cloudflare CNAME)"
  value       = module.ui_static_webapp.default_hostname
}

output "static_webapp_api_key" {
  description = "API key for Static Web App deployment (add to GitHub Secrets)"
  value       = module.ui_static_webapp.api_key
  sensitive   = true
}

output "custom_domain_validation_token" {
  description = "TXT record value for custom domain validation - add this as a TXT record in Cloudflare DNS"
  value       = var.custom_domain != "" ? module.ui_static_webapp.custom_domain_validation_token : null
  sensitive   = true
}

output "api_container_fqdn" {
  description = "FQDN of the API Container (for Cloudflare CNAME)"
  value       = module.api_container.fqdn
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
