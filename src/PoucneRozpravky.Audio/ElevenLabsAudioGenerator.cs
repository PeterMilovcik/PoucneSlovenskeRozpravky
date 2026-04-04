using ElevenLabs;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Audio;

public sealed class ElevenLabsAudioGenerator : IAudioGenerator
{
    private readonly ElevenLabsOptions _options;
    private readonly ElevenLabsClient _client;

    public ElevenLabsAudioGenerator(ElevenLabsOptions options, HttpClient httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _client = new ElevenLabsClient(httpClient, new Uri(options.BaseUrl));
        _client.AuthorizeUsingApiKeyInHeader(options.ApiKey);
    }

    public async Task<AudioInfo> GenerateAudioAsync(
        string text,
        string outputPath,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var segments = TextPreprocessor.SplitIntoSegments(text, _options.MaxCharsPerRequest);

        if (segments.Count == 1)
        {
            var audioBytes = await GenerateSegmentAsync(segments[0], ct).ConfigureAwait(false);
            await File.WriteAllBytesAsync(outputPath, audioBytes, ct).ConfigureAwait(false);
        }
        else
        {
            await using var outputStream = File.Create(outputPath);
            foreach (var segment in segments)
            {
                var audioBytes = await GenerateSegmentAsync(segment, ct).ConfigureAwait(false);
                await outputStream.WriteAsync(audioBytes, ct).ConfigureAwait(false);
            }
        }

        var durationSeconds = EstimateDuration(text);

        return new AudioInfo(
            File: outputPath,
            DurationSeconds: durationSeconds,
            VoiceId: _options.VoiceId,
            GeneratedAt: DateTimeOffset.UtcNow);
    }

    private async Task<byte[]> GenerateSegmentAsync(string text, CancellationToken ct)
    {
        var voiceSettings = new VoiceSettingsResponseModel
        {
            Stability = _options.Stability,
            SimilarityBoost = _options.SimilarityBoost,
        };

        var request = new BodyTextToSpeechFull
        {
            Text = text,
            ModelId = _options.ModelId,
            VoiceSettings = voiceSettings,
        };

        return await _client.TextToSpeech.CreateTextToSpeechByVoiceIdAsync(
            voiceId: _options.VoiceId,
            request: request,
            outputFormat: TextToSpeechFullOutputFormat.Mp344100128,
            cancellationToken: ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Estimates audio duration from word count (approx 140 words per minute).
    /// </summary>
    private static double EstimateDuration(string text)
    {
        var wordCount = text.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
        return wordCount / 140.0 * 60.0;
    }
}
