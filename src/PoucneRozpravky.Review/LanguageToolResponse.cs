using System.Text.Json.Serialization;

namespace PoucneRozpravky.Review;

public class LanguageToolResponse
{
    [JsonPropertyName("matches")]
    public List<LanguageToolMatch> Matches { get; set; } = [];
}

public class LanguageToolMatch
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("shortMessage")]
    public string ShortMessage { get; set; } = string.Empty;

    [JsonPropertyName("replacements")]
    public List<LanguageToolReplacement> Replacements { get; set; } = [];

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("rule")]
    public LanguageToolRule Rule { get; set; } = new();

    [JsonPropertyName("context")]
    public LanguageToolContext? Context { get; set; }
}

public class LanguageToolReplacement
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class LanguageToolRule
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public LanguageToolCategory Category { get; set; } = new();
}

public class LanguageToolCategory
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class LanguageToolContext
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }
}
