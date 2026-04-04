using PoucneRozpravky.Core.Models;
using PoucneRozpravky.Review;

namespace PoucneRozpravky.Tests;

public class ContentReviewerTests
{
    private readonly ContentReviewer _reviewer = new(new ContentReviewOptions());

    private static StoryOutline CreateOutline(
        string moral = "dobro vždy zvíťazí",
        params string[] characterNames)
    {
        var characters = characterNames.Length > 0
            ? characterNames.Select(n => new StoryCharacter(n, "popis", "hrdina")).ToList()
            : [new StoryCharacter("Janko", "chlapec", "hrdina")];

        return new StoryOutline
        {
            Title = "Testovacia rozprávka",
            Theme = "priateľstvo",
            Moral = moral,
            Setting = "les",
            Characters = characters,
            TargetMinutes = 10,
            TargetWordCount = 1400
        };
    }

    [Fact]
    public async Task ReviewContent_TextWithForbiddenWords_FailsAgeAppropriateness()
    {
        var text = "Bol raz jeden kráľ. Janko chcel násilie a zabiť draka. Poučenie: dobro vždy zvíťazí.";
        var outline = CreateOutline();

        var result = await _reviewer.ReviewContentAsync(text, outline);

        Assert.Equal("fail", result.CategoryResults["AgeAppropriateness"]);
        Assert.Contains(result.Comments, c => c.Contains("nevhodné slová"));
    }

    [Fact]
    public async Task ReviewContent_TextWithFearWords_WarnsEmotionalSafety()
    {
        var text = "Bol raz jeden Janko. V lese bolo strašidelný tieň. Poučenie: dobro vždy zvíťazí.";
        var outline = CreateOutline();

        var result = await _reviewer.ReviewContentAsync(text, outline);

        Assert.Equal("warn", result.CategoryResults["EmotionalSafety"]);
        Assert.Contains(result.Comments, c => c.Contains("strach"));
    }

    [Fact]
    public async Task ReviewContent_TextWithExplicitMoral_PassesEducationalValue()
    {
        var text = "Bol raz jeden Janko. Janko bol dobrý. Janko pomáhal. " +
                   "A tak sa naučil, že dobro vždy zvíťazí. Poučenie: dobro vždy zvíťazí.";
        var outline = CreateOutline();

        var result = await _reviewer.ReviewContentAsync(text, outline);

        Assert.Equal("pass", result.CategoryResults["EducationalValue"]);
    }

    [Fact]
    public async Task ReviewContent_TextMentioningAllCharacters_PassesLogicalConsistency()
    {
        var text = "Bol raz jeden Janko a Marienka. " +
                   "Janko a Marienka sa stretli v lese. Janko bol statočný a Marienka múdra. " +
                   "Janko a Marienka spolu našli cestu domov. Poučenie: dobro vždy zvíťazí.";
        var outline = CreateOutline("dobro vždy zvíťazí", "Janko", "Marienka");

        var result = await _reviewer.ReviewContentAsync(text, outline);

        Assert.Equal("pass", result.CategoryResults["LogicalConsistency"]);
    }

    [Fact]
    public async Task ReviewContent_MissingCharacter_FailsLogicalConsistency()
    {
        var text = "Bol raz jeden Janko. Janko žil šťastne. Janko bol dobrý. Poučenie: dobro vždy zvíťazí.";
        var outline = CreateOutline("dobro vždy zvíťazí", "Janko", "Marienka");

        var result = await _reviewer.ReviewContentAsync(text, outline);

        Assert.Equal("fail", result.CategoryResults["LogicalConsistency"]);
        Assert.Contains(result.Comments, c => c.Contains("Marienka"));
    }

    [Fact]
    public async Task ReviewContent_CleanFairyTale_PassesAllChecks()
    {
        var text = "Bol raz jeden Janko. Janko bol statočný chlapec. Janko pomáhal všetkým. " +
                   "A od tej doby Janko vedel, že dobro vždy zvíťazí. Poučenie: dobro vždy zvíťazí.";
        var outline = CreateOutline();

        var result = await _reviewer.ReviewContentAsync(text, outline);

        foreach (var (category, status) in result.CategoryResults)
        {
            Assert.True(status != "fail",
                $"Category '{category}' should not fail for clean text, but got '{status}'.");
        }
    }
}
