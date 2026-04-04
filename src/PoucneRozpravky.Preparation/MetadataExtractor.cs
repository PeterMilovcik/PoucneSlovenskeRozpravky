using System.Text.RegularExpressions;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Preparation;

/// <summary>
/// Metadata extracted from a story text.
/// </summary>
public record ExtractedMetadata(
    int WordCount,
    double EstimatedMinutes,
    List<string> Characters,
    bool HasMoral);

public partial class MetadataExtractor
{
    private const double WordsPerMinute = 140.0;

    /// <summary>
    /// Extracts metadata from generated story text.
    /// </summary>
    public ExtractedMetadata ExtractFromText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        int wordCount = CountWords(text);
        double estimatedMinutes = wordCount / WordsPerMinute;
        var characters = FindCharacterNames(text);
        bool hasMoral = HasMoralSection(text);

        return new ExtractedMetadata(wordCount, estimatedMinutes, characters, hasMoral);
    }

    /// <summary>
    /// Parses a structured outline Markdown file into a <see cref="StoryOutline"/>.
    /// </summary>
    public async Task<StoryOutline> ExtractFromOutlineAsync(string outlinePath, CancellationToken ct = default)
    {
        var text = await File.ReadAllTextAsync(outlinePath, ct);
        return ParseOutline(text);
    }

    internal static StoryOutline ParseOutline(string text)
    {
        string title = ExtractSection(text, "Názov") ?? ExtractFirstHeading(text) ?? "Bez názvu";
        string theme = ExtractSection(text, "Téma") ?? "";
        string moral = ExtractSection(text, "Poučenie") ?? ExtractSection(text, "Morál") ?? "";
        string setting = ExtractSection(text, "Prostredie") ?? ExtractSection(text, "Miesto") ?? "";

        var characters = ParseCharacters(text);
        var scenes = ParseScenes(text);

        int targetMinutes = ExtractNumber(text, "Dĺžka") ?? ExtractNumber(text, "Minút") ?? 12;

        return new StoryOutline
        {
            Title = title.Trim(),
            Theme = theme.Trim(),
            Moral = moral.Trim(),
            Setting = setting.Trim(),
            Characters = characters,
            Scenes = scenes,
            TargetMinutes = targetMinutes,
            TargetWordCount = targetMinutes * 140
        };
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return WordRegex().Matches(text).Count;
    }

    private static List<string> FindCharacterNames(string text)
    {
        // Match capitalized Slovak names (2+ capital-letter words that appear multiple times)
        var candidates = CapitalizedNameRegex().Matches(text)
            .Select(m => m.Value)
            .GroupBy(n => n, StringComparer.Ordinal)
            .Where(g => g.Count() >= 2) // Name must appear at least twice
            .Select(g => g.Key)
            .ToList();

        // Filter out common Slovak words that happen to be capitalized (sentence starters, etc.)
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Bol", "Bola", "Boli", "Raz", "Kde", "Keď", "Ako", "Tak", "Ale", "Potom",
            "Vtedy", "Preto", "Kapitola", "Poučenie", "Morál", "Téma", "Príbeh",
            "Rozprávka", "Ten", "Tá", "To", "Ich", "Však", "Veď", "Prečo", "Kam"
        };

        return candidates
            .Where(n => !stopWords.Contains(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool HasMoralSection(string text)
    {
        return text.Contains("poučenie", StringComparison.OrdinalIgnoreCase)
            || text.Contains("morál", StringComparison.OrdinalIgnoreCase)
            || MoralSectionRegex().IsMatch(text);
    }

    private static string? ExtractSection(string text, string sectionName)
    {
        // Match patterns like "**Téma:** value" or "## Téma\nvalue"
        var pattern = $@"(?:\*\*{sectionName}:?\*\*\s*:?\s*(.+?)(?:\n|$))|(?:^##?\s*{sectionName}\s*\n+(.+?)(?:\n\n|$))";
        var match = Regex.Match(text, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

        if (match.Success)
            return (match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value).Trim();

        return null;
    }

    private static string? ExtractFirstHeading(string text)
    {
        var match = Regex.Match(text, @"^#\s+(.+)$", RegexOptions.Multiline);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static List<StoryCharacter> ParseCharacters(string text)
    {
        var characters = new List<StoryCharacter>();

        // Match "- **Name** - description" or "- Name: description" patterns
        var matches = CharacterLineRegex().Matches(text);
        foreach (Match match in matches)
        {
            string name = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            string description = match.Groups[3].Value.Trim();
            characters.Add(new StoryCharacter(name.Trim(), description, ""));
        }

        return characters;
    }

    private static List<SceneOutline> ParseScenes(string text)
    {
        var scenes = new List<SceneOutline>();

        // Match numbered scenes/chapters: "1. Title - Description" or "### Scéna 1: Title"
        var matches = SceneLineRegex().Matches(text);
        int number = 1;
        foreach (Match match in matches)
        {
            string title = match.Groups[1].Value.Trim();
            string description = match.Groups[2].Success ? match.Groups[2].Value.Trim() : "";
            scenes.Add(new SceneOutline(number++, title, description, 0));
        }

        return scenes;
    }

    private static int? ExtractNumber(string text, string label)
    {
        var pattern = $@"\*\*{label}:?\*\*\s*:?\s*(\d+)";
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        return match.Success ? int.Parse(match.Groups[1].Value) : null;
    }

    [GeneratedRegex(@"\S+")]
    private static partial Regex WordRegex();

    [GeneratedRegex(@"\b[A-ZÁÄČĎÉÍĽĹŇÓÔŔŠŤÚÝŽ][a-záäčďéíľĺňóôŕšťúýž]{2,}(?:\s[A-ZÁÄČĎÉÍĽĹŇÓÔŔŠŤÚÝŽ][a-záäčďéíľĺňóôŕšťúýž]+)?\b")]
    private static partial Regex CapitalizedNameRegex();

    [GeneratedRegex(@"^#{1,3}\s*(?:Poučenie|Morál)", RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex MoralSectionRegex();

    [GeneratedRegex(@"^-\s+(?:\*\*(.+?)\*\*|(\w[\w\s]*?))[\s:–—-]+(.+)$", RegexOptions.Multiline)]
    private static partial Regex CharacterLineRegex();

    [GeneratedRegex(@"(?:^\d+\.\s+(.+?)(?:\s*[-–—]\s*(.+))?$|^###?\s*(?:Scéna|Kapitola)\s*\d+:?\s*(.+?)(?:\s*[-–—]\s*(.+))?$)", RegexOptions.Multiline)]
    private static partial Regex SceneLineRegex();
}
