variable "name" {
  description = "The name of the Azure Container Registry. Must be globally unique."
  type        = string
}

variable "resource_group_name" {
  description = "The resource group name where the ACR will be created."
  type        = string
  default     = ""
}

variable "location" {
  description = "Azure location for the registry."
  type        = string
  default     = "Australia East"
}

variable "sku" {
  description = "SKU of the container registry. Options: Basic, Standard, Premium"
  type        = string
  default     = "Basic"
}

variable "admin_enabled" {
  description = "Enable the admin account on the registry (optional)."
  type        = bool
  default     = false
}

variable "tags" {
  description = "Tags to assign to the ACR."
  type        = map(string)
  default     = {}
}

variable "create_resource_group" {
  description = "Whether to create a new resource group for the ACR."
  type        = bool
  default     = false
}
