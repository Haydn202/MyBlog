# Complete Environment Configuration Guide

This document provides a comprehensive overview of all environment configurations for both the API and Client.

## Overview

The application uses environment-specific configuration for:
- ✅ **API URLs** (Client → API communication)
- ✅ **CORS Origins** (API → Client allowed origins)
- ✅ **Admin Credentials** (Initial admin account)
- ✅ **JWT Token Keys** (Authentication security)
- ✅ **Database Connections** (Production database)

---

## 🔧 Configuration Files

### API (Backend)

#### Development: `API/appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data source=blog.db"
  },
  "AdminSettings": {
    "UserName": "admin",
    "Email": "admin@email.com",
    "Password": "admin"
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "https://localhost:4200",
      "http://localhost:4201",
      "https://localhost:4201"
    ]
  }
}
```

#### Production: `API/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "#{CONNECTION_STRING}#"
  },
  "TokenKey": "#{TOKEN_KEY}#",
  "AdminSettings": {
    "UserName": "#{ADMIN_USERNAME}#",
    "Email": "#{ADMIN_EMAIL}#",
    "Password": "#{ADMIN_PASSWORD}#"
  },
  "CorsSettings": {
    "AllowedOrigins": ["#{CLIENT_URL}#"]
  }
}
```

### Client (Frontend)

#### Development: `client/src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5285',
};
```

#### Production: `client/src/environments/environment.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: '#{API_URL}#',  // Replaced during build
};
```

---

## 🔐 GitHub Secrets Required

Add these secrets to your GitHub repository's **production environment**:

### API Secrets
| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `TOKEN_KEY` | JWT signing key (64+ chars) | Generated via `openssl rand -base64 64` |
| `ADMIN_USERNAME` | Initial admin username | `admin` |
| `ADMIN_EMAIL` | Initial admin email | `admin@yourdomain.com` |
| `ADMIN_PASSWORD` | Initial admin password | Strong password (20+ chars) |
| `CLIENT_URL` | Production client URL (for CORS) | `https://yourdomain.com` |
| `CONNECTION_STRING` | Production database connection | Database-specific connection string |

### Client Secrets
| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `API_URL` | Production API endpoint | `https://api.yourdomain.com` |

### Docker Hub Secrets
| Secret Name | Description |
|-------------|-------------|
| `DOCKERHUB_USERNAME` | Your Docker Hub username |
| `DOCKERHUB_TOKEN` | Docker Hub access token |

---

## 📋 Setup Instructions

### 1. Generate Secure Secrets

#### Generate TOKEN_KEY
```bash
openssl rand -base64 64
```

#### Generate ADMIN_PASSWORD
```bash
openssl rand -base64 24
```

### 2. Add Secrets to GitHub

1. Go to your GitHub repository
2. Navigate to **Settings** → **Environments** → **production**
3. Click **Add secret** for each secret above
4. Paste the generated values

### 3. Configure Your Production URLs

Based on your cloud provider:

**Azure Container Apps:**
```
API_URL: https://myblog-api.azurecontainerapps.io
CLIENT_URL: https://myblog-ui.azurecontainerapps.io
```

**DigitalOcean with Custom Domain:**
```
API_URL: https://api.yourdomain.com
CLIENT_URL: https://yourdomain.com
```

**AWS:**
```
API_URL: https://abc123.us-east-1.awsapprunner.com
CLIENT_URL: https://def456.us-east-1.awsapprunner.com
```

---

## 🚀 How It Works

### Development Flow
1. Run `dotnet run` in API directory → Uses `appsettings.Development.json`
2. Run `npm start` in client directory → Uses `environment.ts`
3. API allows CORS from localhost:4200
4. Client connects to localhost:5285

### Production Flow
1. Push code to GitHub (master branch)
2. **API Workflow** (`docker-publish.yml`):
   - Replaces tokens in `appsettings.json`
   - Builds Docker image with production config
   - Pushes to Docker Hub
3. **Client Workflow** (`docker-build-ui.yml`):
   - Replaces `#{API_URL}#` in `environment.prod.ts`
   - Builds Angular with production config
   - Pushes Docker image to Docker Hub
4. Deploy both images to your cloud provider

---

## 🔍 Verification Checklist

Before deploying to production, verify:

- [ ] All GitHub secrets are set in the production environment
- [ ] `CLIENT_URL` matches your actual client domain (no trailing slash)
- [ ] `API_URL` matches your actual API domain (no trailing slash)
- [ ] `TOKEN_KEY` is at least 64 characters
- [ ] `ADMIN_PASSWORD` is strong (20+ characters)
- [ ] CORS `CLIENT_URL` includes the protocol (`https://`)
- [ ] Both workflows run successfully in GitHub Actions

---

## 🧪 Testing Locally

### Test Production API Config Locally

1. Create a temporary `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data source=blog-prod.db"
  },
  "TokenKey": "your-test-token-key",
  "AdminSettings": {
    "UserName": "testadmin",
    "Email": "test@example.com",
    "Password": "TestPassword123!"
  },
  "CorsSettings": {
    "AllowedOrigins": ["https://test.example.com"]
  }
}
```

2. Run with production environment:
```bash
cd API
dotnet run --environment Production
```

### Test Production Client Config Locally

1. Temporarily update `environment.prod.ts` with real URL
2. Build and serve:
```bash
cd client
npm run build
npx http-server dist/client/browser -p 4200
```

---

## 🐛 Troubleshooting

### CORS Errors in Production

**Problem:** API rejects client requests

**Solutions:**
1. Verify `CLIENT_URL` secret matches your actual client domain
2. Check for trailing slashes (remove them)
3. Ensure protocol is included (`https://`)
4. Check API logs for CORS error details

### Token Replacement Failed

**Problem:** See `#{TOKEN}#` in production app

**Solutions:**
1. Check GitHub Actions workflow logs
2. Verify secret names match exactly (case-sensitive)
3. Ensure workflow has `environment: production` set
4. Check that secrets are in the **environment**, not repository level

### API Can't Start

**Problem:** Application fails to start in production

**Solutions:**
1. Check that all required secrets are set
2. Verify `CONNECTION_STRING` format is correct
3. Check logs for specific configuration errors
4. Ensure `CorsSettings.AllowedOrigins` is not empty

### Client Can't Connect to API

**Problem:** Client shows connection errors

**Solutions:**
1. Verify `API_URL` is accessible from browser
2. Check browser console for CORS errors
3. Verify API is actually running
4. Check that API's CORS includes your client URL

---

## 📖 Related Documentation

- [Client Environment Setup](client/ENVIRONMENT_SETUP.md) - Detailed client configuration
- [Docker Workflows](.github/workflows/) - CI/CD pipeline details
- [API Options](API/Options/) - Configuration classes

---

## 🔒 Security Best Practices

1. **Never commit secrets** to version control
2. **Rotate secrets regularly** (especially TOKEN_KEY)
3. **Use strong passwords** (20+ characters, mixed case, numbers, symbols)
4. **Limit CORS origins** to only your actual domains
5. **Use HTTPS** in production (always)
6. **Keep secrets backed up** securely (password manager)
7. **Use different secrets** per environment
8. **Review GitHub Actions logs** carefully (secrets are masked but errors aren't)

---

## 🎯 Quick Reference

### Local Development URLs
```
Client:  http://localhost:4200
API:     http://localhost:5285
```

### Production Secrets Needed
```
API:
- TOKEN_KEY
- ADMIN_USERNAME
- ADMIN_EMAIL
- ADMIN_PASSWORD
- CLIENT_URL
- CONNECTION_STRING

Client:
- API_URL

Docker:
- DOCKERHUB_USERNAME
- DOCKERHUB_TOKEN
```

### Commands
```bash
# Generate token key
openssl rand -base64 64

# Generate password
openssl rand -base64 24

# Test API locally
cd API && dotnet run

# Test Client locally
cd client && npm start

# Build for production
cd client && npm run build
```

