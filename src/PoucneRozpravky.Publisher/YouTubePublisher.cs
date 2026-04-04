using System.Text.Json;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Publisher;

public class YouTubePublisher(PublisherOptions options) : IPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task<PublicationInfo> PublishAsync(
        StoryMetadata story,
        string platform,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(story);

        if (story.Video is null || !File.Exists(story.Video.File))
            throw new InvalidOperationException($"Video file not found for story '{story.Id}'. Generate video before publishing to YouTube.");

        string outputDir = Path.Combine(options.OutputBaseDirectory, "youtube", story.Id);
        Directory.CreateDirectory(outputDir);

        var tags = new List<string>(options.YouTubeDefaultTags) { story.Theme, story.Title };

        string description =
            $"""
            {story.Title} – Poučná slovenská rozprávka pre deti

            Téma: {story.Theme}
            Ponaučenie: {story.Moral}

            🎧 Táto rozprávka je súčasťou série „Poučné Slovenské Rozprávky" –
            krátke príbehy pre deti, ktoré učia dôležité životné hodnoty.

            #rozprávky #preDeti #slovenskéRozprávky
            """;

        var videoMetadata = new
        {
            title = story.Title,
            description,
            tags,
            categoryId = options.YouTubeCategory,
            defaultLanguage = options.YouTubeDefaultLanguage,
            privacyStatus = options.YouTubePrivacyStatus,
            videoFile = Path.GetFullPath(story.Video.File),
            videoDurationSeconds = story.Video.DurationSeconds,
            thumbnail = story.Images?.Cover is not null ? Path.GetFullPath(story.Images.Cover) : null,
            publishDate = DateTimeOffset.UtcNow.ToString("o"),
        };

        string metadataPath = Path.Combine(outputDir, "video-metadata.json");
        string json = JsonSerializer.Serialize(videoMetadata, JsonOptions);
        await File.WriteAllTextAsync(metadataPath, json, ct);

        Console.WriteLine($"[YouTubePublisher] Video metadata saved to: {metadataPath}");
        Console.WriteLine($"[YouTubePublisher] Actual YouTube upload requires OAuth setup.");

        // Scaffolded YouTube upload logic (requires OAuth client secrets):
        //
        // if (options.YouTubeClientSecretsPath is null)
        //     throw new InvalidOperationException("YouTube client secrets path not configured.");
        //
        // using var stream = new FileStream(options.YouTubeClientSecretsPath, FileMode.Open, FileAccess.Read);
        // var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        //     GoogleClientSecrets.FromStream(stream).Secrets,
        //     [YouTubeService.Scope.YoutubeUpload],
        //     "user",
        //     ct);
        //
        // var youtubeService = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer
        // {
        //     HttpClientInitializer = credential,
        //     ApplicationName = "PoucneRozpravky",
        // });
        //
        // var video = new Google.Apis.YouTube.v3.Data.Video
        // {
        //     Snippet = new Google.Apis.YouTube.v3.Data.VideoSnippet
        //     {
        //         Title = story.Title,
        //         Description = description,
        //         Tags = tags,
        //         CategoryId = options.YouTubeCategory,
        //         DefaultLanguage = options.YouTubeDefaultLanguage,
        //     },
        //     Status = new Google.Apis.YouTube.v3.Data.VideoStatus
        //     {
        //         PrivacyStatus = options.YouTubePrivacyStatus,
        //     }
        // };
        //
        // using var videoStream = new FileStream(story.Video.File, FileMode.Open, FileAccess.Read);
        // var uploadRequest = youtubeService.Videos.Insert(video, "snippet,status", videoStream, "video/*");
        // var uploadResponse = await uploadRequest.UploadAsync(ct);
        //
        // if (uploadResponse.Status == Google.Apis.Upload.UploadStatus.Failed)
        //     throw new InvalidOperationException($"YouTube upload failed: {uploadResponse.Exception}");
        //
        // string videoUrl = $"https://www.youtube.com/watch?v={uploadRequest.ResponseBody.Id}";

        return new PublicationInfo
        {
            Platform = "youtube",
            Url = Path.GetFullPath(metadataPath),
            PublishedAt = DateTimeOffset.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                ["title"] = story.Title,
                ["videoFile"] = story.Video.File,
                ["categoryId"] = options.YouTubeCategory,
                ["privacyStatus"] = options.YouTubePrivacyStatus,
                ["status"] = "metadata_prepared",
            }
        };
    }
}
