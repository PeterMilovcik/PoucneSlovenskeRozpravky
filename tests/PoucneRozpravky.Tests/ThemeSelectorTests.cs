using System.Text.Json;
using NSubstitute;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;
using PoucneRozpravky.Preparation;

namespace PoucneRozpravky.Tests;

public class ThemeSelectorTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _themesPath;
    private readonly ICatalogManager _catalog;

    public ThemeSelectorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"theme-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _themesPath = Path.Combine(_tempDir, "themes.json");
        _catalog = Substitute.For<ICatalogManager>();

        var themes = new ThemeCollection
        {
            Themes =
            [
                new ThemeEntry { Id = "1", Category = "hodnoty", Title = "Priateľstvo", Description = "O priateľstve", ExampleMoral = "Priatelia sú dôležití." },
                new ThemeEntry { Id = "2", Category = "hodnoty", Title = "Odvaha", Description = "O odvahe", ExampleMoral = "Odvaha pomáha prekonať strach." },
                new ThemeEntry { Id = "3", Category = "príroda", Title = "Les", Description = "O lese", ExampleMoral = "Prírodu treba chrániť." }
            ]
        };

        File.WriteAllText(_themesPath, JsonSerializer.Serialize(themes));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public async Task SelectUniqueTheme_ReturnsThemeNotAlreadyUsed()
    {
        _catalog.GetUsedThemesAsync().Returns(["Priateľstvo"]);
        _catalog.GetUsedMoralsAsync().Returns(["Priatelia sú dôležití."]);
        var selector = new ThemeSelector(_catalog, _themesPath);

        var (theme, _) = await selector.SelectUniqueThemeAsync();

        Assert.NotEqual("Priateľstvo", theme);
    }

    [Fact]
    public async Task SelectUniqueTheme_NoUsedThemes_ReturnsAnyTheme()
    {
        _catalog.GetUsedThemesAsync().Returns(new List<string>());
        _catalog.GetUsedMoralsAsync().Returns(new List<string>());
        var selector = new ThemeSelector(_catalog, _themesPath);

        var (theme, moral) = await selector.SelectUniqueThemeAsync();

        Assert.False(string.IsNullOrWhiteSpace(theme));
        Assert.False(string.IsNullOrWhiteSpace(moral));
    }

    [Fact]
    public async Task IsUnique_DuplicateThemeAndMoral_ReturnsFalse()
    {
        _catalog.GetAllStoriesAsync().Returns(
        [
            new StoryMetadata
            {
                Id = "existing",
                Title = "Test",
                Theme = "Priateľstvo",
                Moral = "Priatelia sú dôležití."
            }
        ]);
        var selector = new ThemeSelector(_catalog, _themesPath);

        var isUnique = await selector.IsUniqueAsync("Priateľstvo", "Priatelia sú dôležití.");

        Assert.False(isUnique);
    }

    [Fact]
    public async Task IsUnique_DifferentTheme_ReturnsTrue()
    {
        _catalog.GetAllStoriesAsync().Returns(
        [
            new StoryMetadata
            {
                Id = "existing",
                Title = "Test",
                Theme = "Priateľstvo",
                Moral = "Priatelia sú dôležití."
            }
        ]);
        var selector = new ThemeSelector(_catalog, _themesPath);

        var isUnique = await selector.IsUniqueAsync("Odvaha", "Odvaha pomáha.");

        Assert.True(isUnique);
    }
}
