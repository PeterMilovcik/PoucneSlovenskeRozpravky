using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Catalog;

public static class StatusTracker
{
    private static readonly Dictionary<StoryStatus, string> Descriptions = new()
    {
        [StoryStatus.OutlineDraft] = "Náčrt osnovy",
        [StoryStatus.OutlineReady] = "Osnova hotová",
        [StoryStatus.TextGenerating] = "Generovanie textu",
        [StoryStatus.TextDraft] = "Náčrt textu",
        [StoryStatus.GrammarChecked] = "Gramatika skontrolovaná",
        [StoryStatus.StyleChecked] = "Štýl skontrolovaný",
        [StoryStatus.ContentReviewed] = "Obsah skontrolovaný",
        [StoryStatus.TextReady] = "Text hotový",
        [StoryStatus.AudioReady] = "Audio hotové",
        [StoryStatus.ImagesReady] = "Obrázky hotové",
        [StoryStatus.VideoReady] = "Video hotové",
        [StoryStatus.PublishedText] = "Text publikovaný",
        [StoryStatus.PublishedAudio] = "Audio publikované",
        [StoryStatus.PublishedVideo] = "Video publikované",
        [StoryStatus.FullyPublished] = "Plne publikované",
    };

    /// <summary>
    /// Validates a status transition. Allows skipping forward but never going backwards.
    /// </summary>
    public static bool ValidateTransition(StoryStatus currentStatus, StoryStatus newStatus)
    {
        return (int)newStatus > (int)currentStatus;
    }

    /// <summary>
    /// Returns the next expected status in the lifecycle, or null if already at the final status.
    /// </summary>
    public static StoryStatus? GetNextStatus(StoryStatus current)
    {
        if (current == StoryStatus.FullyPublished)
            return null;

        return (StoryStatus)((int)current + 1);
    }

    /// <summary>
    /// Returns a Slovak description of the given status.
    /// </summary>
    public static string GetStatusDescription(StoryStatus status)
    {
        return Descriptions.TryGetValue(status, out var description)
            ? description
            : status.ToString();
    }
}
