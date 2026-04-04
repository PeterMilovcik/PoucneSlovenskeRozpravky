using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IChapterGenerator
{
    Task<string> GenerateChapterAsync(StoryOutline outline, SceneOutline scene, string? previousChapterEnding, CancellationToken ct = default);
    Task<string> MergeChaptersAsync(StoryOutline outline, List<string> chapters, CancellationToken ct = default);
}
