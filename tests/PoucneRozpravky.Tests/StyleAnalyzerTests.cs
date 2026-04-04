using PoucneRozpravky.Review;

namespace PoucneRozpravky.Tests;

public class StyleAnalyzerTests
{
    private readonly StyleAnalyzer _analyzer = new();

    // Short, simple fairy tale snippet with short sentences and simple words
    private const string SimpleFairyTale =
        "Bol raz jeden malý zajac. Žil v lese. Mal rád mrkvu. Každý deň skákal po lúke.";

    // Longer, more complex text with longer sentences
    private const string ComplexText =
        "V hlbokom a tajomnom lese za siedmimi horami a siedmimi dolinami žil starý múdry čarodejník, " +
        "ktorý poznal všetky tajomstvá prírody a rozprával sa so zvieratami o živote a spravodlivosti. " +
        "Jeho neobyčajné a mimoriadne schopnosti mu umožňovali premieňať kamene na zlato " +
        "a rozpúšťať najsilnejšie kúzla temných bytostí.";

    [Fact]
    public async Task AnalyzeStyle_SimpleFairyTale_ReturnsReasonableMetrics()
    {
        var result = await _analyzer.AnalyzeStyleAsync(SimpleFairyTale);

        Assert.True(result.AverageSentenceLength > 0);
        Assert.True(result.MaxSentenceLength > 0);
        Assert.True(result.SimpleWordPercentage > 0);
    }

    [Fact]
    public async Task AnalyzeStyle_AverageSentenceLength_IsCalculatedCorrectly()
    {
        // "Bol raz jeden malý zajac." = 5 words
        // "Žil v lese." = 3 words
        // "Mal rád mrkvu." = 3 words
        // "Každý deň skákal po lúke." = 5 words
        // Average = (5+3+3+5)/4 = 4.0
        var result = await _analyzer.AnalyzeStyleAsync(SimpleFairyTale);

        Assert.Equal(4.0, result.AverageSentenceLength);
    }

    [Fact]
    public async Task AnalyzeStyle_MaxSentenceLength_IsCalculatedCorrectly()
    {
        var result = await _analyzer.AnalyzeStyleAsync(SimpleFairyTale);

        // Max sentence has 5 words
        Assert.Equal(5, result.MaxSentenceLength);
    }

    [Fact]
    public async Task AnalyzeStyle_SimpleWordPercentage_HighForSimpleText()
    {
        var result = await _analyzer.AnalyzeStyleAsync(SimpleFairyTale);

        // Most words in SimpleFairyTale are ≤2 syllables
        Assert.True(result.SimpleWordPercentage >= 70,
            $"Expected high simple word percentage but got {result.SimpleWordPercentage}%");
    }

    [Fact]
    public async Task AnalyzeStyle_ComplexText_HasLongerSentences()
    {
        var result = await _analyzer.AnalyzeStyleAsync(ComplexText);

        Assert.True(result.AverageSentenceLength > 10,
            $"Expected longer average sentences for complex text but got {result.AverageSentenceLength}");
    }

    [Fact]
    public async Task AnalyzeStyle_ComplexText_HasLowerSimpleWordPercentage()
    {
        var simpleResult = await _analyzer.AnalyzeStyleAsync(SimpleFairyTale);
        var complexResult = await _analyzer.AnalyzeStyleAsync(ComplexText);

        Assert.True(complexResult.SimpleWordPercentage < simpleResult.SimpleWordPercentage,
            $"Complex text ({complexResult.SimpleWordPercentage}%) should have lower simple word % than simple text ({simpleResult.SimpleWordPercentage}%)");
    }

    [Fact]
    public async Task AnalyzeStyle_EmptyText_ReturnsZeroMetrics()
    {
        var result = await _analyzer.AnalyzeStyleAsync("");

        Assert.Equal(0, result.AverageSentenceLength);
        Assert.Equal(0, result.MaxSentenceLength);
        Assert.Equal(100, result.SimpleWordPercentage);
    }
}
