# modules/containerinstance/main.tf

resource "azurerm_container_group" "container" {
  name                = var.container_name
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  ip_address_type     = "Public"
  dns_name_label      = var.dns_name_label

  image_registry_credential {
    server   = var.registry_server
    username = var.registry_username
    password = var.registry_password
  }

  container {
    name   = var.container_name
    image  = "${var.registry_server}/${var.image_name}:${var.image_tag}"
    cpu    = var.cpu
    memory = var.memory

    ports {
      port     = var.container_port
      protocol = "TCP"
    }

    environment_variables        = var.environment_variables
    secure_environment_variables = var.secure_environment_variables
  }

  tags = var.tags
}
