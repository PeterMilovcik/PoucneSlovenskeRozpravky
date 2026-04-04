using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IUniquenessChecker
{
    Task<bool> IsOutlineUniqueAsync(StoryOutline outline, CancellationToken ct = default);
    Task<string?> FindSimilarStoryAsync(StoryOutline outline, CancellationToken ct = default);
}
