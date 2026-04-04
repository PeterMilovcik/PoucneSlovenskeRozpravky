namespace PoucneRozpravky.Review;

/// <summary>
/// Configuration for the LanguageTool HTTP API connection.
/// </summary>
public class LanguageToolOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8010/v2";
    public string Language { get; set; } = "sk";
    public string[]? EnabledRules { get; set; }
    public string[]? DisabledRules { get; set; }
}
