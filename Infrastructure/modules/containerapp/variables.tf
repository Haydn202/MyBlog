variable "app_name" {
  description = "Name of the Container App"
  type        = string
}

variable "environment_name" {
  description = "Name of the Container App Environment"
  type        = string
  default     = "default"
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

variable "registry_server" {
  description = "Container registry server (e.g. docker.io or myacr.azurecr.io)"
  type        = string
}

variable "registry_username" {
  description = "Container registry username (omit for public images, e.g. public Docker Hub)"
  type        = string
  default     = null
}

variable "registry_password" {
  description = "Container registry password (omit for public images)"
  type        = string
  default     = null
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

variable "container_port" {
  description = "Port the container listens on"
  type        = number
  default     = 80
}

variable "cpu" {
  description = "CPU cores for the container (Consumption: 0.25, 0.5, 1, 2)"
  type        = number
  default     = 0.25
}

variable "memory_gb" {
  description = "Memory in GB for the container (e.g. 0.5 for 0.5Gi)"
  type        = string
  default     = "0.5"
}

variable "environment_variables" {
  description = "Environment variables for the container"
  type        = map(string)
  default     = {}
}

variable "min_replicas" {
  description = "Minimum number of replicas (0 for scale-to-zero)"
  type        = number
  default     = 0
}

variable "max_replicas" {
  description = "Maximum number of replicas"
  type        = number
  default     = 10
}

variable "tags" {
  description = "Tags for the resources"
  type        = map(string)
  default     = {}
}

variable "custom_domain" {
  description = "Custom domain for the Container App (e.g. api.example.com). Leave empty to use default FQDN only. Requires TXT record asuid.<custom_domain> with custom_domain_verification_id for managed cert."
  type        = string
  default     = ""
}
