using System.Text.Json;
using System.Text.Json.Serialization;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Catalog;

internal class StoryCatalog
{
    public string Version { get; set; } = "1.0";
    public DateTimeOffset? LastUpdated { get; set; }
    public int TotalCount { get; set; }
    public List<StoryMetadata> Stories { get; set; } = [];
}

public class CatalogManager : ICatalogManager
{
    private readonly string _catalogPath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public CatalogManager(string? catalogPath = null)
    {
        _catalogPath = Path.GetFullPath(catalogPath ?? "../../katalog.json");
    }

    public async Task<List<StoryMetadata>> GetAllStoriesAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            return catalog.Stories;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<StoryMetadata?> GetStoryAsync(string id, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            return catalog.Stories.FirstOrDefault(s => s.Id == id);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddStoryAsync(StoryMetadata story, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            catalog.Stories.Add(story);
            catalog.TotalCount = catalog.Stories.Count;
            catalog.LastUpdated = DateTimeOffset.UtcNow;
            await SaveCatalogAsync(catalog, ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateStoryAsync(StoryMetadata story, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            var index = catalog.Stories.FindIndex(s => s.Id == story.Id);
            if (index < 0)
                throw new KeyNotFoundException($"Story '{story.Id}' not found in catalog.");

            catalog.Stories[index] = story;
            catalog.LastUpdated = DateTimeOffset.UtcNow;
            await SaveCatalogAsync(catalog, ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateStatusAsync(string id, StoryStatus newStatus, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            var story = catalog.Stories.FirstOrDefault(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Story '{id}' not found in catalog.");

            if (!StatusTracker.ValidateTransition(story.Status, newStatus))
                throw new InvalidOperationException(
                    $"Invalid status transition from {story.Status} to {newStatus}.");

            story.Status = newStatus;
            catalog.LastUpdated = DateTimeOffset.UtcNow;
            await SaveCatalogAsync(catalog, ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<List<string>> GetUsedThemesAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            return catalog.Stories
                .Select(s => s.Theme)
                .Distinct()
                .ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<List<string>> GetUsedMoralsAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var catalog = await LoadCatalogAsync(ct);
            return catalog.Stories
                .Select(s => s.Moral)
                .Distinct()
                .ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<StoryCatalog> LoadCatalogAsync(CancellationToken ct)
    {
        if (!File.Exists(_catalogPath))
            return new StoryCatalog();

        await using var stream = File.OpenRead(_catalogPath);
        return await JsonSerializer.DeserializeAsync<StoryCatalog>(stream, JsonOptions, ct)
            ?? new StoryCatalog();
    }

    private async Task SaveCatalogAsync(StoryCatalog catalog, CancellationToken ct)
    {
        var directory = Path.GetDirectoryName(_catalogPath);
        if (directory is not null)
            Directory.CreateDirectory(directory);

        var tempPath = _catalogPath + ".tmp";
        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, catalog, JsonOptions, ct);
        }

        File.Move(tempPath, _catalogPath, overwrite: true);
    }
}
