using System.Text.Json.Serialization;

namespace PoucneRozpravky.Preparation;

public class ThemeCollection
{
    [JsonPropertyName("themes")]
    public List<ThemeEntry> Themes { get; set; } = [];

    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; } = [];
}

public class ThemeEntry
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("category")]
    public required string Category { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("exampleMoral")]
    public required string ExampleMoral { get; set; }
}
