# modules/appservice/main.tf

resource "azurerm_linux_web_app" "app" {
  name                = var.app_name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = var.app_service_plan_id

  site_config {
    application_stack {
      docker_image     = "${var.container_registry}/${var.container_image}"
      docker_image_tag = var.container_tag
    }
  }

  app_settings = merge({
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_ENABLE_CI"                    = "true"
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${var.container_registry}"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = var.registry_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = var.registry_password
  }, var.app_settings)

  identity {
    type = "SystemAssigned"
  }
}
