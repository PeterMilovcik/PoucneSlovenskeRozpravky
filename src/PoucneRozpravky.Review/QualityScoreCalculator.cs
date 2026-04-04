using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Review;

public static class QualityScoreCalculator
{
    public static int CalculateGrammarScore(GrammarCheckResult result)
    {
        var score = 20 - (result.LanguageToolErrors * 2);
        return Math.Clamp(score, 0, 20);
    }

    public static int CalculateStyleScore(StyleAnalysisResult result)
    {
        var score = 20;

        // Average sentence length: ideal 8-15 words for children
        if (result.AverageSentenceLength > 20)
            score -= 4;
        else if (result.AverageSentenceLength > 15)
            score -= 2;

        // Max sentence length: penalize very long sentences
        if (result.MaxSentenceLength > 35)
            score -= 4;
        else if (result.MaxSentenceLength > 25)
            score -= 2;

        // Flesch-Kincaid grade: ideal ≤ 4 for children
        if (result.FleschKincaidGrade > 8)
            score -= 4;
        else if (result.FleschKincaidGrade > 4)
            score -= 2;

        // Simple word percentage: higher is better for children (target ≥ 80%)
        if (result.SimpleWordPercentage < 60)
            score -= 4;
        else if (result.SimpleWordPercentage < 80)
            score -= 2;

        // Passive voice: lower is better (target ≤ 10%)
        if (result.PassiveVoicePercentage > 20)
            score -= 4;
        else if (result.PassiveVoicePercentage > 10)
            score -= 2;

        return Math.Clamp(score, 0, 20);
    }

    public static int CalculateConsistencyScore(ContentReviewResult result)
    {
        return GetCategoryScore(result, "LogicalConsistency");
    }

    public static int CalculateAppropriatenessScore(ContentReviewResult result)
    {
        return GetCategoryScore(result, "AgeAppropriateness");
    }

    public static int CalculateEducationalScore(ContentReviewResult result)
    {
        return GetCategoryScore(result, "EducationalValue");
    }

    private static int GetCategoryScore(ContentReviewResult result, string category)
    {
        if (!result.CategoryResults.TryGetValue(category, out var status))
            return 0;

        return status switch
        {
            "pass" => 20,
            "warn" => 10,
            "fail" => 0,
            _ => 0
        };
    }
}
