# modules/containerinstance/variables.tf

variable "container_name" {
  description = "Name of the container group"
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

variable "dns_name_label" {
  description = "DNS name label for the container (creates {label}.{region}.azurecontainer.io)"
  type        = string
}

variable "registry_server" {
  description = "Container registry server (e.g., myacr.azurecr.io)"
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

variable "image_name" {
  description = "Container image name"
  type        = string
}

variable "image_tag" {
  description = "Container image tag"
  type        = string
  default     = "latest"
}

variable "cpu" {
  description = "CPU cores for the container"
  type        = number
  default     = 0.5
}

variable "memory" {
  description = "Memory in GB for the container"
  type        = number
  default     = 1
}

variable "container_port" {
  description = "Port the container listens on"
  type        = number
  default     = 80
}

variable "environment_variables" {
  description = "Environment variables for the container"
  type        = map(string)
  default     = {}
}

variable "secure_environment_variables" {
  description = "Secure environment variables for the container"
  type        = map(string)
  default     = {}
  sensitive   = true
}

variable "tags" {
  description = "Tags for the container group"
  type        = map(string)
  default     = {}
}
