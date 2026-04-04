using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Catalog;

public class StoryDirectoryManager
{
    private readonly string _workspaceRoot;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private static readonly string[] Subdirectories = ["audio", "images", "video", "kapitoly"];

    public StoryDirectoryManager(string? workspaceRoot = null)
    {
        _workspaceRoot = workspaceRoot ?? Directory.GetCurrentDirectory();
    }

    public string GetStoryPath(string id)
    {
        return Path.Combine(_workspaceRoot, "rozpravky", id);
    }

    public void CreateStoryDirectory(string id)
    {
        var storyPath = GetStoryPath(id);

        foreach (var sub in Subdirectories)
        {
            Directory.CreateDirectory(Path.Combine(storyPath, sub));
        }
    }

    public async Task SaveOutlineAsync(string id, string outline)
    {
        var path = Path.Combine(GetStoryPath(id), "outline.md");
        await File.WriteAllTextAsync(path, outline);
    }

    public async Task SaveStoryAsync(string id, Story story)
    {
        var path = Path.Combine(GetStoryPath(id), "rozpravka.md");

        var sb = new StringBuilder();
        sb.AppendLine("---");
        sb.AppendLine($"title: \"{EscapeYaml(story.Title)}\"");
        sb.AppendLine($"id: \"{story.Id}\"");
        sb.AppendLine($"theme: \"{EscapeYaml(story.Outline.Theme)}\"");
        sb.AppendLine($"moral: \"{EscapeYaml(story.Outline.Moral)}\"");
        sb.AppendLine($"wordCount: {story.WordCount}");
        sb.AppendLine($"estimatedMinutes: {story.EstimatedMinutes:F1}");
        sb.AppendLine($"createdAt: {story.CreatedAt:O}");
        sb.AppendLine("---");
        sb.AppendLine();
        sb.Append(story.Text);

        await File.WriteAllTextAsync(path, sb.ToString());
    }

    public async Task SaveMetadataAsync(string id, StoryMetadata metadata)
    {
        var path = Path.Combine(GetStoryPath(id), "metadata.json");
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, metadata, JsonOptions);
    }

    public async Task<StoryMetadata?> LoadMetadataAsync(string id)
    {
        var path = Path.Combine(GetStoryPath(id), "metadata.json");

        if (!File.Exists(path))
            return null;

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<StoryMetadata>(stream, JsonOptions);
    }

    private static string EscapeYaml(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
