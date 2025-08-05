#!/bin/bash

# 🎮 World Leaders Game - Educational Cost Calculator
# Calculate Azure AI costs based on actual educational usage

echo "🎓 Educational Project Cost Calculator"
echo "====================================="
echo ""

# Get user inputs
read -p "📊 How many students will use the system? " num_students
read -p "💬 Average AI conversations per student per day? " conversations_per_student
read -p "🗣️ Minutes of speech practice per student per day? " speech_minutes_per_student

# Calculate daily totals
total_conversations=$((num_students * conversations_per_student))
total_speech_minutes=$((num_students * speech_minutes_per_student))

echo ""
echo "📈 Usage Summary:"
echo "   👥 Students: $num_students"
echo "   💬 Daily AI conversations: $total_conversations"
echo "   🗣️ Daily speech minutes: $total_speech_minutes"
echo ""

# Cost calculations (GBP rates)
# GPT-4: £0.024 per 1K input tokens, £0.048 per 1K output tokens
# Average conversation: 150 input tokens, 100 output tokens
conversation_cost_per_day=$(echo "scale=2; $total_conversations * 0.012" | bc)
conversation_cost_per_month=$(echo "scale=2; $conversation_cost_per_day * 30" | bc)

# Speech Services: £0.80 per hour
speech_hours_per_day=$(echo "scale=2; $total_speech_minutes / 60" | bc)
speech_cost_per_day=$(echo "scale=2; $speech_hours_per_day * 0.80" | bc)
speech_cost_per_month=$(echo "scale=2; $speech_cost_per_day * 30" | bc)

# Content Moderator: £0.80 per 1K transactions (2 checks per conversation)
moderation_checks_per_day=$((total_conversations * 2))
moderation_cost_per_day=$(echo "scale=2; $moderation_checks_per_day * 0.0008" | bc)
moderation_cost_per_month=$(echo "scale=2; $moderation_cost_per_day * 30" | bc)

# Apply free tier benefits
free_moderation_checks=5000
free_speech_hours=5
monthly_moderation_checks=$((moderation_checks_per_day * 30))
monthly_speech_hours=$(echo "scale=2; $speech_hours_per_day * 30" | bc)

# Adjust for free tier
if [ $monthly_moderation_checks -le $free_moderation_checks ]; then
    adjusted_moderation_cost=0.00
else
    excess_checks=$((monthly_moderation_checks - free_moderation_checks))
    adjusted_moderation_cost=$(echo "scale=2; $excess_checks * 0.0008" | bc)
fi

adjusted_speech_cost=$(echo "scale=2; if ($monthly_speech_hours > $free_speech_hours) $speech_cost_per_month - ($free_speech_hours * 0.80) else 0" | bc)

# Total costs
total_daily_cost=$(echo "scale=2; $conversation_cost_per_day + $speech_cost_per_day + $moderation_cost_per_day" | bc)
total_monthly_cost=$(echo "scale=2; $conversation_cost_per_month + $adjusted_speech_cost + $adjusted_moderation_cost" | bc)
cost_per_student_per_month=$(echo "scale=2; $total_monthly_cost / $num_students" | bc)

echo "💰 Estimated Costs:"
echo "   📅 Daily total: £$total_daily_cost"
echo "   📅 Monthly total: £$total_monthly_cost"
echo "   👤 Per student/month: £$cost_per_student_per_month"
echo ""

echo "🎁 Free Tier Benefits Applied:"
echo "   ✅ Content moderation: First 5,000 checks FREE"
echo "   ✅ Speech services: First 5 hours FREE"
echo ""

# Usage recommendations
if (( $(echo "$total_monthly_cost < 20" | bc -l) )); then
    echo "🌟 Recommendation: Perfect for small educational projects!"
    echo "   This is very affordable for educational use."
elif (( $(echo "$total_monthly_cost < 50" | bc -l) )); then
    echo "📚 Recommendation: Great for classroom settings!"
    echo "   Cost-effective for regular educational use."
elif (( $(echo "$total_monthly_cost < 100" | bc -l) )); then
    echo "🏫 Recommendation: Suitable for larger educational programs."
    echo "   Consider budget allocation for sustained use."
else
    echo "⚠️ Recommendation: Consider optimizing usage patterns."
    echo "   You might want to reduce daily interactions or use batch processing."
fi

echo ""
echo "💡 Cost Optimization Tips:"
echo "   • Start with lower daily usage to test and optimize"
echo "   • Use the free tiers fully before paying"
echo "   • Monitor usage with Azure budget alerts"
echo "   • Consider grouping students for shared sessions"
echo ""
echo "🔗 Set up budget alerts: https://portal.azure.com"
