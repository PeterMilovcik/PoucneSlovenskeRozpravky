namespace PoucneRozpravky.Core.Models;

public enum IssueSeverity
{
    Info,
    Warning,
    Error
}

public record QualityIssue(string Category, IssueSeverity Severity, string Description, string? Location);

public class QualityScore
{
    public int Grammar { get; set; }
    public int Style { get; set; }
    public int LogicalConsistency { get; set; }
    public int AgeAppropriateness { get; set; }
    public int EducationalValue { get; set; }
    public int Total => Grammar + Style + LogicalConsistency + AgeAppropriateness + EducationalValue;
    public bool IsPassing => Total >= 80
        && Grammar >= 12
        && Style >= 12
        && LogicalConsistency >= 12
        && AgeAppropriateness >= 12
        && EducationalValue >= 12;
    public List<QualityIssue> Issues { get; set; } = [];
}
