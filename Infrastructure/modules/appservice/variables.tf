variable "app_name" {}
variable "resource_group_name" {}
variable "location" { default = "australiaeast" }
variable "acr_name" {}
variable "container_image" {}
variable "container_tag" { default = "latest" }
variable "keyvault_id" {}
