# modules/staticwebapp/outputs.tf

output "id" {
  description = "The ID of the Static Web App"
  value       = azurerm_static_web_app.swa.id
}

output "default_hostname" {
  description = "The default hostname of the Static Web App"
  value       = azurerm_static_web_app.swa.default_host_name
}

output "api_key" {
  description = "API key for deployment (sensitive)"
  value       = azurerm_static_web_app.swa.api_key
  sensitive   = true
}

output "url" {
  description = "The URL of the Static Web App"
  value       = "https://${azurerm_static_web_app.swa.default_host_name}"
}
