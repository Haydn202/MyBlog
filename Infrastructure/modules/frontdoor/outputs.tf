# modules/frontdoor/outputs.tf

output "profile_id" {
  description = "The ID of the Front Door profile"
  value       = azurerm_cdn_frontdoor_profile.fd.id
}

output "ui_endpoint_hostname" {
  description = "The hostname of the UI endpoint"
  value       = azurerm_cdn_frontdoor_endpoint.ui.host_name
}

output "api_endpoint_hostname" {
  description = "The hostname of the API endpoint"
  value       = azurerm_cdn_frontdoor_endpoint.api.host_name
}

output "ui_url" {
  description = "The HTTPS URL for the UI"
  value       = "https://${azurerm_cdn_frontdoor_endpoint.ui.host_name}"
}

output "api_url" {
  description = "The HTTPS URL for the API"
  value       = "https://${azurerm_cdn_frontdoor_endpoint.api.host_name}"
}
