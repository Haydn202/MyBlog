output "keyvault_id" {
  description = "The ID of the Key Vault"
  value       = azurerm_key_vault.keyvault.id
}

output "keyvault_name" {
  description = "The name of the Key Vault"
  value       = azurerm_key_vault.keyvault.name
}

output "keyvault_uri" {
  description = "The URI of the Key Vault"
  value       = azurerm_key_vault.keyvault.vault_uri
}

output "sp_access_policy_id" {
  description = "The ID of the access policy for the service principal"
  value       = azurerm_key_vault_access_policy.sp.id
}

output "secret_ids" {
  description = "The IDs of any secrets created"
  value       = { for k, s in azurerm_key_vault_secret.secrets : k => s.id }
}
