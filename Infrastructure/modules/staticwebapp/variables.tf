# modules/staticwebapp/variables.tf

variable "app_name" {
  description = "Name of the Static Web App"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "centralus"
}

variable "tags" {
  description = "Tags for resources"
  type        = map(string)
  default     = {}
}
