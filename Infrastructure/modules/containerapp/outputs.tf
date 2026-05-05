output "id" {
  description = "The ID of the Container App"
  value       = azurerm_container_app.app.id
}

output "fqdn" {
  description = "The FQDN of the Container App (default ingress)"
  value       = azurerm_container_app.app.ingress[0].fqdn
}

output "url" {
  description = "The default URL to access the app (HTTP)"
  value       = "https://${azurerm_container_app.app.ingress[0].fqdn}"
}

output "latest_revision_fqdn" {
  description = "FQDN of the latest revision"
  value       = azurerm_container_app.app.latest_revision_fqdn
}

output "principal_id" {
  description = "Principal ID of the Container App's system-assigned identity (for Key Vault access)"
  value       = azurerm_container_app.app.identity[0].principal_id
}

output "custom_domain_verification_id" {
  description = "TXT record value for custom domain verification. Add DNS TXT: asuid.<custom_domain> = this value (e.g. in Cloudflare)."
  value       = azurerm_container_app.app.custom_domain_verification_id
  sensitive   = true
}
