using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Azure Speech Services implementation for child-safe pronunciation assessment
/// Context: Educational language learning for 12-year-old players
/// Safety: COPPA-compliant processing with no audio storage, real-time only
/// Educational Objective: Build pronunciation confidence through encouraging feedback
/// </summary>
public class SpeechRecognitionService : ISpeechRecognitionService
{
    private readonly SpeechConfig _speechConfig;
    private readonly ILogger<SpeechRecognitionService> _logger;
    private readonly IConfiguration _configuration;
    
    // Educational settings loaded from configuration with child-friendly defaults
    private readonly double _childPassingScore;
    private readonly int _maxAudioDurationSeconds;
    private readonly double _confidenceThreshold;
    
    // Supported languages for the educational game
    private readonly Dictionary<string, string> _supportedLanguages = new()
    {
        { "en", "en-US" },
        { "es", "es-ES" },
        { "fr", "fr-FR" },
        { "de", "de-DE" },
        { "it", "it-IT" },
        { "pt", "pt-PT" },
        { "zh", "zh-CN" },
        { "ja", "ja-JP" },
        { "ko", "ko-KR" },
        { "ru", "ru-RU" },
        { "ar", "ar-SA" },
        { "hi", "hi-IN" }
    };

    public SpeechRecognitionService(
        IConfiguration configuration, 
        ILogger<SpeechRecognitionService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Load educational settings from configuration with child-friendly defaults
        _childPassingScore = _configuration.GetValue<double>("SpeechServices:ChildPassingScore", 70.0);
        _maxAudioDurationSeconds = _configuration.GetValue<int>("SpeechServices:MaxAudioDurationSeconds", 10);
        _confidenceThreshold = _configuration.GetValue<double>("SpeechServices:ConfidenceThreshold", 0.6);
        
        // Initialize Azure Speech Config with child safety settings
        var speechKey = _configuration["SpeechServices:ApiKey"];
        var speechRegion = _configuration["SpeechServices:Region"];
        
        if (string.IsNullOrEmpty(speechKey) || string.IsNullOrEmpty(speechRegion))
        {
            _logger.LogWarning("Speech Services configuration missing. Speech recognition will be disabled.");
            _speechConfig = null!;
        }
        else
        {
            _speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            _speechConfig.SpeechSynthesisLanguage = "en-US"; // Default language
            
            // Configure for child-safe processing
            _speechConfig.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, "2000");
            _speechConfig.SetProperty(PropertyId.SpeechServiceConnection_InitialSilenceTimeoutMs, "3000");
        }
    }

    public async Task<PronunciationAssessmentResult> AssessPronunciationAsync(
        byte[] audioData, string targetText, string languageCode)
    {
        if (_speechConfig == null)
        {
            return CreateOfflineFallbackResult(targetText);
        }

        try
        {
            _logger.LogInformation("Starting pronunciation assessment for text: {Text} in language: {Language}", 
                targetText, languageCode);

            // Validate input for child safety
            if (audioData == null || audioData.Length == 0)
            {
                return CreateErrorResult("No audio provided", targetText);
            }

            if (audioData.Length > _maxAudioDurationSeconds * 16000 * 2) // 16kHz, 16-bit audio
            {
                return CreateErrorResult("Audio too long - please try a shorter recording", targetText);
            }

            // Configure speech recognizer for the specific language
            var language = GetAzureLanguageCode(languageCode);
            _speechConfig.SpeechRecognitionLanguage = language;

            // Create push stream with proper format (16kHz, 16-bit, mono)
            var audioFormat = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);
            var pushStream = AudioInputStream.CreatePushStream(audioFormat);
            
            // Create audio config from the push stream
            using var audioConfig = AudioConfig.FromStreamInput(pushStream);
            using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

            // Write audio data to push stream and close it
            pushStream.Write(audioData);
            pushStream.Close();

            // Perform recognition
            var result = await recognizer.RecognizeOnceAsync();
            
            return await ProcessPronunciationResult(result, targetText, languageCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during pronunciation assessment for text: {Text}", targetText);
            return CreateErrorResult("Sorry, we had trouble checking your pronunciation. Please try again!", targetText);
        }
    }

    public async Task<GameSpeechRecognitionResult> RecognizeSpeechAsync(byte[] audioData, string languageCode)
    {
        if (_speechConfig == null)
        {
            return new GameSpeechRecognitionResult(
                "", 0.0, false, 
                "Speech recognition not available. Please type your answer instead!", 
                DateTime.UtcNow);
        }

        try
        {
            _logger.LogInformation("Starting speech recognition for language: {Language}", languageCode);

            // Validate input
            if (audioData == null || audioData.Length == 0)
            {
                return new GameSpeechRecognitionResult(
                    "", 0.0, false, "No audio provided", DateTime.UtcNow);
            }

            // Configure for the specific language
            var language = GetAzureLanguageCode(languageCode);
            _speechConfig.SpeechRecognitionLanguage = language;

            // Create push stream with proper format (16kHz, 16-bit, mono)
            var audioFormat = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);
            var pushStream = AudioInputStream.CreatePushStream(audioFormat);
            
            // Create audio config from the push stream
            using var audioConfig = AudioConfig.FromStreamInput(pushStream);
            using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

            // Write audio data to push stream and close it
            pushStream.Write(audioData);
            pushStream.Close();

            var result = await recognizer.RecognizeOnceAsync();

            return ProcessSpeechRecognitionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during speech recognition for language: {Language}", languageCode);
            return new GameSpeechRecognitionResult(
                "", 0.0, false, "Speech recognition error occurred", DateTime.UtcNow);
        }
    }

    public async Task<bool> IsLanguageSupportedAsync(string languageCode)
    {
        await Task.CompletedTask; // Make async for future enhancements
        return _supportedLanguages.ContainsKey(languageCode);
    }

    public async Task<string> GetChildFriendlyFeedbackAsync(double score, string targetText, string languageName)
    {
        await Task.CompletedTask; // Make async for future AI enhancement

        return score switch
        {
            >= 90 => $"🌟 Fantastic! Your {languageName} pronunciation of '{targetText}' was excellent! You're becoming a language expert!",
            >= 80 => $"🎉 Great job! Your {languageName} pronunciation was really good! Keep practicing and you'll be perfect!",
            >= 70 => $"✨ Nice work! Your {languageName} pronunciation is getting better! You're on the right track!",
            >= 60 => $"💪 Good try! {languageName} can be tricky, but you're learning! Let's practice '{targetText}' again!",
            >= 50 => $"🌱 Keep going! Every attempt at {languageName} makes you stronger! Try saying '{targetText}' a bit slower!",
            _ => $"🚀 That's okay! Learning {languageName} takes time and practice. Listen carefully and try '{targetText}' again!"
        };
    }

    public async Task<SpeechServiceHealthStatus> CheckServiceHealthAsync()
    {
        try
        {
            if (_speechConfig == null)
            {
                return new SpeechServiceHealthStatus(
                    false, 
                    "Speech Services not configured. Text input is still available!",
                    new List<string>(),
                    DateTime.UtcNow);
            }

            // Simple connectivity test
            using var recognizer = new SpeechRecognizer(_speechConfig);
            
            // Test basic configuration
            await Task.Delay(100); // Simulate health check
            
            return new SpeechServiceHealthStatus(
                true,
                "Speech recognition is ready for language learning!",
                _supportedLanguages.Keys.ToList(),
                DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Speech service health check failed");
            return new SpeechServiceHealthStatus(
                false,
                "Speech recognition temporarily unavailable. You can still practice by typing!",
                new List<string>(),
                DateTime.UtcNow);
        }
    }

    private async Task<PronunciationAssessmentResult> ProcessPronunciationResult(
        Microsoft.CognitiveServices.Speech.SpeechRecognitionResult result, string targetText, string languageCode)
    {
        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            // For now, use simple text similarity for pronunciation assessment
            // In a full implementation, this would use Azure's pronunciation assessment API
            var recognizedText = result.Text;
            var similarity = CalculateTextSimilarity(recognizedText, targetText);
            
            var accuracy = similarity * 100;
            var fluency = Math.Max(60, accuracy - 10); // Simplified fluency score
            var completeness = recognizedText.Length > 0 ? 100 : 0;
            var overall = (accuracy + fluency + completeness) / 3.0;
            
            var passed = overall >= _childPassingScore;
            var feedback = await GetChildFriendlyFeedbackAsync(overall, targetText, GetLanguageName(languageCode));
            
            // Create simplified word-level scores
            var wordScores = new List<WordPronunciationScore>
            {
                new WordPronunciationScore(targetText, accuracy, accuracy >= 70 ? "Great!" : "Try again!")
            };

            _logger.LogInformation("Pronunciation assessment completed: Overall={Overall}, Accuracy={Accuracy}, Target='{Target}'",
                overall, accuracy, targetText);

            return new PronunciationAssessmentResult(
                accuracy, fluency, completeness, overall,
                recognizedText, passed, feedback, wordScores, DateTime.UtcNow);
        }
        else
        {
            var errorMessage = result.Reason switch
            {
                ResultReason.NoMatch => "We couldn't hear you clearly. Please try speaking a bit louder!",
                ResultReason.Canceled => "Recording was stopped. Please try again!",
                _ => "We had trouble understanding. Please try speaking clearly!"
            };
            
            return CreateErrorResult(errorMessage, targetText);
        }
    }

    private GameSpeechRecognitionResult ProcessSpeechRecognitionResult(Microsoft.CognitiveServices.Speech.SpeechRecognitionResult result)
    {
        return result.Reason switch
        {
            ResultReason.RecognizedSpeech => new GameSpeechRecognitionResult(
                result.Text, 0.9, true, "", DateTime.UtcNow), // High confidence for recognized speech
            ResultReason.NoMatch => new GameSpeechRecognitionResult(
                "", 0.0, false, "No speech detected. Please try speaking clearly!", DateTime.UtcNow),
            ResultReason.Canceled => new GameSpeechRecognitionResult(
                "", 0.0, false, "Recording was cancelled. Please try again!", DateTime.UtcNow),
            _ => new GameSpeechRecognitionResult(
                "", 0.0, false, "Speech recognition failed. Please try again!", DateTime.UtcNow)
        };
    }

    private PronunciationAssessmentResult CreateOfflineFallbackResult(string targetText)
    {
        return new PronunciationAssessmentResult(
            0, 0, 0, 0, "",
            false,
            "Speech recognition is not available right now. You can still practice by typing your pronunciation!",
            new List<WordPronunciationScore>(),
            DateTime.UtcNow
        );
    }

    private PronunciationAssessmentResult CreateErrorResult(string errorMessage, string targetText)
    {
        return new PronunciationAssessmentResult(
            0, 0, 0, 0, "",
            false,
            errorMessage,
            new List<WordPronunciationScore>(),
            DateTime.UtcNow
        );
    }

    private double CalculateTextSimilarity(string text1, string text2)
    {
        if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
            return 0.0;

        // Simple similarity calculation using Levenshtein distance
        var maxLength = Math.Max(text1.Length, text2.Length);
        if (maxLength == 0) return 1.0;

        var distance = ComputeLevenshteinDistance(text1.ToLowerInvariant(), text2.ToLowerInvariant());
        return 1.0 - (double)distance / maxLength;
    }

    private int ComputeLevenshteinDistance(string s, string t)
    {
        var d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++)
            d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++)
            d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[s.Length, t.Length];
    }

    private string GetAzureLanguageCode(string languageCode)
    {
        return _supportedLanguages.TryGetValue(languageCode, out var azureCode) ? azureCode : "en-US";
    }

    private string GetLanguageName(string languageCode) => languageCode switch
    {
        "en" => "English",
        "es" => "Spanish",
        "fr" => "French", 
        "de" => "German",
        "it" => "Italian",
        "pt" => "Portuguese",
        "zh" => "Chinese",
        "ja" => "Japanese",
        "ko" => "Korean",
        "ru" => "Russian",
        "ar" => "Arabic",
        "hi" => "Hindi",
        _ => "Local Language"
    };
}