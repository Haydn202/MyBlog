# modules/frontdoor/variables.tf

variable "profile_name" {
  description = "Name of the Front Door profile"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "ui_origin_hostname" {
  description = "Hostname of the UI origin (container instance FQDN)"
  type        = string
}

variable "api_origin_hostname" {
  description = "Hostname of the API origin (container instance FQDN)"
  type        = string
}

variable "tags" {
  description = "Tags for resources"
  type        = map(string)
  default     = {}
}
