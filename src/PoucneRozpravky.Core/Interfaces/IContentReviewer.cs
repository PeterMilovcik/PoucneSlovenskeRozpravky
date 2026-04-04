using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IContentReviewer
{
    Task<ContentReviewResult> ReviewContentAsync(string text, StoryOutline outline, CancellationToken ct = default);
}
