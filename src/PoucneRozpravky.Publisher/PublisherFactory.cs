using PoucneRozpravky.Core.Interfaces;

namespace PoucneRozpravky.Publisher;

public class PublisherFactory(PublisherOptions options)
{
    public IPublisher GetPublisher(string platform) => platform.ToLowerInvariant() switch
    {
        "blog" => new BlogPublisher(options),
        "spotify" => new SpotifyPublisher(options),
        "youtube" => new YouTubePublisher(options),
        _ => throw new ArgumentException($"Unknown publishing platform: '{platform}'. Supported platforms: blog, spotify, youtube.", nameof(platform)),
    };
}
