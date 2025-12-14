# Infrastructure Implementation Guide

A step-by-step guide to deploy the Rubber Duck Diaries infrastructure on Azure.

---

## Prerequisites

Before starting, ensure you have:

- [ ] Azure CLI installed (`az --version`)
- [ ] Terraform installed (`terraform --version`)
- [ ] An Azure subscription with Owner/Contributor access
- [ ] GitHub repository access with admin permissions

---

## Step 1: Verify Azure Authentication

```bash
# Login to Azure
az login

# Verify subscription
az account show

# If needed, set the correct subscription
az account set --subscription "YOUR_SUBSCRIPTION_ID"
```

---

## Step 2: Add SQL Server Firewall Rules

The SQL module is missing firewall rules. Add them to allow Azure services to connect.

**File:** `modules/sql/main.tf`

Add after the `azurerm_mssql_database` resource:

```hcl
# Allow Azure services to access SQL Server
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
```

---

## Step 3: Update App Service Module (Fix Deprecated Resources)

Replace the entire contents of `modules/appservice/main.tf`:

```hcl
# modules/appservice/main.tf

resource "azurerm_linux_web_app" "app" {
  name                = var.app_name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = var.app_service_plan_id

  site_config {
    application_stack {
      docker_image_name        = "${var.container_image}:${var.container_tag}"
      docker_registry_url      = "https://${var.container_registry}"
      docker_registry_username = var.registry_username
      docker_registry_password = var.registry_password
    }
  }

  app_settings = merge({
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_ENABLE_CI"                    = "true"
  }, var.app_settings)

  identity {
    type = "SystemAssigned"
  }
}
```

---

## Step 4: Update App Service Module Variables

Replace the contents of `modules/appservice/variables.tf`:

```hcl
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
  default     = "australiaeast"
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
```

---

## Step 5: Update App Service Module Outputs

Replace the contents of `modules/appservice/outputs.tf`:

```hcl
# modules/appservice/outputs.tf

output "app_id" {
  description = "The ID of the App Service"
  value       = azurerm_linux_web_app.app.id
}

output "default_hostname" {
  description = "The default hostname of the App Service"
  value       = azurerm_linux_web_app.app.default_hostname
}

output "principal_id" {
  description = "The Principal ID of the App Service's managed identity"
  value       = azurerm_linux_web_app.app.identity[0].principal_id
}
```

---

## Step 6: Update Main Terraform Configuration

Replace the entire `main.tf` file:

```hcl
# main.tf

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

# =============================================================================
# Resource Group
# =============================================================================
resource "azurerm_resource_group" "rg" {
  name     = "RG-RubberDuckDiaries"
  location = "australiaeast"
}

# =============================================================================
# Container Registry
# =============================================================================
module "acr" {
  source              = "./modules/acr"
  name                = "rubberduckdiariesacr"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true
  tags = {
    project = "RubberDuckDiaries"
    env     = "prod"
  }
}

# =============================================================================
# Key Vault
# =============================================================================
module "keyvault" {
  source              = "./modules/keyvault"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = "845c67a0-eb3f-4822-be54-4f78446fc867"
  sp_object_id        = "26a5e7bc-b888-4aa1-b248-a1750a80f0b4"
  keyvault_name       = "rubberduckdiaries-kv"
}

# =============================================================================
# SQL Database
# =============================================================================
module "sql_db" {
  source              = "./modules/sql"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sql_server_name     = "rubberduckdiaries-sqlserver"
  sql_db_name         = "RubberDuckDiariesDB"
  sql_admin           = "adminuser"
  keyvault_id         = module.keyvault.keyvault_id
}

# =============================================================================
# App Service Plan (Shared for both UI and API)
# =============================================================================
module "app_service_plan" {
  source              = "./modules/appserviceplan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  plan_name           = "rubberduckdiaries-plan"
  sku_name            = "B1"
}

# =============================================================================
# API App Service
# =============================================================================
module "api_app_service" {
  source              = "./modules/appservice"
  app_name            = "rubberduckdiaries-api"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  app_service_plan_id = module.app_service_plan.id
  container_image     = "rubberduckdiaries-api"
  container_tag       = "latest"
  container_registry  = module.acr.login_server
  registry_username   = module.acr.admin_username
  registry_password   = module.acr.admin_password

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
  }
}

# =============================================================================
# UI App Service
# =============================================================================
module "ui_app_service" {
  source              = "./modules/appservice"
  app_name            = "rubberduckdiaries-ui"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  app_service_plan_id = module.app_service_plan.id
  container_image     = "rubberduckdiaries-ui"
  container_tag       = "latest"
  container_registry  = module.acr.login_server
  registry_username   = module.acr.admin_username
  registry_password   = module.acr.admin_password

  app_settings = {}
}

# =============================================================================
# Outputs
# =============================================================================
output "api_url" {
  description = "URL of the API App Service"
  value       = "https://${module.api_app_service.default_hostname}"
}

output "ui_url" {
  description = "URL of the UI App Service"
  value       = "https://${module.ui_app_service.default_hostname}"
}

output "acr_login_server" {
  description = "ACR login server for pushing images"
  value       = module.acr.login_server
}

output "sql_server_name" {
  description = "SQL Server name"
  value       = module.sql_db.sql_server_name
}
```

---

## Step 7: Deploy Infrastructure with Terraform

```bash
cd Infrastructure

# Initialize Terraform (download providers)
terraform init

# Validate configuration
terraform validate

# Preview changes
terraform plan

# Apply changes (type 'yes' when prompted)
terraform apply
```

**Expected output after apply:**
```
api_url = "https://rubberduckdiaries-api.azurewebsites.net"
ui_url = "https://rubberduckdiaries-ui.azurewebsites.net"
acr_login_server = "rubberduckdiariesacr.azurecr.io"
sql_server_name = "rubberduckdiaries-sqlserver"
```

---

## Step 8: Get Connection String from Key Vault

```bash
# Retrieve the database connection string
az keyvault secret show \
  --vault-name rubberduckdiaries-kv \
  --name DbConnectionString \
  --query value -o tsv
```

Save this value - you'll need it for GitHub secrets.

---

## Step 9: Generate a JWT Token Key

Generate a secure random key for JWT signing:

```bash
# Option 1: Using openssl
openssl rand -base64 64

# Option 2: Using Python
python3 -c "import secrets; print(secrets.token_urlsafe(64))"
```

Save this value - you'll need it for GitHub secrets.

---

## Step 10: Configure GitHub Secrets

Go to your GitHub repository → **Settings** → **Secrets and variables** → **Actions**

Create these secrets:

| Secret Name | Value | How to Get |
|-------------|-------|------------|
| `AZURE_CREDENTIALS` | Service Principal JSON | See Step 10a below |
| `CONNECTION_STRING` | Database connection string | From Step 8 |
| `TOKEN_KEY` | JWT signing key | From Step 9 |
| `ADMIN_USERNAME` | Your choice (e.g., `admin`) | Choose a username |
| `ADMIN_EMAIL` | Your email | Your email address |
| `ADMIN_PASSWORD` | Strong password | Generate secure password |
| `CLIENT_URL` | `https://rubberduckdiaries-ui.azurewebsites.net` | From Terraform output |
| `API_URL` | `https://rubberduckdiaries-api.azurewebsites.net` | From Terraform output |

### Step 10a: Create Azure Service Principal for GitHub

```bash
# Create service principal with Contributor role
az ad sp create-for-rbac \
  --name "github-actions-rubberduckdiaries" \
  --role contributor \
  --scopes /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/RG-RubberDuckDiaries \
  --sdk-auth
```

Copy the entire JSON output and save it as the `AZURE_CREDENTIALS` GitHub secret.

---

## Step 11: Push Initial Docker Images

The App Services need images in ACR before they can start. Run the GitHub Actions workflows or push manually:

### Option A: Trigger GitHub Actions (Recommended)

1. Make a small change to a file in `API/` and push to master
2. Make a small change to a file in `client/` and push to master
3. Check GitHub Actions tab to see builds running

### Option B: Push Manually

```bash
# Login to ACR
az acr login --name rubberduckdiariesacr

# Build and push API
cd API
docker build -t rubberduckdiariesacr.azurecr.io/rubberduckdiaries-api:latest .
docker push rubberduckdiariesacr.azurecr.io/rubberduckdiaries-api:latest

# Build and push UI
cd ../client
docker build -t rubberduckdiariesacr.azurecr.io/rubberduckdiaries-ui:latest .
docker push rubberduckdiariesacr.azurecr.io/rubberduckdiaries-ui:latest
```

---

## Step 12: Verify Deployment

### Check App Services are running:

```bash
# Check API health
curl https://rubberduckdiaries-api.azurewebsites.net/health

# Check UI is serving
curl -I https://rubberduckdiaries-ui.azurewebsites.net
```

### View App Service logs (if issues):

```bash
# Stream API logs
az webapp log tail \
  --name rubberduckdiaries-api \
  --resource-group RG-RubberDuckDiaries

# Stream UI logs
az webapp log tail \
  --name rubberduckdiaries-ui \
  --resource-group RG-RubberDuckDiaries
```

---

## Step 13: Run Database Migrations

The API should run EF Core migrations on startup, but if needed:

```bash
# Connect to SQL Server and verify database
az sql db show \
  --name RubberDuckDiariesDB \
  --server rubberduckdiaries-sqlserver \
  --resource-group RG-RubberDuckDiaries
```

---

## Troubleshooting

### Issue: App Service shows "Application Error"

1. Check logs: `az webapp log tail --name APP_NAME --resource-group RG-RubberDuckDiaries`
2. Verify image exists in ACR: `az acr repository list --name rubberduckdiariesacr`
3. Restart app: `az webapp restart --name APP_NAME --resource-group RG-RubberDuckDiaries`

### Issue: Cannot connect to database

1. Check firewall rules: Azure Portal → SQL Server → Networking
2. Verify connection string is correct in Key Vault
3. Test connectivity from App Service console

### Issue: CORS errors in browser

1. Verify `CLIENT_URL` secret matches the actual UI URL
2. Check API logs for CORS rejection messages
3. Redeploy API after fixing secrets

### Issue: Docker image pull fails

1. Verify ACR credentials are correct
2. Check image name matches exactly
3. Verify image was pushed successfully: `az acr repository show-tags --name rubberduckdiariesacr --repository rubberduckdiaries-api`

---

## Summary Checklist

- [ ] Step 1: Azure authentication verified
- [ ] Step 2: SQL firewall rules added
- [ ] Step 3: App Service module updated
- [ ] Step 4: App Service variables updated
- [ ] Step 5: App Service outputs updated
- [ ] Step 6: Main.tf updated
- [ ] Step 7: Terraform applied successfully
- [ ] Step 8: Connection string retrieved
- [ ] Step 9: JWT token key generated
- [ ] Step 10: GitHub secrets configured
- [ ] Step 11: Docker images pushed to ACR
- [ ] Step 12: Deployment verified
- [ ] Step 13: Database migrations complete

---

## What's Next?

After successful deployment, consider:

1. **Custom Domain**: Add your own domain name to the App Services
2. **SSL Certificate**: Configure managed certificates for HTTPS
3. **Monitoring**: Set up Azure Application Insights
4. **Scaling**: Adjust App Service Plan SKU based on traffic
5. **Backups**: Configure SQL database backups
