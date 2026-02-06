// modules/sql/main.tf

# Generate a strong SQL admin password
resource "random_password" "sql_password" {
  length  = 16
  special = true
}

# SQL Server
resource "azurerm_mssql_server" "sqlserver" {
  name                         = var.sql_server_name
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.sql_admin
  administrator_login_password = random_password.sql_password.result

  # Optional: enable public access
  public_network_access_enabled = true
}

# SQL Database (Serverless - auto-pause when idle)
resource "azurerm_mssql_database" "sqldatabase" {
  name                        = var.sql_db_name
  server_id                   = azurerm_mssql_server.sqlserver.id
  sku_name                    = "Basic"
  min_capacity                = 0.5
  auto_pause_delay_in_minutes = 20
  max_size_gb                 = 32
}

# Allow Azure services to access SQL Server
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Local connection string
locals {
  db_connection_string = "Server=tcp:${azurerm_mssql_server.sqlserver.name}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.sqldatabase.name};Persist Security Info=False;User ID=${var.sql_admin};Password=${random_password.sql_password.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}

# Store connection string in Key Vault
resource "azurerm_key_vault_secret" "db_connection" {
  name         = "DbConnectionString"
  value        = local.db_connection_string
  key_vault_id = var.keyvault_id

  lifecycle {
    ignore_changes  = [value] # don't overwrite manual edits
    prevent_destroy = true
  }
}

# Store SQL password in Key Vault
resource "azurerm_key_vault_secret" "sql_password" {
  name         = "SqlAdminPassword"
  value        = random_password.sql_password.result
  key_vault_id = var.keyvault_id

  lifecycle {
    ignore_changes  = [value]
    prevent_destroy = true
  }
}
