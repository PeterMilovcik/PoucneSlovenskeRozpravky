using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Tests;

public class CatalogManagerTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _catalogPath;
    private readonly CatalogManager _manager;

    public CatalogManagerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"catalog-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _catalogPath = Path.Combine(_tempDir, "katalog.json");
        _manager = new CatalogManager(_catalogPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private static StoryMetadata CreateStory(string id = "test-001", string theme = "priateľstvo", string moral = "dobro víťazí") =>
        new()
        {
            Id = id,
            Title = $"Rozprávka {id}",
            Theme = theme,
            Moral = moral,
            Status = StoryStatus.OutlineDraft,
            CreatedAt = DateTimeOffset.UtcNow
        };

    [Fact]
    public async Task AddStory_AndGetStory_RoundTrip()
    {
        var story = CreateStory();

        await _manager.AddStoryAsync(story);
        var retrieved = await _manager.GetStoryAsync("test-001");

        Assert.NotNull(retrieved);
        Assert.Equal("test-001", retrieved.Id);
        Assert.Equal(story.Title, retrieved.Title);
        Assert.Equal(story.Theme, retrieved.Theme);
    }

    [Fact]
    public async Task GetAllStories_ReturnsCorrectCount()
    {
        await _manager.AddStoryAsync(CreateStory("s-001"));
        await _manager.AddStoryAsync(CreateStory("s-002"));
        await _manager.AddStoryAsync(CreateStory("s-003"));

        var all = await _manager.GetAllStoriesAsync();

        Assert.Equal(3, all.Count);
    }

    [Fact]
    public async Task UpdateStatus_ChangesStatus()
    {
        var story = CreateStory();
        await _manager.AddStoryAsync(story);

        await _manager.UpdateStatusAsync("test-001", StoryStatus.OutlineReady);

        var updated = await _manager.GetStoryAsync("test-001");
        Assert.NotNull(updated);
        Assert.Equal(StoryStatus.OutlineReady, updated.Status);
    }

    [Fact]
    public async Task UpdateStatus_BackwardTransition_Throws()
    {
        var story = CreateStory();
        story.Status = StoryStatus.TextReady;
        await _manager.AddStoryAsync(story);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _manager.UpdateStatusAsync("test-001", StoryStatus.OutlineDraft));
    }

    [Fact]
    public async Task GetUsedThemes_ReturnsThemesFromExistingStories()
    {
        await _manager.AddStoryAsync(CreateStory("s-001", theme: "priateľstvo"));
        await _manager.AddStoryAsync(CreateStory("s-002", theme: "odvaha"));
        await _manager.AddStoryAsync(CreateStory("s-003", theme: "priateľstvo"));

        var themes = await _manager.GetUsedThemesAsync();

        Assert.Equal(2, themes.Count);
        Assert.Contains("priateľstvo", themes);
        Assert.Contains("odvaha", themes);
    }

    [Fact]
    public async Task GetStory_NonExistent_ReturnsNull()
    {
        var result = await _manager.GetStoryAsync("nonexistent");

        Assert.Null(result);
    }
}
