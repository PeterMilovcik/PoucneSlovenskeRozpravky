namespace PoucneRozpravky.Core.Models;

public class StoryConfig
{
    public int TargetMinutes { get; set; } = 12;
    public int MinMinutes { get; set; } = 5;
    public int MaxMinutes { get; set; } = 30;
    public int WordsPerMinute { get; set; } = 140;
    public string? Theme { get; set; }
    public string? Moral { get; set; }
    public List<string>? Characters { get; set; }
    public string? Setting { get; set; }
    public int TargetWordCount => TargetMinutes * WordsPerMinute;
    public int ChapterThresholdMinutes { get; set; } = 15;
    public bool UseChapters => TargetMinutes > ChapterThresholdMinutes;
}
