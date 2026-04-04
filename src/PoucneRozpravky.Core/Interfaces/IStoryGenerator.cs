using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IStoryGenerator
{
    Task<Story> GenerateStoryAsync(StoryOutline outline, StoryConfig config, CancellationToken ct = default);
}
