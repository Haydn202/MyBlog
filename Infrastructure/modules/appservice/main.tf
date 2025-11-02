resource "azurerm_app_service_plan" "plan" {
  name                = "${var.app_name}-plan"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = "Linux"
  reserved            = true

  sku {
    tier = "Basic"
    size = "B1"
  }
}

resource "azurerm_app_service" "app" {
  name                = var.app_name
  location            = var.location
  resource_group_name = var.resource_group_name
  app_service_plan_id = azurerm_app_service_plan.plan.id

  site_config {
    linux_fx_version = "DOCKER|${var.acr_name}.azurecr.io/${var.container_image}:${var.container_tag}"
  }

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${var.acr_name}.azurecr.io"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = var.acr_name
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = data.azurerm_container_registry.acr_admin_password.password
    "DATABASE_CONNECTION_STRING"          = data.azurerm_key_vault_secret.db_connection.value
  }

  identity {
    type = "SystemAssigned"
  }
}

data "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = var.resource_group_name
}

data "azurerm_container_registry_admin_password" "acr_admin_password" {
  name                = data.azurerm_container_registry.acr.name
  resource_group_name = var.resource_group_name
}

data "azurerm_key_vault_secret" "db_connection" {
  name         = "DbConnectionString"
  key_vault_id = var.keyvault_id
}
