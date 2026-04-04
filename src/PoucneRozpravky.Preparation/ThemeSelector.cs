using System.Text.Json;
using PoucneRozpravky.Core.Interfaces;

namespace PoucneRozpravky.Preparation;

public class ThemeSelector : IThemeSelector
{
    private readonly ICatalogManager _catalog;
    private readonly string _themesPath;
    private static readonly Random s_random = new();

    public ThemeSelector(ICatalogManager catalog, string themesPath)
    {
        _catalog = catalog;
        _themesPath = themesPath;
    }

    public async Task<(string Theme, string Moral)> SelectUniqueThemeAsync(CancellationToken ct = default)
    {
        var collection = await LoadThemesAsync(ct);
        var usedThemes = await _catalog.GetUsedThemesAsync(ct);
        var usedMorals = await _catalog.GetUsedMoralsAsync(ct);

        var available = collection.Themes
            .Where(t => !usedThemes.Contains(t.Title, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (available.Count > 0)
        {
            // Prefer entries whose example moral hasn't been used either
            var fullyUnique = available
                .Where(t => !usedMorals.Contains(t.ExampleMoral, StringComparer.OrdinalIgnoreCase))
                .ToList();

            var pool = fullyUnique.Count > 0 ? fullyUnique : available;
            var entry = pool[s_random.Next(pool.Count)];
            return (entry.Title, entry.ExampleMoral);
        }

        // All themes used — combine two themes creatively
        return CombineThemes(collection.Themes);
    }

    public async Task<bool> IsUniqueAsync(string theme, string moral, CancellationToken ct = default)
    {
        var stories = await _catalog.GetAllStoriesAsync(ct);

        return !stories.Any(s =>
            string.Equals(s.Theme, theme, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Moral, moral, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<ThemeCollection> LoadThemesAsync(CancellationToken ct)
    {
        var json = await File.ReadAllTextAsync(_themesPath, ct);
        return JsonSerializer.Deserialize<ThemeCollection>(json)
               ?? throw new InvalidOperationException($"Failed to deserialize themes from '{_themesPath}'.");
    }

    private static (string Theme, string Moral) CombineThemes(List<ThemeEntry> themes)
    {
        if (themes.Count < 2)
            throw new InvalidOperationException("Need at least two themes to combine.");

        var first = themes[s_random.Next(themes.Count)];
        ThemeEntry second;
        do
        {
            second = themes[s_random.Next(themes.Count)];
        } while (second.Id == first.Id);

        var combinedTheme = $"{first.Title} + {second.Title}";
        var combinedMoral = $"{first.ExampleMoral.TrimEnd('.')} a {second.ExampleMoral[..1].ToLowerInvariant()}{second.ExampleMoral[1..]}";

        return (combinedTheme, combinedMoral);
    }
}
