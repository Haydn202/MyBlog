# modules/appservice/outputs.tf

output "app_id" {
  description = "The ID of the App Service"
  value       = azurerm_linux_web_app.app.id
}

output "default_hostname" {
  description = "The default hostname of the App Service"
  value       = azurerm_linux_web_app.app.default_hostname
}

output "principal_id" {
  description = "The Principal ID of the App Service's managed identity"
  value       = azurerm_linux_web_app.app.identity[0].principal_id
}

output "name" {
  description = "The name of the App Service"
  value       = azurerm_linux_web_app.app.name
}
