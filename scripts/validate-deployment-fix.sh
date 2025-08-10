#!/bin/bash

# Validation script for Azure deployment timeout and Kudu warm-up fixes
# Tests that the fixes for issue #59 have been properly implemented

set -e

echo "🔍 Validating Azure Deployment Fixes"
echo "===================================="

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

# Test 1: Verify increased timeout in main workflow
echo "✅ Test 1: Checking main workflow timeout..."
if grep -q "timeout 900" "$REPO_ROOT/.github/workflows/azure-deploy.yml"; then
    echo "   ✅ Main workflow timeout increased to 900 seconds"
else
    echo "   ❌ Main workflow timeout not updated"
    exit 1
fi

# Test 2: Verify modern az webapp deploy command in main workflow
echo "✅ Test 2: Checking modern deployment command in main workflow..."
if grep -q "az webapp deploy" "$REPO_ROOT/.github/workflows/azure-deploy.yml"; then
    echo "   ✅ Modern az webapp deploy command found"
else
    echo "   ❌ Modern deployment command not found"
    exit 1
fi

# Test 3: Verify deprecated command removed from main workflow
echo "✅ Test 3: Checking deprecated command removal in main workflow..."
if ! grep -q "az webapp deployment source config-zip" "$REPO_ROOT/.github/workflows/azure-deploy.yml"; then
    echo "   ✅ Deprecated config-zip command removed"
else
    echo "   ❌ Deprecated command still found"
    exit 1
fi

# Test 4: Verify Kudu pre-warming in main workflow
echo "✅ Test 4: Checking Kudu pre-warming in main workflow..."
if grep -q "Pre-warming Kudu" "$REPO_ROOT/.github/workflows/azure-deploy.yml"; then
    echo "   ✅ Kudu pre-warming added"
else
    echo "   ❌ Kudu pre-warming not found"
    exit 1
fi

# Test 5: Verify fast-deploy script updates
echo "✅ Test 5: Checking fast-deploy script updates..."
if grep -q "timeout 900" "$REPO_ROOT/scripts/fast-deploy.sh" && 
   grep -q "az webapp deploy" "$REPO_ROOT/scripts/fast-deploy.sh" && 
   ! grep -q "config-zip" "$REPO_ROOT/scripts/fast-deploy.sh"; then
    echo "   ✅ Fast-deploy script properly updated"
else
    echo "   ❌ Fast-deploy script not properly updated"
    exit 1
fi

# Test 6: Verify documentation updates
echo "✅ Test 6: Checking documentation updates..."
DOC_FILES=(
    "docs/_posts/2025-08-08-zero-downtime-blue-green-deployment-educational-platforms.md"
    "docs/_posts/2025-01-15-devops-educational-platforms-bulletproof-dotnet8-deployments.md"
)

for doc_file in "${DOC_FILES[@]}"; do
    if [ -f "$REPO_ROOT/$doc_file" ]; then
        if grep -q "az webapp deploy" "$REPO_ROOT/$doc_file" && 
           ! grep -q "config-zip" "$REPO_ROOT/$doc_file"; then
            echo "   ✅ Documentation file $doc_file updated"
        else
            echo "   ❌ Documentation file $doc_file not properly updated"
            exit 1
        fi
    fi
done

# Test 7: Verify no deprecated commands remain anywhere
echo "✅ Test 7: Checking for remaining deprecated commands..."
DEPRECATED_FOUND=$(find "$REPO_ROOT" -name "*.yml" -o -name "*.yaml" -o -name "*.sh" -o -name "*.md" | grep -v "validate-deployment-fix.sh" | xargs grep -l "config-zip" 2>/dev/null || true)
if [ -z "$DEPRECATED_FOUND" ]; then
    echo "   ✅ No deprecated commands found"
else
    echo "   ❌ Deprecated commands still found in:"
    echo "$DEPRECATED_FOUND"
    exit 1
fi

# Test 8: Verify YAML syntax is still valid
echo "✅ Test 8: Checking YAML syntax..."
if python3 -c "import yaml; yaml.safe_load(open('$REPO_ROOT/.github/workflows/azure-deploy.yml', 'r'))" 2>/dev/null; then
    echo "   ✅ YAML syntax is valid"
else
    echo "   ❌ YAML syntax is invalid"
    exit 1
fi

# Test 9: Verify shell script syntax
echo "✅ Test 9: Checking shell script syntax..."
if bash -n "$REPO_ROOT/scripts/fast-deploy.sh"; then
    echo "   ✅ Shell script syntax is valid"
else
    echo "   ❌ Shell script syntax is invalid"
    exit 1
fi

echo ""
echo "🎉 All validation tests passed!"
echo "================================"
echo ""
echo "✅ Issue #59 fixes successfully implemented:"
echo "   • Deployment timeout increased from 600s to 900s"
echo "   • Deprecated az webapp deployment source config-zip replaced with az webapp deploy"
echo "   • Kudu pre-warming added to reduce deployment delays"
echo "   • All scripts and documentation updated consistently"
echo "   • No deprecated commands remain in the repository"
echo ""
echo "🚀 The deployment process should now handle longer deployment times"
echo "   and reduce Kudu warm-up failures for worldleaders-api-prod."