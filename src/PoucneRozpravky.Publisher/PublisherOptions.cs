namespace PoucneRozpravky.Publisher;

public class PublisherOptions
{
    public string OutputBaseDirectory { get; set; } = "publish";

    // Blog settings
    public string BlogFormat { get; set; } = "markdown";
    public string BlogAuthor { get; set; } = "Poučné Slovenské Rozprávky";

    // Spotify / podcast settings
    public string PodcastTitle { get; set; } = "Poučné Slovenské Rozprávky";
    public string PodcastLanguage { get; set; } = "sk";
    public string PodcastCategory { get; set; } = "Kids & Family";

    // YouTube settings
    public string YouTubeCategory { get; set; } = "22";
    public string YouTubeDefaultLanguage { get; set; } = "sk";
    public string YouTubePrivacyStatus { get; set; } = "private";
    public List<string> YouTubeDefaultTags { get; set; } =
    [
        "rozprávky",
        "slovenské rozprávky",
        "pre deti",
        "poučné príbehy",
        "bedtime stories",
        "Slovak fairy tales"
    ];
    public string? YouTubeClientSecretsPath { get; set; }
}
