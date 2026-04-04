using OpenAI.Images;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Images;

public class DallEImageGenerator : IImageGenerator
{
    private readonly DallEOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ImageClient _imageClient;

    public DallEImageGenerator(DallEOptions options, HttpClient httpClient)
    {
        _options = options;
        _httpClient = httpClient;
        _imageClient = new ImageClient(options.Model, options.ApiKey);
    }

    public Task<List<string>> ExtractScenesAsync(string text, int sceneCount, CancellationToken ct = default)
    {
        var paragraphs = text
            .Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        if (paragraphs.Count == 0)
            return Task.FromResult(new List<string>());

        var scenes = new List<string>();
        int paragraphsPerScene = Math.Max(1, paragraphs.Count / sceneCount);

        for (int i = 0; i < sceneCount && i * paragraphsPerScene < paragraphs.Count; i++)
        {
            int start = i * paragraphsPerScene;
            int end = i == sceneCount - 1
                ? paragraphs.Count
                : Math.Min(start + paragraphsPerScene, paragraphs.Count);

            var segment = string.Join(" ", paragraphs.Skip(start).Take(end - start));
            scenes.Add(ExtractFirstSentence(segment));
        }

        return Task.FromResult(scenes);
    }

    public async Task<string> GenerateImageAsync(string sceneDescription, string outputPath, CancellationToken ct = default)
    {
        var prompt = $"{_options.ArtStylePrefix}: {sceneDescription}";

        var options = new ImageGenerationOptions
        {
            Quality = ParseQuality(_options.ImageQuality),
            Size = ParseSize(_options.ImageSize),
            Style = ParseStyle(_options.ImageStyle),
            ResponseFormat = GeneratedImageFormat.Uri,
        };

        GeneratedImage image = await _imageClient.GenerateImageAsync(prompt, options, ct);

        var imageBytes = await _httpClient.GetByteArrayAsync(image.ImageUri, ct);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        await File.WriteAllBytesAsync(outputPath, imageBytes, ct);

        return outputPath;
    }

    public async Task<ImagesInfo> GenerateAllImagesAsync(Story story, string outputDirectory, CancellationToken ct = default)
    {
        Directory.CreateDirectory(outputDirectory);

        int sceneCount = Math.Max(1, story.Outline.Scenes.Count);
        var sceneDescriptions = await ExtractScenesAsync(story.Text, sceneCount, ct);

        // Generate cover image with a prompt based on title and main characters
        var coverPrompt = BuildCoverPrompt(story);
        var coverPath = Path.Combine(outputDirectory, "cover.png");
        await GenerateImageAsync(coverPrompt, coverPath, ct);

        // Generate one image per scene
        var scenePaths = new List<string>();
        for (int i = 0; i < sceneDescriptions.Count; i++)
        {
            var scenePath = Path.Combine(outputDirectory, $"scene-{i + 1:D2}.png");
            await GenerateImageAsync(sceneDescriptions[i], scenePath, ct);
            scenePaths.Add(scenePath);
        }

        return new ImagesInfo(coverPath, scenePaths, DateTimeOffset.UtcNow);
    }

    private static string BuildCoverPrompt(Story story)
    {
        var characters = string.Join(", ", story.Outline.Characters.Select(c => c.Name).Take(3));
        return $"Obálka detskej knihy: {story.Title}. Hlavné postavy: {characters}. Prostredie: {story.Outline.Setting}";
    }

    private static string ExtractFirstSentence(string text)
    {
        char[] sentenceEnders = ['.', '!', '?'];
        int endIndex = -1;

        foreach (var ender in sentenceEnders)
        {
            int idx = text.IndexOf(ender);
            if (idx >= 0 && (endIndex < 0 || idx < endIndex))
                endIndex = idx;
        }

        return endIndex >= 0 ? text[..(endIndex + 1)].Trim() : text.Trim();
    }

    private static GeneratedImageSize ParseSize(string size) => size switch
    {
        "256x256" => GeneratedImageSize.W256xH256,
        "512x512" => GeneratedImageSize.W512xH512,
        "1024x1024" => GeneratedImageSize.W1024xH1024,
        "1024x1792" => GeneratedImageSize.W1024xH1792,
        "1792x1024" => GeneratedImageSize.W1792xH1024,
        _ => GeneratedImageSize.W1024xH1024,
    };

    private static GeneratedImageQuality ParseQuality(string quality) => quality.ToLowerInvariant() switch
    {
        "high" => GeneratedImageQuality.High,
        _ => GeneratedImageQuality.Standard,
    };

    private static GeneratedImageStyle ParseStyle(string style) => style.ToLowerInvariant() switch
    {
        "vivid" => GeneratedImageStyle.Vivid,
        _ => GeneratedImageStyle.Natural,
    };
}
