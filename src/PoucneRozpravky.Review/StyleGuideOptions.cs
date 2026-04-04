namespace PoucneRozpravky.Review;

/// <summary>
/// Configurable thresholds for fairy-tale style analysis.
/// </summary>
public sealed class StyleGuideOptions
{
    public double AverageSentenceLengthMin { get; set; } = 8;
    public double AverageSentenceLengthMax { get; set; } = 12;
    public int MaxSentenceLength { get; set; } = 25;
    public double FleschKincaidMin { get; set; } = 1.5;
    public double FleschKincaidMax { get; set; } = 2.5;
    public double SimpleWordPercentageMin { get; set; } = 85;
    public double MaxPassiveVoicePercentage { get; set; } = 10;
}
