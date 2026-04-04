using System.Diagnostics;
using System.Globalization;
using FFMpegCore;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Video;

public class SlideshowVideoGenerator(VideoOptions options) : IVideoGenerator
{
    public async Task<VideoInfo> GenerateVideoAsync(
        string audioPath,
        List<string> imagePaths,
        string outputPath,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(audioPath);
        ArgumentNullException.ThrowIfNull(imagePaths);
        if (imagePaths.Count == 0)
            throw new ArgumentException("At least one image is required.", nameof(imagePaths));
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var audioInfo = await FFProbe.AnalyseAsync(audioPath, cancellationToken: ct);
        double totalDuration = audioInfo.Duration.TotalSeconds;
        int imageCount = imagePaths.Count;
        double durationPerImage = totalDuration / imageCount;
        double transition = options.TransitionDurationSeconds;

        var resolution = options.Resolution.Split('x');
        string width = resolution[0];
        string height = resolution[1];

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        // Build FFmpeg arguments directly for complex filter graph with crossfade transitions.
        var args = new List<string>();

        // Add each image as an input
        foreach (string imagePath in imagePaths)
        {
            args.Add("-loop");
            args.Add("1");
            args.Add("-t");
            args.Add(durationPerImage.ToString("F3", CultureInfo.InvariantCulture));
            args.Add("-i");
            args.Add(imagePath);
        }

        // Add audio input
        args.Add("-i");
        args.Add(audioPath);

        int audioInputIndex = imageCount;

        // Build the complex filter graph
        var filterParts = new List<string>();

        // Scale each image input to target resolution
        for (int i = 0; i < imageCount; i++)
        {
            filterParts.Add(
                $"[{i}:v]scale={width}:{height}:force_original_aspect_ratio=decrease," +
                $"pad={width}:{height}:(ow-iw)/2:(oh-ih)/2:color=black," +
                $"setsar=1,format=yuva420p[v{i}]");
        }

        if (imageCount == 1)
        {
            // Single image — no transitions needed
            filterParts.Add($"[v0]format={options.PixelFormat}[vout]");
        }
        else
        {
            // Chain crossfade transitions between consecutive segments
            string previousLabel = "v0";
            double offset = durationPerImage - transition;

            for (int i = 1; i < imageCount; i++)
            {
                string outputLabel = i < imageCount - 1 ? $"xf{i}" : "vout";
                double currentOffset = offset + (i - 1) * (durationPerImage - transition);

                filterParts.Add(
                    $"[{previousLabel}][v{i}]xfade=transition=fade:" +
                    $"duration={transition.ToString("F3", CultureInfo.InvariantCulture)}:" +
                    $"offset={currentOffset.ToString("F3", CultureInfo.InvariantCulture)}" +
                    (i == imageCount - 1 ? $",format={options.PixelFormat}" : "") +
                    $"[{outputLabel}]");

                previousLabel = outputLabel;
            }
        }

        string filterGraph = string.Join(";", filterParts);

        args.Add("-filter_complex");
        args.Add(filterGraph);

        // Map outputs
        args.Add("-map");
        args.Add("[vout]");
        args.Add("-map");
        args.Add($"{audioInputIndex}:a");

        // Encoding settings
        args.Add("-c:v");
        args.Add(options.VideoCodec);
        args.Add("-c:a");
        args.Add(options.AudioCodec);
        args.Add("-pix_fmt");
        args.Add(options.PixelFormat);
        args.Add("-shortest");
        args.Add("-y");
        args.Add(outputPath);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = options.FfmpegPath,
                Arguments = string.Join(' ', args.Select(EscapeArgument)),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string stderr = await process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"FFmpeg exited with code {process.ExitCode}: {stderr}");

        var outputInfo = await FFProbe.AnalyseAsync(outputPath, cancellationToken: ct);

        return new VideoInfo(
            File: outputPath,
            DurationSeconds: outputInfo.Duration.TotalSeconds,
            GeneratedAt: DateTimeOffset.UtcNow);
    }

    private static string EscapeArgument(string arg)
    {
        if (arg.Contains(' ') || arg.Contains('"'))
            return $"\"{arg.Replace("\"", "\\\"")}\"";
        return arg;
    }
}
