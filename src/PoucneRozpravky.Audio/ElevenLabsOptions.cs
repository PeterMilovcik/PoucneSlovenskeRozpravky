namespace PoucneRozpravky.Audio;

public sealed class ElevenLabsOptions
{
    public required string ApiKey { get; set; }

    public string BaseUrl { get; set; } = "https://api.elevenlabs.io/v1";

    public required string VoiceId { get; set; }

    public string ModelId { get; set; } = "eleven_multilingual_v2";

    public double Stability { get; set; } = 0.5;

    public double SimilarityBoost { get; set; } = 0.75;

    public int MaxCharsPerRequest { get; set; } = 5000;
}
