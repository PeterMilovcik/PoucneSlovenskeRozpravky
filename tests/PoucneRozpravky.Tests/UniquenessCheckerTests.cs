using NSubstitute;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;
using PoucneRozpravky.Preparation;

namespace PoucneRozpravky.Tests;

public class UniquenessCheckerTests
{
    private readonly ICatalogManager _catalog;
    private readonly UniquenessChecker _checker;

    public UniquenessCheckerTests()
    {
        _catalog = Substitute.For<ICatalogManager>();
        _checker = new UniquenessChecker(_catalog);
    }

    private static StoryOutline CreateOutline(string title, params string[] characterNames)
    {
        var characters = characterNames.Select(n => new StoryCharacter(n, "popis", "hrdina")).ToList();
        return new StoryOutline
        {
            Title = title,
            Theme = "priateľstvo",
            Moral = "dobro víťazí",
            Setting = "les",
            Characters = characters,
            TargetMinutes = 10,
            TargetWordCount = 1400
        };
    }

    [Fact]
    public async Task IsOutlineUnique_IdenticalTitle_ReturnsFalse()
    {
        _catalog.GetAllStoriesAsync().Returns(
        [
            new StoryMetadata
            {
                Id = "existing",
                Title = "Janko a Marienka",
                Theme = "odvaha",
                Moral = "pomoc druhým"
            }
        ]);

        var outline = CreateOutline("Janko a Marienka", "Janko");

        Assert.False(await _checker.IsOutlineUniqueAsync(outline));
    }

    [Fact]
    public async Task IsOutlineUnique_IdenticalTitleCaseInsensitive_ReturnsFalse()
    {
        _catalog.GetAllStoriesAsync().Returns(
        [
            new StoryMetadata
            {
                Id = "existing",
                Title = "janko a marienka",
                Theme = "odvaha",
                Moral = "pomoc"
            }
        ]);

        var outline = CreateOutline("Janko A Marienka", "Janko");

        Assert.False(await _checker.IsOutlineUniqueAsync(outline));
    }

    [Fact]
    public async Task IsOutlineUnique_Over50PercentCharacterOverlap_ReturnsFalse()
    {
        // Story title contains character names used in outline — the overlap heuristic
        // compares outline character names against title words
        _catalog.GetAllStoriesAsync().Returns(
        [
            new StoryMetadata
            {
                Id = "existing",
                Title = "Janko Marienka",
                Theme = "odvaha",
                Moral = "pomoc"
            }
        ]);

        // Both outline characters appear in existing title words
        var outline = CreateOutline("Nový príbeh", "Janko", "Marienka");

        Assert.False(await _checker.IsOutlineUniqueAsync(outline));
    }

    [Fact]
    public async Task IsOutlineUnique_CompletelyDifferent_ReturnsTrue()
    {
        _catalog.GetAllStoriesAsync().Returns(
        [
            new StoryMetadata
            {
                Id = "existing",
                Title = "Janko a drak",
                Theme = "odvaha",
                Moral = "pomoc"
            }
        ]);

        var outline = CreateOutline("Líška a medveď", "Líška", "Medveď");

        Assert.True(await _checker.IsOutlineUniqueAsync(outline));
    }

    [Fact]
    public async Task IsOutlineUnique_NoCatalogStories_ReturnsTrue()
    {
        _catalog.GetAllStoriesAsync().Returns(new List<StoryMetadata>());

        var outline = CreateOutline("Akýkoľvek príbeh", "Hrdina");

        Assert.True(await _checker.IsOutlineUniqueAsync(outline));
    }
}
