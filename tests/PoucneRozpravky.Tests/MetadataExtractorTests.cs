using PoucneRozpravky.Preparation;

namespace PoucneRozpravky.Tests;

public class MetadataExtractorTests
{
    private readonly MetadataExtractor _extractor = new();

    private const string SampleFairyTale =
        "Bol raz jeden Janko. Janko žil v malom domčeku na okraji lesa. " +
        "Každý deň Janko pomáhal mamke s prácou. " +
        "Jedného dňa Janko stretol múdru Líšku, ktorá mu povedala tajomstvo lesa.\n\n" +
        "## Poučenie\n" +
        "Kto pomáha iným, nájde šťastie.";

    [Fact]
    public void ExtractFromText_WordCount_IsPositive()
    {
        var result = _extractor.ExtractFromText(SampleFairyTale);

        Assert.True(result.WordCount > 0, $"Expected positive word count but got {result.WordCount}");
    }

    [Fact]
    public void ExtractFromText_WordCount_MatchesExpected()
    {
        var text = "Jeden dva tri štyri päť šesť sedem osem deväť desať.";

        var result = _extractor.ExtractFromText(text);

        // 10 words + 1 period attached = the regex counts \S+ tokens
        Assert.True(result.WordCount >= 10,
            $"Expected at least 10 words but got {result.WordCount}");
    }

    [Fact]
    public void ExtractFromText_EstimatedMinutes_IsWordCountDividedBy140()
    {
        var result = _extractor.ExtractFromText(SampleFairyTale);

        var expectedMinutes = result.WordCount / 140.0;
        Assert.Equal(expectedMinutes, result.EstimatedMinutes, precision: 4);
    }

    [Fact]
    public void ExtractFromText_HasMoral_DetectsPoučenieSection()
    {
        var result = _extractor.ExtractFromText(SampleFairyTale);

        Assert.True(result.HasMoral, "Text with '## Poučenie' section should have HasMoral = true");
    }

    [Fact]
    public void ExtractFromText_HasMoral_DetectsMoralKeyword()
    {
        var text = "Príbeh o Jankovi. Morál: Buď dobrý ku všetkým.";

        var result = _extractor.ExtractFromText(text);

        Assert.True(result.HasMoral, "Text with 'Morál' keyword should have HasMoral = true");
    }

    [Fact]
    public void ExtractFromText_NoMoral_ReturnsFalse()
    {
        var text = "Janko išiel do lesa. Stretol zajaca. Vrátil sa domov.";

        var result = _extractor.ExtractFromText(text);

        Assert.False(result.HasMoral, "Text without moral keywords should have HasMoral = false");
    }

    [Fact]
    public void ExtractFromText_EmptyText_ReturnsZeroWordCount()
    {
        var result = _extractor.ExtractFromText("");

        Assert.Equal(0, result.WordCount);
        Assert.Equal(0.0, result.EstimatedMinutes);
    }
}
