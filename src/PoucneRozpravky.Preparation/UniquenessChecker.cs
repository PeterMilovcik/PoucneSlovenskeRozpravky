using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Preparation;

public class UniquenessChecker : IUniquenessChecker
{
    private readonly ICatalogManager _catalog;

    public UniquenessChecker(ICatalogManager catalog)
    {
        _catalog = catalog;
    }

    public async Task<bool> IsOutlineUniqueAsync(StoryOutline outline, CancellationToken ct = default)
    {
        var stories = await _catalog.GetAllStoriesAsync(ct);

        foreach (var story in stories)
        {
            // Exact title match → not unique
            if (string.Equals(story.Title, outline.Title, StringComparison.OrdinalIgnoreCase))
                return false;

            // Character-name overlap > 50% → not unique
            if (HasExcessiveCharacterOverlap(outline, story))
                return false;
        }

        return true;
    }

    public async Task<string?> FindSimilarStoryAsync(StoryOutline outline, CancellationToken ct = default)
    {
        var stories = await _catalog.GetAllStoriesAsync(ct);
        string? mostSimilarId = null;
        double highestScore = 0;

        foreach (var story in stories)
        {
            double score = CalculateSimilarity(outline, story);
            if (score > highestScore)
            {
                highestScore = score;
                mostSimilarId = story.Id;
            }
        }

        // Only return if there's meaningful similarity
        return highestScore >= 0.3 ? mostSimilarId : null;
    }

    private static bool HasExcessiveCharacterOverlap(StoryOutline outline, StoryMetadata story)
    {
        if (outline.Characters.Count == 0)
            return false;

        // StoryMetadata doesn't carry character names directly, so we compare
        // against title words as a heuristic (character names often appear in titles).
        var outlineNames = outline.Characters
            .Select(c => c.Name.ToLowerInvariant())
            .ToHashSet();

        var titleWords = story.Title
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .ToHashSet();

        int overlap = outlineNames.Count(n => titleWords.Contains(n));
        double ratio = (double)overlap / outlineNames.Count;

        return ratio > 0.5;
    }

    private static double CalculateSimilarity(StoryOutline outline, StoryMetadata story)
    {
        double score = 0;

        if (string.Equals(story.Title, outline.Title, StringComparison.OrdinalIgnoreCase))
            score += 0.5;

        if (string.Equals(story.Theme, outline.Theme, StringComparison.OrdinalIgnoreCase))
            score += 0.25;

        if (string.Equals(story.Moral, outline.Moral, StringComparison.OrdinalIgnoreCase))
            score += 0.25;

        return score;
    }
}
