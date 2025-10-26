# Environment Configuration Guide

This guide explains how the Angular client uses environment variables for different deployment environments.

## Overview

The application uses Angular's environment file replacement system to manage configuration for different environments (development, production, etc.).

## Environment Files

### `src/environments/environment.ts` (Development)
Used for local development when running `ng serve` or `npm start`.

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5285',
};
```

### `src/environments/environment.prod.ts` (Production)
Used for production builds when running `ng build` or `npm run build`.

```typescript
export const environment = {
  production: true,
  apiUrl: '#{API_URL}#',  // Replaced during CI/CD
};
```

## How It Works

### During Development
When you run `ng serve`, Angular uses `environment.ts` by default, pointing to your local API at `http://localhost:5285`.

### During Production Build
1. Angular's build process replaces `environment.ts` with `environment.prod.ts`
2. The `#{API_URL}#` token is replaced with the actual production API URL
3. This happens in the GitHub Actions workflow before building the Docker image

## GitHub Secrets Required

Add these secrets to your GitHub repository's `production` environment:

| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `API_URL` | Production API URL | `https://api.yourdomain.com` |

### How to Add Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Environments** → **production**
3. Under **Environment secrets**, click **Add secret**
4. Add `API_URL` with your production API URL

## Services Using Environment

All HTTP services now use the environment configuration:

- `account.service.ts` - Authentication endpoints
- `posts.service.ts` - Blog post endpoints
- `comments.service.ts` - Comment endpoints
- `topics.service.ts` - Topic endpoints

Example usage:
```typescript
import { environment } from '../../environments/environment';

export class PostsService {
  private baseUrl = environment.apiUrl;
  
  getPosts() {
    return this.http.get(`${this.baseUrl}/posts`);
  }
}
```

## Local Development Setup

No additional setup needed! Just run:

```bash
cd client
npm install
npm start
```

The app will automatically use `http://localhost:5285` for API calls.

## Testing Production Configuration Locally

To test with production environment settings locally:

```bash
# Build with production configuration
npm run build

# Serve the production build
npx http-server dist/client/browser -p 4200
```

**Note:** You'll need to manually replace the `#{API_URL}#` token in the built files or set it before building.

## Cloud Deployment Examples

### Production API URLs

Depending on your cloud provider, your `API_URL` might look like:

**Azure Container Apps:**
```
https://myblog-api.azurecontainerapps.io
```

**AWS App Runner:**
```
https://abc123.us-east-1.awsapprunner.com
```

**DigitalOcean:**
```
https://api.yourdomain.com
```

**Custom Domain:**
```
https://api.myblog.com
```

## CORS Configuration

⚠️ **Important:** Make sure your API's CORS settings allow requests from your production UI domain!

In your `API/Program.cs`, update the CORS policy:

```csharp
app.UseCors(options => options
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins(
        "http://localhost:4200",  // Development
        "https://yourdomain.com"  // Production UI
    )
    .WithExposedHeaders("X-Refresh-Token"));
```

## Troubleshooting

### API calls failing in production

1. **Check the console** - Look for CORS errors or incorrect URLs
2. **Verify API_URL secret** - Ensure it's set correctly in GitHub
3. **Check CORS settings** - Make sure API allows your UI domain
4. **Verify HTTPS** - Most browsers require HTTPS for production apps

### Token not replaced

If you see `#{API_URL}#` in your production app:

1. Check that the GitHub workflow ran successfully
2. Verify the `API_URL` secret is set in the environment
3. Check the workflow logs for token replacement errors

### API URL includes trailing slash

Don't include trailing slashes in `API_URL`:

✅ Good: `https://api.yourdomain.com`  
❌ Bad: `https://api.yourdomain.com/`

The services append paths like `/posts`, so a trailing slash would create invalid URLs.

## Adding New Environment Variables

To add more environment variables:

1. **Update environment files:**
```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5285',
  newVariable: 'dev-value',
};

// environment.prod.ts
export const environment = {
  production: true,
  apiUrl: '#{API_URL}#',
  newVariable: '#{NEW_VARIABLE}#',
};
```

2. **Add to GitHub workflow** (`.github/workflows/docker-build-ui.yml`):
```yaml
- name: Replace tokens
  uses: cschleiden/replace-tokens@v1
  with:
    files: '["client/src/environments/environment.prod.ts"]'
  env:
    API_URL: ${{ secrets.API_URL }}
    NEW_VARIABLE: ${{ secrets.NEW_VARIABLE }}
```

3. **Add secret to GitHub** environment settings

## Best Practices

✅ **Do:**
- Use environment variables for all external URLs and API endpoints
- Keep development values in `environment.ts` for easy local testing
- Use token replacement for production secrets
- Document all environment variables

❌ **Don't:**
- Hardcode URLs in services
- Commit production secrets to version control
- Include sensitive data in environment files (use backend configuration instead)
- Use environment variables for non-configuration data

## Related Files

- `client/angular.json` - Configures file replacements
- `client/src/environments/` - Environment files
- `.github/workflows/docker-build-ui.yml` - CI/CD token replacement
- `client/src/core/services/*.service.ts` - Services using environment config

