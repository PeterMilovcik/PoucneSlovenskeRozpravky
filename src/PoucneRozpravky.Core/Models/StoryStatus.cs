namespace PoucneRozpravky.Core.Models;

public enum StoryStatus
{
    OutlineDraft,
    OutlineReady,
    TextGenerating,
    TextDraft,
    GrammarChecked,
    StyleChecked,
    ContentReviewed,
    TextReady,
    AudioReady,
    ImagesReady,
    VideoReady,
    PublishedText,
    PublishedAudio,
    PublishedVideo,
    FullyPublished
}
