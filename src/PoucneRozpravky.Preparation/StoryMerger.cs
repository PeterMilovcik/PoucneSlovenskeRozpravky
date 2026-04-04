using System.Text;
using System.Text.RegularExpressions;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Preparation;

public class StoryMerger
{
    private const string SectionBreak = "\n\n---\n\n";

    /// <summary>
    /// Merges chapter files into a single Markdown story document.
    /// </summary>
    public async Task<string> MergeChaptersAsync(
        string outlinePath,
        IReadOnlyList<string> chapterPaths,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(chapterPaths);
        if (chapterPaths.Count == 0)
            throw new ArgumentException("At least one chapter path is required.", nameof(chapterPaths));

        var outlineText = await File.ReadAllTextAsync(outlinePath, ct);
        var title = ExtractTitle(outlineText);

        var sb = new StringBuilder();
        sb.AppendLine($"# {title}");
        sb.AppendLine();

        for (int i = 0; i < chapterPaths.Count; i++)
        {
            if (i > 0)
                sb.Append(SectionBreak);

            var chapterText = await File.ReadAllTextAsync(chapterPaths[i], ct);
            chapterText = NormalizeChapterFormatting(chapterText, i + 1);
            sb.Append(chapterText.TrimEnd());
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Validates the merged story against its outline.
    /// </summary>
    public bool ValidateMergedStory(string text, StoryOutline outline)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(outline);

        // Check all characters appear
        foreach (var character in outline.Characters)
        {
            if (!text.Contains(character.Name, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // Check word count is within ±30% of target
        int wordCount = CountWords(text);
        int target = outline.TargetWordCount > 0 ? outline.TargetWordCount : outline.TargetMinutes * 140;
        if (target > 0)
        {
            double lowerBound = target * 0.7;
            double upperBound = target * 1.3;
            if (wordCount < lowerBound || wordCount > upperBound)
                return false;
        }

        // Check moral presence
        bool hasMoral = text.Contains("poučenie", StringComparison.OrdinalIgnoreCase)
                     || text.Contains("morál", StringComparison.OrdinalIgnoreCase)
                     || text.Contains(outline.Moral, StringComparison.OrdinalIgnoreCase);
        if (!hasMoral)
            return false;

        return true;
    }

    private static string ExtractTitle(string outlineText)
    {
        // Look for first Markdown heading
        var match = Regex.Match(outlineText, @"^#\s+(.+)$", RegexOptions.Multiline);
        return match.Success ? match.Groups[1].Value.Trim() : "Rozprávka";
    }

    private static string NormalizeChapterFormatting(string chapter, int chapterNumber)
    {
        var trimmed = chapter.Trim();

        // If chapter doesn't start with a heading, add one
        if (!trimmed.StartsWith('#'))
        {
            trimmed = $"## Kapitola {chapterNumber}\n\n{trimmed}";
        }
        else
        {
            // Ensure chapter headings are ## level (not # which is reserved for the title)
            trimmed = Regex.Replace(trimmed, @"^#(?!#)", "##", RegexOptions.Multiline);
        }

        return trimmed;
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return Regex.Matches(text, @"\S+").Count;
    }
}
