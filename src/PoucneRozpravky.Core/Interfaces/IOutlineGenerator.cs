using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IOutlineGenerator
{
    Task<StoryOutline> GenerateOutlineAsync(StoryConfig config, CancellationToken ct = default);
    Task<List<StoryOutline>> GenerateIdeasAsync(StoryConfig config, int count = 5, CancellationToken ct = default);
}
