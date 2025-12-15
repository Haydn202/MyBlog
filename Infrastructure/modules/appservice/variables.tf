# modules/appservice/variables.tf

variable "app_name" {
  description = "Name of the App Service"
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

variable "app_service_plan_id" {
  description = "ID of the App Service Plan"
  type        = string
}

variable "container_image" {
  description = "Container image name (without tag)"
  type        = string
}

variable "container_tag" {
  description = "Container image tag"
  type        = string
  default     = "latest"
}

variable "container_registry" {
  description = "Container registry login server (e.g., myacr.azurecr.io)"
  type        = string
}

variable "registry_username" {
  description = "Container registry username"
  type        = string
}

variable "registry_password" {
  description = "Container registry password"
  type        = string
  sensitive   = true
}

variable "app_settings" {
  description = "Additional app settings"
  type        = map(string)
  default     = {}
}
