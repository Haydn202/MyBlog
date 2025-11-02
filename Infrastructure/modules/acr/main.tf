resource "azurerm_resource_group" "acr" {
  count    = var.create_resource_group ? 1 : 0
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_container_registry" "acr" {
  name                = var.name
  resource_group_name = var.create_resource_group ? azurerm_resource_group.acr[0].name : var.resource_group_name
  location            = var.location
  sku                 = var.sku
  admin_enabled       = var.admin_enabled

  tags = var.tags
}
