using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IVideoGenerator
{
    Task<VideoInfo> GenerateVideoAsync(string audioPath, List<string> imagePaths, string outputPath, CancellationToken ct = default);
}
