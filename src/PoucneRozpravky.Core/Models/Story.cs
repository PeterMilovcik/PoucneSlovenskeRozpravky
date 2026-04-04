namespace PoucneRozpravky.Core.Models;

public class Story
{
    /// <summary>Format: "YYYY-MM-DD-slug"</summary>
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required StoryOutline Outline { get; set; }
    /// <summary>Full Markdown text of the story.</summary>
    public required string Text { get; set; }
    public int WordCount { get; set; }
    public double EstimatedMinutes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    /// <summary>Relative path to story directory.</summary>
    public required string DirectoryPath { get; set; }
}
