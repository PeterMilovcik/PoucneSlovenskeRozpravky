using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IAudioGenerator
{
    Task<AudioInfo> GenerateAudioAsync(string text, string outputPath, CancellationToken ct = default);
}
