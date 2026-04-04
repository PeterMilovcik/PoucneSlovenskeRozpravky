using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IImageGenerator
{
    Task<List<string>> ExtractScenesAsync(string text, int sceneCount, CancellationToken ct = default);
    Task<string> GenerateImageAsync(string sceneDescription, string outputPath, CancellationToken ct = default);
    Task<ImagesInfo> GenerateAllImagesAsync(Story story, string outputDirectory, CancellationToken ct = default);
}
