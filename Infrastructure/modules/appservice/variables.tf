variable "app_name" {}
variable "resource_group_name" {}
variable "location" { default = "australiaeast" }
variable "acr_name" {}
variable "container_image" {}
variable "keyvault_id" {}
variable "app_service_plan_id" {}
variable "container_registry" {}
variable "registry_username" {}
variable "registry_password" {}
variable "container_tag" {
  default = "latest"
}
