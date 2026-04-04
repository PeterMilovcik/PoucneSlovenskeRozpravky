namespace PoucneRozpravky.Core.Models;

public record AudioInfo(string File, double DurationSeconds, string VoiceId, DateTimeOffset GeneratedAt);

public record ImagesInfo(string Cover, List<string> Scenes, DateTimeOffset GeneratedAt);

public record VideoInfo(string File, double DurationSeconds, DateTimeOffset GeneratedAt);

public class StoryMetadata
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int TargetMinutes { get; set; }
    public int WordCount { get; set; }
    public required string Theme { get; set; }
    public required string Moral { get; set; }
    public StoryStatus Status { get; set; }
    public QualityReport? QualityReport { get; set; }
    public AudioInfo? Audio { get; set; }
    public ImagesInfo? Images { get; set; }
    public VideoInfo? Video { get; set; }
    public List<PublicationInfo> Publications { get; set; } = [];
}
