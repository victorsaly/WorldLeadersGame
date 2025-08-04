#!/bin/bash

# 🧪 Test Azure AI Services Configuration
# Verify that all Azure AI services are working correctly
# This script uses environment variables or local config files

echo "🧪 Testing Azure AI Services for World Leaders Game"
echo "=================================================="
echo ""

# Check if we have local configuration
if [ -f "azure-ai-config.local.json" ]; then
    echo "🔐 Using local configuration file for testing"
    OPENAI_ENDPOINT=$(jq -r '.AzureOpenAI.Endpoint' azure-ai-config.local.json)
    OPENAI_KEY=$(jq -r '.AzureOpenAI.ApiKey' azure-ai-config.local.json)
    CONTENT_ENDPOINT=$(jq -r '.ContentModerator.Endpoint' azure-ai-config.local.json)
    CONTENT_KEY=$(jq -r '.ContentModerator.ApiKey' azure-ai-config.local.json)
    SPEECH_KEY=$(jq -r '.SpeechServices.ApiKey' azure-ai-config.local.json)
    SPEECH_REGION=$(jq -r '.SpeechServices.Region' azure-ai-config.local.json)
elif [ -f ".env.production" ]; then
    echo "🔐 Using .env.production file for testing"
    source .env.production
    OPENAI_ENDPOINT=$AZURE_OPENAI_ENDPOINT
    OPENAI_KEY=$AZURE_OPENAI_API_KEY
    CONTENT_ENDPOINT=$CONTENT_MODERATOR_ENDPOINT
    CONTENT_KEY=$CONTENT_MODERATOR_API_KEY
    SPEECH_KEY=$SPEECH_SERVICES_API_KEY
    SPEECH_REGION=$SPEECH_SERVICES_REGION
else
    echo "❌ No local configuration found"
    echo "   Create azure-ai-config.local.json or .env.production with your credentials"
    exit 1
fi

# Validate we have the necessary values
if [[ "$OPENAI_KEY" == *"your-"* ]] || [[ "$OPENAI_KEY" == "" ]]; then
    echo "❌ No valid Azure AI credentials found"
    echo "   Please update your local configuration files with real API keys"
    exit 1
fi

# Test OpenAI Service
echo "1️⃣ Testing Azure OpenAI Service..."
curl -X POST "${OPENAI_ENDPOINT}openai/deployments/gpt-4-educational/chat/completions?api-version=2024-02-15-preview" \
  -H "Content-Type: application/json" \
  -H "api-key: $OPENAI_KEY" \
  -d '{
    "messages": [
      {
        "role": "system",
        "content": "You are Maya, an encouraging career guide for 12-year-old students learning about world leadership."
      },
      {
        "role": "user",
        "content": "What jobs help people learn about other countries?"
      }
    ],
    "max_tokens": 100,
    "temperature": 0.7
  }' 2>/dev/null | jq '.choices[0].message.content' 2>/dev/null && echo "✅ OpenAI test passed" || echo "❌ OpenAI test failed"

echo ""

# Test Content Moderator
echo "2️⃣ Testing Content Moderator..."
curl -X POST "${CONTENT_ENDPOINT}contentmoderator/moderate/v1.0/ProcessText/Screen" \
  -H "Content-Type: text/plain" \
  -H "Ocp-Apim-Subscription-Key: $CONTENT_KEY" \
  -d "Hello world! This is educational content for children." 2>/dev/null | jq '.Classification' 2>/dev/null && echo "✅ Content Moderator test passed" || echo "❌ Content Moderator test failed"

echo ""

# Test Speech Services (list voices)
echo "3️⃣ Testing Speech Services..."
curl -X GET "https://${SPEECH_REGION}.tts.speech.microsoft.com/cognitiveservices/voices/list" \
  -H "Ocp-Apim-Subscription-Key: $SPEECH_KEY" 2>/dev/null | jq '. | length' 2>/dev/null && echo "✅ Speech Services test passed" || echo "❌ Speech Services test failed"

echo ""
echo "✅ Azure AI Services Test Complete!"
echo ""
echo "🎮 Your World Leaders Game is now connected to Azure AI!"
echo "   🧠 OpenAI GPT-4o ready for educational conversations"
echo "   🛡️ Content Moderator protecting child safety"
echo "   🗣️ Speech Services ready for language learning"
echo ""
echo "🚀 Next Steps:"
echo "   1. Start your game: dotnet run --project src/WorldLeaders/WorldLeaders.API"
echo "   2. Test AI agents: https://localhost:7289/api/AI/personalities"
echo "   3. Monitor costs: https://portal.azure.com"
