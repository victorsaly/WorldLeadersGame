using WorldLeaders.Shared.DTOs;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Service for speech recognition and pronunciation assessment for child language learners
/// Educational focus: Safe, encouraging pronunciation practice for 12-year-old players
/// Safety priority: COPPA-compliant processing with no audio storage
/// </summary>
public interface ISpeechRecognitionService
{
    /// <summary>
    /// Assess pronunciation quality using Azure Speech Services
    /// Processes audio in real-time without storage for child safety
    /// </summary>
    /// <param name="audioData">Audio data as byte array for real-time processing</param>
    /// <param name="targetText">Text the child is trying to pronounce</param>
    /// <param name="languageCode">Language code for pronunciation assessment</param>
    /// <returns>Child-friendly pronunciation assessment with encouragement</returns>
    Task<PronunciationAssessmentResult> AssessPronunciationAsync(byte[] audioData, string targetText, string languageCode);

    /// <summary>
    /// Convert speech to text for pronunciation comparison
    /// Real-time processing only, no audio storage
    /// </summary>
    /// <param name="audioData">Audio data for speech recognition</param>
    /// <param name="languageCode">Language code for recognition</param>
    /// <returns>Recognized text from speech</returns>
    Task<GameSpeechRecognitionResult> RecognizeSpeechAsync(byte[] audioData, string languageCode);

    /// <summary>
    /// Check if speech recognition is available for the given language
    /// Helps provide appropriate fallbacks for children
    /// </summary>
    /// <param name="languageCode">Language code to check support for</param>
    /// <returns>True if language is supported for speech recognition</returns>
    Task<bool> IsLanguageSupportedAsync(string languageCode);

    /// <summary>
    /// Get child-friendly feedback based on pronunciation score
    /// Always positive and encouraging for young learners
    /// </summary>
    /// <param name="score">Pronunciation accuracy score (0-100)</param>
    /// <param name="targetText">Text being practiced</param>
    /// <param name="languageName">Name of language being learned</param>
    /// <returns>Encouraging, educational feedback message</returns>
    Task<string> GetChildFriendlyFeedbackAsync(double score, string targetText, string languageName);

    /// <summary>
    /// Test microphone and speech service connectivity
    /// Helps ensure proper setup for children's learning sessions
    /// </summary>
    /// <returns>Service health status for user feedback</returns>
    Task<SpeechServiceHealthStatus> CheckServiceHealthAsync();
}

/// <summary>
/// Result of pronunciation assessment with child-friendly feedback
/// </summary>
public record PronunciationAssessmentResult(
    double AccuracyScore,
    double FluencyScore,
    double CompletenessScore,
    double OverallScore,
    string RecognizedText,
    bool Passed,
    string ChildFriendlyFeedback,
    List<WordPronunciationScore> WordScores,
    DateTime AssessedAt
);

/// <summary>
/// Individual word pronunciation scoring for detailed feedback
/// </summary>
public record WordPronunciationScore(
    string Word,
    double AccuracyScore,
    string Feedback
);

/// <summary>
/// Speech recognition result with confidence scoring
/// </summary>
public record GameSpeechRecognitionResult(
    string RecognizedText,
    double ConfidenceScore,
    bool IsSuccessful,
    string ErrorMessage,
    DateTime RecognizedAt
);

/// <summary>
/// Health status of speech recognition service
/// </summary>
public record SpeechServiceHealthStatus(
    bool IsAvailable,
    string StatusMessage,
    List<string> SupportedLanguages,
    DateTime CheckedAt
);