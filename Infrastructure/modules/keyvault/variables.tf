variable "resource_group_name" {
  description = "The resource group where Key Vault will be created"
  type        = string
}

variable "location" {
  description = "Azure region where Key Vault will be deployed"
  type        = string
  default     = "centralus"
}

variable "tenant_id" {
  description = "Azure AD tenant ID for Key Vault"
  type        = string
}

variable "sp_object_id" {
  description = "Object ID of the Service Principal that will access the Key Vault"
  type        = string
}

variable "keyvault_name" {
  description = "The name of the Key Vault"
  type        = string
}

variable "secrets" {
  description = "Optional map of secrets to create in the Key Vault"
  type        = map(string)
  default     = {}
}
