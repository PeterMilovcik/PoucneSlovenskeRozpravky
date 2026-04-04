using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Review;

public class ReviewPipeline : IReviewPipeline
{
    private readonly IGrammarChecker _grammarChecker;
    private readonly IStyleAnalyzer _styleAnalyzer;
    private readonly IContentReviewer _contentReviewer;

    public ReviewPipeline(
        IGrammarChecker grammarChecker,
        IStyleAnalyzer styleAnalyzer,
        IContentReviewer contentReviewer)
    {
        _grammarChecker = grammarChecker;
        _styleAnalyzer = styleAnalyzer;
        _contentReviewer = contentReviewer;
    }

    public async Task<QualityReport> RunReviewAsync(Story story, CancellationToken ct = default)
    {
        // 1. Grammar: check → auto-correct → re-check
        var initialGrammar = await _grammarChecker.CheckGrammarAsync(story.Text, ct);
        var correctedText = story.Text;

        if (initialGrammar.LanguageToolErrors > 0)
        {
            correctedText = await _grammarChecker.AutoCorrectAsync(story.Text, ct);
        }

        var finalGrammar = await _grammarChecker.CheckGrammarAsync(correctedText, ct);

        // 2. Style analysis on corrected text
        var styleResult = await _styleAnalyzer.AnalyzeStyleAsync(correctedText, ct);

        // 3. Content review
        var contentResult = await _contentReviewer.ReviewContentAsync(correctedText, story.Outline, ct);

        // 4. Compute quality scores
        var score = new QualityScore
        {
            Grammar = QualityScoreCalculator.CalculateGrammarScore(finalGrammar),
            Style = QualityScoreCalculator.CalculateStyleScore(styleResult),
            LogicalConsistency = QualityScoreCalculator.CalculateConsistencyScore(contentResult),
            AgeAppropriateness = QualityScoreCalculator.CalculateAppropriatenessScore(contentResult),
            EducationalValue = QualityScoreCalculator.CalculateEducationalScore(contentResult)
        };

        BuildIssues(score, finalGrammar, styleResult, contentResult);

        // 5. Build and return report
        return new QualityReport
        {
            StoryId = story.Id,
            Score = score,
            GrammarDetails = finalGrammar,
            StyleDetails = styleResult,
            ContentDetails = contentResult,
            ReviewedAt = DateTimeOffset.UtcNow
        };
    }

    private static void BuildIssues(
        QualityScore score,
        GrammarCheckResult grammar,
        StyleAnalysisResult style,
        ContentReviewResult content)
    {
        if (grammar.LanguageToolErrors > 0)
        {
            score.Issues.Add(new QualityIssue(
                "Grammar",
                grammar.LanguageToolErrors > 5 ? IssueSeverity.Error : IssueSeverity.Warning,
                $"Zostáva {grammar.LanguageToolErrors} gramatických chýb po automatickej oprave.",
                null));
        }

        foreach (var item in grammar.ManualReviewNeeded)
        {
            score.Issues.Add(new QualityIssue("Grammar", IssueSeverity.Info, item, null));
        }

        if (style.AverageSentenceLength > 15)
        {
            score.Issues.Add(new QualityIssue(
                "Style", IssueSeverity.Warning,
                $"Priemerná dĺžka vety ({style.AverageSentenceLength:F1}) presahuje odporúčaných 15 slov.",
                null));
        }

        if (style.PassiveVoicePercentage > 10)
        {
            score.Issues.Add(new QualityIssue(
                "Style", IssueSeverity.Info,
                $"Podiel trpného rodu ({style.PassiveVoicePercentage:F1}%) je nad 10%.",
                null));
        }

        foreach (var comment in content.Comments)
        {
            var severity = content.CategoryResults.Values.Any(v => v == "fail")
                ? IssueSeverity.Error
                : IssueSeverity.Warning;

            score.Issues.Add(new QualityIssue("Content", severity, comment, null));
        }
    }
}
