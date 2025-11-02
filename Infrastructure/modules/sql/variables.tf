variable "sql_server_name" {
  type        = string
  description = "Name of the SQL server"
}

variable "sql_db_name" {
  type        = string
  description = "Name of the SQL database"
}

variable "sql_admin" {
  type        = string
  description = "SQL administrator login name"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name for SQL server and DB"
}

variable "location" {
  type        = string
  description = "Azure region for SQL resources"
}

variable "keyvault_id" {
  type        = string
  description = "Resource ID of the Key Vault to store secrets"
}
