using System.Text.Json;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Publisher;

public class SpotifyPublisher(PublisherOptions options) : IPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task<PublicationInfo> PublishAsync(
        StoryMetadata story,
        string platform,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(story);

        if (story.Audio is null || !File.Exists(story.Audio.File))
            throw new InvalidOperationException($"Audio file not found for story '{story.Id}'. Generate audio before publishing to Spotify.");

        string outputDir = Path.Combine(options.OutputBaseDirectory, "spotify", story.Id);
        Directory.CreateDirectory(outputDir);

        var episodeMetadata = new
        {
            title = story.Title,
            description = $"Poučná slovenská rozprávka: {story.Title}. " +
                          $"Téma: {story.Theme}. Ponaučenie: {story.Moral}.",
            language = options.PodcastLanguage,
            category = options.PodcastCategory,
            podcastTitle = options.PodcastTitle,
            audioFile = Path.GetFullPath(story.Audio.File),
            audioDurationSeconds = story.Audio.DurationSeconds,
            coverArt = story.Images?.Cover is not null ? Path.GetFullPath(story.Images.Cover) : null,
            publishDate = DateTimeOffset.UtcNow.ToString("o"),
        };

        string metadataPath = Path.Combine(outputDir, "episode-metadata.json");
        string json = JsonSerializer.Serialize(episodeMetadata, JsonOptions);
        await File.WriteAllTextAsync(metadataPath, json, ct);

        Console.WriteLine($"[SpotifyPublisher] Episode metadata saved to: {metadataPath}");
        Console.WriteLine($"[SpotifyPublisher] Actual Spotify publishing requires RSS feed setup and podcast hosting platform.");

        return new PublicationInfo
        {
            Platform = "spotify",
            Url = Path.GetFullPath(metadataPath),
            PublishedAt = DateTimeOffset.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                ["title"] = story.Title,
                ["audioFile"] = story.Audio.File,
                ["durationSeconds"] = story.Audio.DurationSeconds.ToString("F1"),
                ["status"] = "metadata_prepared",
            }
        };
    }
}
