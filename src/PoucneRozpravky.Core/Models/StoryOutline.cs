namespace PoucneRozpravky.Core.Models;

public record StoryCharacter(string Name, string Description, string Archetype);

public record SceneOutline(int Number, string Title, string Description, int EstimatedWords);

public class StoryOutline
{
    public required string Title { get; set; }
    public required string Theme { get; set; }
    public required string Moral { get; set; }
    public List<StoryCharacter> Characters { get; set; } = [];
    public List<SceneOutline> Scenes { get; set; } = [];
    public required string Setting { get; set; }
    public int TargetMinutes { get; set; }
    public int TargetWordCount { get; set; }
}
