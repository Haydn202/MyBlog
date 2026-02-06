# modules/staticwebapp/main.tf

resource "azurerm_static_web_app" "swa" {
  name                = var.app_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku_tier            = "Free"
  sku_size            = "Free"

  tags = var.tags
}

# Custom domain configuration
# Apex domains (root domains) require dns-txt-token validation
resource "azurerm_static_web_app_custom_domain" "custom_domain" {
  count              = var.custom_domain != "" ? 1 : 0
  static_web_app_id  = azurerm_static_web_app.swa.id
  domain_name        = var.custom_domain
  validation_type    = "dns-txt-token"
}
