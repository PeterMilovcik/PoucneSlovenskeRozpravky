namespace PoucneRozpravky.Core.Models;

public record GrammarCheckResult(
    int LanguageToolErrors,
    int AutoCorrected,
    List<string> ManualReviewNeeded);

public record StyleAnalysisResult(
    double AverageSentenceLength,
    int MaxSentenceLength,
    double FleschKincaidGrade,
    double SimpleWordPercentage,
    double PassiveVoicePercentage,
    double TypeTokenRatio);

public record ContentReviewResult(
    Dictionary<string, string> CategoryResults,
    List<string> Comments);

public class QualityReport
{
    public required string StoryId { get; set; }
    public required QualityScore Score { get; set; }
    public required GrammarCheckResult GrammarDetails { get; set; }
    public required StyleAnalysisResult StyleDetails { get; set; }
    public required ContentReviewResult ContentDetails { get; set; }
    public DateTimeOffset ReviewedAt { get; set; }
    public string ReviewedBy { get; set; } = "LanguageTool+StyleAnalyzer+Copilot";
}
