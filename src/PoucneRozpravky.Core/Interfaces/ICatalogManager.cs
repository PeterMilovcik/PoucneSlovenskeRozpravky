using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface ICatalogManager
{
    Task<List<StoryMetadata>> GetAllStoriesAsync(CancellationToken ct = default);
    Task<StoryMetadata?> GetStoryAsync(string id, CancellationToken ct = default);
    Task AddStoryAsync(StoryMetadata story, CancellationToken ct = default);
    Task UpdateStoryAsync(StoryMetadata story, CancellationToken ct = default);
    Task UpdateStatusAsync(string id, StoryStatus newStatus, CancellationToken ct = default);
    Task<List<string>> GetUsedThemesAsync(CancellationToken ct = default);
    Task<List<string>> GetUsedMoralsAsync(CancellationToken ct = default);
}
