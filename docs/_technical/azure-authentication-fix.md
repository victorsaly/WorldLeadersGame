# Azure Deployment Authentication Fix

**Issue**: GitHub Actions failing with "Unable to get ACTIONS_ID_TOKEN_REQUEST_URL env variable" and "Please make sure to give write permissions to id-token in the workflow."

**Root Cause**: The Azure login action requires OIDC (OpenID Connect) authentication with proper workflow permissions and federated credentials.

## 🔧 Solution Applied

### 1. Updated GitHub Actions Workflow
Added required permissions:
```yaml
permissions:
  id-token: write
  contents: read
```

Changed authentication from:
```yaml
- name: 🔐 Azure Login
  uses: azure/login@v1
  with:
    creds: ${{ secrets.AZURE_CREDENTIALS }}
```

To modern OIDC approach:
```yaml
- name: 🔐 Azure Login
  uses: azure/login@v1
  with:
    client-id: ${{ secrets.AZURE_CLIENT_ID }}
    tenant-id: ${{ secrets.AZURE_TENANT_ID }}
    subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
```

## 🔧 Solution Applied

### 1. Updated GitHub Actions Workflow
Changed from:
```yaml
- name: 🔐 Azure Login
  uses: azure/login@v1
  with:
    creds: ${{ secrets.AZURE_CREDENTIALS }}
```

To:
```yaml
- name: 🔐 Azure Login
  uses: azure/login@v1
  with:
    client-id: ${{ secrets.AZURE_CLIENT_ID }}
    tenant-id: ${{ secrets.AZURE_TENANT_ID }}
    subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
```

### 2. Removed Conflicting Static Web Apps Deployment
- Removed the `deploy-docs` job that was trying to deploy to Azure Static Web Apps
- Documentation is already handled by GitHub Pages via the separate `pages.yml` workflow
- Fixed dependency chain: `post-deployment` now only depends on `deploy-to-azure`

### 3. Enhanced Security with OIDC
- Added workflow permissions for `id-token: write` and `contents: read`
- Configured federated credentials for passwordless authentication
- Enhanced security with temporary, scoped credentials instead of long-lived secrets

### 4. Required GitHub Secrets

You need to add these three secrets to your GitHub repository:

| Secret Name | Description | How to Get |
|-------------|-------------|------------|
| `AZURE_CLIENT_ID` | Service Principal Client ID | From `az ad sp create-for-rbac` output |
| `AZURE_TENANT_ID` | Azure AD Tenant ID | From `az ad sp create-for-rbac` output |
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID | From `az account show --query id` |

### 3. Automated Setup Script

Use the provided script to create the service principal and get the required values:

```bash
./scripts/setup-github-azure-secrets.sh
```

This script will:
- Create a service principal with contributor access to the resource group
- **Set up federated credentials for OIDC authentication (no passwords needed)**
- Display the exact values you need to add as GitHub secrets
- Provide step-by-step instructions for adding them to GitHub

**Security Benefits**: OIDC provides enhanced security with temporary tokens instead of permanent passwords.

## 🎯 Educational Context

**Learning Objective**: Understanding modern Azure authentication patterns and CI/CD security best practices.

**Real-World Application**: Service principal authentication is the industry standard for automated deployments, teaching secure credential management.

**Child Safety Consideration**: Proper credential management ensures the educational game deployment remains secure and stable for 12-year-old users.

## 🆕 OIDC Subject Claim Fix (August 5, 2025)

**New Issue Identified**: Federated identity credential subject claim mismatch
```
Error: No matching federated identity record found for presented assertion subject 
'repo:victorsaly/WorldLeadersGame:environment:production'
```

**Root Cause**: The GitHub workflow uses `environment: production` which creates a different subject claim than expected.

**Subject Claim Formats**:
- ❌ **Old (Main Branch)**: `repo:victorsaly/WorldLeadersGame:ref:refs/heads/main`
- ✅ **New (Environment)**: `repo:victorsaly/WorldLeadersGame:environment:production`

**✅ Fix Applied Successfully** (August 5, 2025):
```bash
# OIDC credential fix script completed successfully
./scripts/fix-azure-oidc-credential.sh
```

**Results**:
- ✅ Found service principal: `worldleaders-github-actions` (Client ID: `6971e7b8-6812-4eff-aaaf-0db3e77d9d93`)
- ✅ Removed old federated credential: `github-main`
- ✅ Created new credential: `github-production` with correct subject claim
- ✅ Verified configuration matches workflow requirements

**New Federated Credential**:
- **Name**: `github-production`
- **Subject**: `repo:victorsaly/WorldLeadersGame:environment:production`
- **Issuer**: `https://token.actions.githubusercontent.com`
- **Audience**: `api://AzureADTokenExchange`

## 📚 Related Documentation

- [Azure Service Principal Authentication](https://docs.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals)
- [GitHub Actions Azure Login](https://github.com/Azure/login#readme)
- [GitHub Repository Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)

## ✅ Verification Status

**OIDC Configuration**: ✅ **COMPLETE** (August 5, 2025)
- Federated credential properly configured for `environment:production`
- Subject claim matches GitHub Actions workflow requirements
- Service principal authentication ready for deployment

**Next Steps**:
1. ✅ **Authentication Fixed** - OIDC credential configured correctly
2. 🔄 **Ready to Deploy** - GitHub Actions workflow should now authenticate successfully  
3. 🚀 **Test Deployment** - Trigger workflow to verify end-to-end functionality

**Expected Results** after triggering GitHub Actions:
1. ✅ Authenticate successfully with Azure using OIDC
2. ✅ Deploy the Web App to `worldleaders-web-prod`
3. ✅ Deploy the API to `worldleaders-api-prod`
4. ✅ Configure proper .NET 8 runtime settings

---

**Fixed**: August 5, 2025  
**AI Autonomy**: 95% - AI-identified issue and implemented comprehensive solution  
**Impact**: Enables continuous deployment of educational game for stable learning experience
