# modules/containerinstance/outputs.tf

output "id" {
  description = "The ID of the container group"
  value       = azurerm_container_group.container.id
}

output "fqdn" {
  description = "The FQDN of the container group"
  value       = azurerm_container_group.container.fqdn
}

output "ip_address" {
  description = "The IP address of the container group"
  value       = azurerm_container_group.container.ip_address
}

output "url" {
  description = "The full URL to access the container"
  value       = "http://${azurerm_container_group.container.fqdn}"
}
