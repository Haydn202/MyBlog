# Log Analytics workspace (required by Container App Environment for logging)
resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.app_name}-law"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = var.tags
}

# Container App Environment
resource "azurerm_container_app_environment" "env" {
  name                       = "${var.app_name}-env"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id

  tags = var.tags
}

# Registry auth only when password is set (e.g. private registry). Public Docker Hub needs no credentials.
resource "azurerm_container_app" "app" {
  name                        = var.app_name
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name         = var.resource_group_name
  revision_mode               = "Single"

  dynamic "secret" {
    for_each = var.registry_password != null ? [1] : []
    content {
      name  = "registry-password"
      value = var.registry_password
    }
  }

  dynamic "registry" {
    for_each = var.registry_password != null ? [1] : []
    content {
      server               = var.registry_server
      username             = var.registry_username
      password_secret_name = "registry-password"
    }
  }

  template {
    min_replicas = var.min_replicas
    max_replicas = var.max_replicas

    container {
      name   = var.app_name
      image  = "${var.registry_server}/${var.image_name}:${var.image_tag}"
      cpu    = var.cpu
      memory = "${var.memory_gb}Gi"

      dynamic "env" {
        for_each = var.environment_variables
        content {
          name  = env.key
          value = env.value
        }
      }

      liveness_probe {
        transport = "HTTP"
        path      = "/"
        port      = var.container_port
        initial_delay = 10
        interval_seconds = 10
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = var.container_port
    transport        = "http"

    traffic_weight {
      percentage     = 100
      latest_revision = true
    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
}

# Custom domain + certificate: add in Azure portal (Container App → Custom domains).
# Terraform does not create azurerm_container_app_custom_domain because Azure-managed certs use
# managedCertificates IDs; the provider only accepts certificates/ IDs, causing a parse error on refresh.
