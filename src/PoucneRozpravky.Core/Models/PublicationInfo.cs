namespace PoucneRozpravky.Core.Models;

public class PublicationInfo
{
    /// <summary>"blog", "spotify", "youtube"</summary>
    public required string Platform { get; set; }
    public required string Url { get; set; }
    public DateTimeOffset PublishedAt { get; set; }
    /// <summary>Platform-specific metadata.</summary>
    public Dictionary<string, string> Metadata { get; set; } = [];
}
