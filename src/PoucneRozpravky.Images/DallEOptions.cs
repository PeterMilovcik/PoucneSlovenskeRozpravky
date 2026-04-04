namespace PoucneRozpravky.Images;

public class DallEOptions
{
    public required string ApiKey { get; set; }
    public string Model { get; set; } = "dall-e-3";
    public string ImageSize { get; set; } = "1024x1024";
    public string ImageQuality { get; set; } = "standard";
    public string ImageStyle { get; set; } = "natural";
    public string ArtStylePrefix { get; set; } =
        "Children's book watercolor illustration, bright warm colors, friendly characters, no text, safe for children";
}
