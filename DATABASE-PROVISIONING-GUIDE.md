# Database Provisioning Guide - World Leaders Game

## 🎯 **Database Provisioning: Current vs Long-term Options**

You have **TWO** database approaches with different provisioning requirements:

---

## 🔄 **Current Approach: SQLite (Enhanced Azure Support)**

### ✅ **No Separate Provisioning Required**
- **Database Type**: File-based SQLite 
- **Provisioning**: ❌ **None needed** - automatically created by your application
- **Azure Setup**: ✅ **Already integrated** in your deployment pipeline
- **Permissions**: ✅ **Handled automatically** by your enhanced Azure temp directory logic

### 🚀 **How It Works in Your Deployment**

#### 1. **Automatic Database Creation**
```csharp
// Your Program.cs automatically creates SQLite database
await app.Services.EnsureDatabaseCreatedAsync();
```

#### 2. **Smart Provider Selection**
```csharp
// Your ServiceCollectionExtensions.cs automatically chooses:
// - SQLite-Temp in Azure App Service (uses D:\local\Temp)
// - SQLite in local development
// - InMemory as fallback
```

#### 3. **Zero Configuration Deployment**
Your GitHub Actions workflow deploys everything automatically:
- ✅ **No database provisioning step**
- ✅ **No connection string configuration**
- ✅ **No special permissions needed**

---

## 🐘 **Long-term Approach: PostgreSQL (Production Scale)**

### ⚠️ **Requires Separate Provisioning**
- **Database Type**: Managed Azure Database for PostgreSQL
- **Provisioning**: ✅ **Required** - separate infrastructure deployment
- **Azure Setup**: ✅ **Automated script provided**
- **Permissions**: ✅ **Azure subscription admin rights needed**

### 🔧 **How PostgreSQL Provisioning Works**

#### 1. **Infrastructure Deployment (One-Time)**
```bash
# Run this ONCE to provision PostgreSQL infrastructure
./scripts/deploy-postgresql.sh
```

This script:
- ✅ **Creates PostgreSQL Flexible Server** using Bicep template
- ✅ **Generates secure credentials** automatically
- ✅ **Configures firewall rules** for Azure services
- ✅ **Updates App Service connection strings** automatically
- ✅ **Estimates costs** (£15-25/month)

#### 2. **Required Azure Permissions**
You need these Azure RBAC roles:
- **Contributor** on the resource group (`worldleaders-prod-rg`)
- **User Access Administrator** (for service principal setup)
- **Azure CLI access** with subscription admin rights

#### 3. **Database Initialization (Automatic)**
After infrastructure is provisioned:
- ✅ **Your existing Program.cs** automatically applies migrations
- ✅ **Smart provider selection** detects PostgreSQL connection string
- ✅ **No code changes needed** - same Entity Framework code

---

## 📊 **Current Deployment Status Analysis**

### ✅ **SQLite is Already Working** (Your Current Setup)

Looking at your deployment pipeline, you have:

1. **✅ Database Provider Selection**: Automatic environment detection
2. **✅ Azure Temp Directory**: Enhanced directory detection with fallbacks
3. **✅ Permission Handling**: Comprehensive writability validation
4. **✅ Error Handling**: Graceful fallback to InMemory if needed
5. **✅ Zero Configuration**: Works out-of-the-box in Azure App Service

### 🔍 **Deployment Pipeline Integration**

Your `.github/workflows/azure-deploy.yml` currently:
- ✅ **Builds and deploys** applications automatically
- ✅ **No database provisioning** steps (because SQLite is self-contained)
- ✅ **Health checks** validate database connectivity
- ✅ **Blue-green deployment** with automated rollback

---

## 🤔 **Do You Need to Provision a Database?**

### **Short Answer: NO** (for SQLite - your current setup)
### **Short Answer: YES** (for PostgreSQL - if you want to upgrade)

### **SQLite (Current)** - No Provisioning Needed
```bash
# Current deployment - everything automatic
git push  # Triggers full deployment with working database
```

### **PostgreSQL (Upgrade)** - One-Time Provisioning
```bash
# One-time PostgreSQL infrastructure setup
./scripts/deploy-postgresql.sh

# Then normal deployments work automatically
git push  # Uses PostgreSQL instead of SQLite
```

---

## 🔐 **Permissions Required by Approach**

### **SQLite Permissions** ✅ **Already Handled**
- **Azure App Service**: ✅ Write access to temp directory (automatic)
- **GitHub Actions**: ✅ App Service deployment permissions (you already have)
- **No additional Azure permissions needed**

### **PostgreSQL Permissions** ⚠️ **Admin Rights Required**
- **Azure Subscription**: Contributor or Owner role
- **Resource Group**: Full access to `worldleaders-prod-rg`
- **Azure CLI**: Authenticated with admin account
- **GitHub Secrets**: Service principal with infrastructure deployment rights

---

## 💡 **Recommendations by Use Case**

### 🎓 **Current Phase: Development/Testing**
**Recommendation**: ✅ **Stick with SQLite** (no provisioning needed)
- Zero setup complexity
- Zero additional costs
- Already working reliably in Azure
- Perfect for validating educational content

### 🏫 **Production Phase: Real Students**
**Decision Point**: Do you need PostgreSQL?

#### **Stick with SQLite if:**
- ✅ Less than 50 concurrent students
- ✅ Cost is critical (educational budget)
- ✅ Simple maintenance preferred
- ✅ Basic analytics sufficient

#### **Upgrade to PostgreSQL if:**
- 🚀 More than 50 concurrent students expected
- 🚀 Advanced teacher analytics needed
- 🚀 Professional backup/recovery required
- 🚀 Budget allows £20-30/month

---

## 🚀 **Next Steps Based on Your Choice**

### **Option A: Continue with SQLite** (Recommended for now)
```bash
# Test your enhanced SQLite implementation
./scripts/validate-sqlite-azure.sh

# Deploy normally - no provisioning needed
git add -A
git commit -m "Enhanced SQLite Azure support"
git push
```

### **Option B: Upgrade to PostgreSQL** (Future scalability)
```bash
# Step 1: Provision PostgreSQL infrastructure (one-time)
./scripts/deploy-postgresql.sh

# Step 2: Normal deployment automatically uses PostgreSQL
git push
```

---

## 🎯 **Summary**

### **Current Status**: ✅ **Ready to Deploy**
- **SQLite**: No provisioning needed, already configured
- **Deployment**: Fully automated via GitHub Actions
- **Permissions**: No additional Azure permissions required

### **Future Option**: 🚀 **PostgreSQL Available When Needed**
- **Provisioning**: One-time script execution
- **Permissions**: Azure admin rights required
- **Cost**: £15-25/month for production features

**Bottom Line**: Your current SQLite setup requires **no additional provisioning** and is ready to deploy immediately. PostgreSQL is available as a one-command upgrade when you need production scale.

Which approach would you like to proceed with?
