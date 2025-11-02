output "sql_server_name" {
  value = azurerm_mssql_server.sqlserver.name
}

output "sql_database_name" {
  value = azurerm_mssql_database.sqldatabase.name
}

output "db_connection_string" {
  value     = local.db_connection_string
  sensitive = true
}

output "sql_password" {
  value     = random_password.sql_password.result
  sensitive = true
}
