using System.Text;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Publisher;

public class BlogPublisher(PublisherOptions options) : IPublisher
{
    public async Task<PublicationInfo> PublishAsync(
        StoryMetadata story,
        string platform,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(story);

        string outputDir = Path.Combine(options.OutputBaseDirectory, "blog", story.Id);
        Directory.CreateDirectory(outputDir);

        string content = GenerateMarkdown(story);
        string fileName = $"{story.Id}.md";
        string outputPath = Path.Combine(outputDir, fileName);

        await File.WriteAllTextAsync(outputPath, content, Encoding.UTF8, ct);

        Console.WriteLine($"[BlogPublisher] Blog post saved to: {outputPath}");
        Console.WriteLine($"[BlogPublisher] Actual blog publishing requires platform-specific configuration (WordPress, Ghost, etc.).");

        return new PublicationInfo
        {
            Platform = "blog",
            Url = Path.GetFullPath(outputPath),
            PublishedAt = DateTimeOffset.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                ["format"] = options.BlogFormat,
                ["author"] = options.BlogAuthor,
                ["title"] = story.Title,
                ["status"] = "local_draft",
            }
        };
    }

    private static string GenerateMarkdown(StoryMetadata story)
    {
        var sb = new StringBuilder();

        sb.AppendLine("---");
        sb.AppendLine($"title: \"{story.Title}\"");
        sb.AppendLine($"date: {DateTimeOffset.UtcNow:yyyy-MM-dd}");
        sb.AppendLine($"theme: \"{story.Theme}\"");
        sb.AppendLine($"moral: \"{story.Moral}\"");
        sb.AppendLine($"words: {story.WordCount}");
        sb.AppendLine($"reading_time: {story.TargetMinutes} min");
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine($"# {story.Title}");
        sb.AppendLine();

        if (story.Images?.Cover is not null)
        {
            sb.AppendLine($"![{story.Title}]({story.Images.Cover})");
            sb.AppendLine();
        }

        sb.AppendLine($"**Téma:** {story.Theme}");
        sb.AppendLine();
        sb.AppendLine($"**Ponaučenie:** {story.Moral}");
        sb.AppendLine();

        if (story.Audio is not null)
        {
            sb.AppendLine("## 🎧 Počúvaj rozprávku");
            sb.AppendLine();
            sb.AppendLine($"Audio verzia ({story.Audio.DurationSeconds:F0} sekúnd): [{Path.GetFileName(story.Audio.File)}]({story.Audio.File})");
            sb.AppendLine();
        }

        if (story.Video is not null)
        {
            sb.AppendLine("## 🎬 Pozri si video");
            sb.AppendLine();
            sb.AppendLine($"Video verzia ({story.Video.DurationSeconds:F0} sekúnd): [{Path.GetFileName(story.Video.File)}]({story.Video.File})");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
