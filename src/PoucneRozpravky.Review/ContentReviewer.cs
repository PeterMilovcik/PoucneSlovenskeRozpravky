using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Review;

public class ContentReviewer : IContentReviewer
{
    private readonly ContentReviewOptions _options;

    public ContentReviewer(ContentReviewOptions options)
    {
        _options = options;
    }

    public Task<ContentReviewResult> ReviewContentAsync(
        string text, StoryOutline outline, CancellationToken ct = default)
    {
        var categoryResults = new Dictionary<string, string>();
        var comments = new List<string>();

        CheckLogicalConsistency(text, outline, categoryResults, comments);
        CheckAgeAppropriateness(text, categoryResults, comments);
        CheckEducationalValue(text, outline, categoryResults, comments);
        CheckNarrativeQuality(text, categoryResults, comments);
        CheckEmotionalSafety(text, categoryResults, comments);

        return Task.FromResult(new ContentReviewResult(categoryResults, comments));
    }

    private void CheckLogicalConsistency(
        string text, StoryOutline outline,
        Dictionary<string, string> results, List<string> comments)
    {
        var textLower = text.ToLowerInvariant();
        var missingCharacters = new List<string>();

        foreach (var character in outline.Characters)
        {
            var nameLower = character.Name.ToLowerInvariant();
            var mentions = CountOccurrences(textLower, nameLower);

            if (mentions == 0)
                missingCharacters.Add(character.Name);
            else if (mentions < _options.MinCharacterMentions)
                comments.Add($"Postava '{character.Name}' sa spomína iba {mentions}×, minimum je {_options.MinCharacterMentions}.");
        }

        var moralPresent = !string.IsNullOrWhiteSpace(outline.Moral)
            && textLower.Contains(outline.Moral.ToLowerInvariant());

        if (missingCharacters.Count > 0)
        {
            results["LogicalConsistency"] = "fail";
            comments.Add($"V texte chýbajú postavy: {string.Join(", ", missingCharacters)}.");
        }
        else if (!moralPresent)
        {
            results["LogicalConsistency"] = "warn";
            comments.Add("Morálne ponaučenie z osnovy sa nenachádza doslovne v texte.");
        }
        else
        {
            results["LogicalConsistency"] = "pass";
        }
    }

    private void CheckAgeAppropriateness(
        string text,
        Dictionary<string, string> results, List<string> comments)
    {
        var textLower = text.ToLowerInvariant();
        var foundForbidden = _options.ForbiddenWords
            .Where(w => textLower.Contains(w.ToLowerInvariant()))
            .ToList();

        if (foundForbidden.Count > 0)
        {
            results["AgeAppropriateness"] = "fail";
            comments.Add($"Nájdené nevhodné slová: {string.Join(", ", foundForbidden)}.");
        }
        else
        {
            results["AgeAppropriateness"] = "pass";
        }
    }

    private void CheckEducationalValue(
        string text, StoryOutline outline,
        Dictionary<string, string> results, List<string> comments)
    {
        var textLower = text.ToLowerInvariant();
        var lines = text.Split('\n');
        var lastQuarter = string.Join('\n',
            lines.Skip((int)(lines.Length * 0.75))).ToLowerInvariant();

        var foundElements = _options.RequiredElements
            .Where(e => textLower.Contains(e.ToLowerInvariant()))
            .ToList();

        var moralInEnding = !string.IsNullOrWhiteSpace(outline.Moral)
            && lastQuarter.Contains(outline.Moral.ToLowerInvariant());

        var hasPoučenieSection = _options.RequiredElements
            .Any(e => lastQuarter.Contains(e.ToLowerInvariant()));

        if (foundElements.Count == 0 && !moralInEnding)
        {
            results["EducationalValue"] = "fail";
            comments.Add("V texte chýba poučenie alebo morálny záver.");
        }
        else if (!hasPoučenieSection)
        {
            results["EducationalValue"] = "warn";
            comments.Add("Poučenie sa nachádza v texte, ale nie na konci príbehu.");
        }
        else
        {
            results["EducationalValue"] = "pass";
        }
    }

    private static void CheckNarrativeQuality(
        string text,
        Dictionary<string, string> results, List<string> comments)
    {
        var textLower = text.ToLowerInvariant();

        string[] beginningMarkers = ["bol raz", "bola raz", "kedysi dávno", "za siedmimi horami", "v jednom kráľovstve"];
        string[] endingMarkers = ["koniec", "a žili šťastne", "a tak sa", "poučenie", "a od tej doby"];

        var hasBeginning = beginningMarkers.Any(m => textLower.Contains(m));
        var hasEnding = endingMarkers.Any(m => textLower.Contains(m));

        if (!hasBeginning && !hasEnding)
        {
            results["NarrativeQuality"] = "fail";
            comments.Add("Príbehu chýba tradičný rozprávkový úvod aj záver.");
        }
        else if (!hasBeginning || !hasEnding)
        {
            results["NarrativeQuality"] = "warn";
            var missing = !hasBeginning ? "úvod" : "záver";
            comments.Add($"Príbehu chýba tradičný rozprávkový {missing}.");
        }
        else
        {
            results["NarrativeQuality"] = "pass";
        }
    }

    private void CheckEmotionalSafety(
        string text,
        Dictionary<string, string> results, List<string> comments)
    {
        var textLower = text.ToLowerInvariant();
        var foundFearWords = _options.FearWords
            .Where(w => textLower.Contains(w.ToLowerInvariant()))
            .ToList();

        if (foundFearWords.Count >= 3)
        {
            results["EmotionalSafety"] = "fail";
            comments.Add($"Príliš veľa slov vyvolávajúcich strach ({foundFearWords.Count}): {string.Join(", ", foundFearWords)}.");
        }
        else if (foundFearWords.Count > 0)
        {
            results["EmotionalSafety"] = "warn";
            comments.Add($"Nájdené slová vyvolávajúce strach: {string.Join(", ", foundFearWords)}.");
        }
        else
        {
            results["EmotionalSafety"] = "pass";
        }
    }

    private static int CountOccurrences(string text, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return 0;

        var count = 0;
        var index = 0;

        while ((index = text.IndexOf(pattern, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += pattern.Length;
        }

        return count;
    }
}
