# modules/frontdoor/main.tf

resource "azurerm_cdn_frontdoor_profile" "fd" {
  name                = var.profile_name
  resource_group_name = var.resource_group_name
  sku_name            = "Standard_AzureFrontDoor"

  tags = var.tags
}

# =============================================================================
# UI Endpoint
# =============================================================================
resource "azurerm_cdn_frontdoor_endpoint" "ui" {
  name                     = "${var.profile_name}-ui"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.fd.id
}

resource "azurerm_cdn_frontdoor_origin_group" "ui" {
  name                     = "ui-origin-group"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.fd.id

  load_balancing {
    sample_size                 = 4
    successful_samples_required = 3
  }

  health_probe {
    path                = "/"
    request_type        = "HEAD"
    protocol            = "Http"
    interval_in_seconds = 100
  }
}

resource "azurerm_cdn_frontdoor_origin" "ui" {
  name                          = "ui-origin"
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.ui.id
  enabled                       = true

  host_name          = var.ui_origin_hostname
  http_port          = 80
  https_port         = 443
  origin_host_header = var.ui_origin_hostname
  priority           = 1
  weight             = 1000

  certificate_name_check_enabled = false
}

resource "azurerm_cdn_frontdoor_route" "ui" {
  name                          = "ui-route"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.ui.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.ui.id
  cdn_frontdoor_origin_ids      = [azurerm_cdn_frontdoor_origin.ui.id]

  supported_protocols    = ["Http", "Https"]
  patterns_to_match      = ["/*"]
  forwarding_protocol    = "HttpOnly"
  link_to_default_domain = true
  https_redirect_enabled = true
}

# =============================================================================
# API Endpoint
# =============================================================================
resource "azurerm_cdn_frontdoor_endpoint" "api" {
  name                     = "${var.profile_name}-api"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.fd.id
}

resource "azurerm_cdn_frontdoor_origin_group" "api" {
  name                     = "api-origin-group"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.fd.id

  load_balancing {
    sample_size                 = 4
    successful_samples_required = 3
  }

  health_probe {
    path                = "/health"
    request_type        = "GET"
    protocol            = "Http"
    interval_in_seconds = 100
  }
}

resource "azurerm_cdn_frontdoor_origin" "api" {
  name                          = "api-origin"
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.api.id
  enabled                       = true

  host_name          = var.api_origin_hostname
  http_port          = 80
  https_port         = 443
  origin_host_header = var.api_origin_hostname
  priority           = 1
  weight             = 1000

  certificate_name_check_enabled = false
}

resource "azurerm_cdn_frontdoor_route" "api" {
  name                          = "api-route"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.api.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.api.id
  cdn_frontdoor_origin_ids      = [azurerm_cdn_frontdoor_origin.api.id]

  supported_protocols    = ["Http", "Https"]
  patterns_to_match      = ["/*"]
  forwarding_protocol    = "HttpOnly"
  link_to_default_domain = true
  https_redirect_enabled = true
}
