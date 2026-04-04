using PoucneRozpravky.Review;

namespace PoucneRozpravky.Tests;

public class SlovakTextAnalyzerTests
{
    [Theory]
    [InlineData("dom", 1)]
    [InlineData("mama", 2)]
    [InlineData("priateľstvo", 3)]
    [InlineData("rozprávka", 3)]
    [InlineData("bôčik", 2)]
    public void CountSyllables_SlovakWords_ReturnsExpected(string word, int expected)
    {
        Assert.Equal(expected, SlovakTextAnalyzer.CountSyllables(word));
    }

    [Theory]
    [InlineData("viac", 1)]
    [InlineData("lietadlo", 3)]
    [InlineData("kôň", 1)]
    public void CountSyllables_Diphthongs_CountAsOneSyllable(string word, int expected)
    {
        Assert.Equal(expected, SlovakTextAnalyzer.CountSyllables(word));
    }

    [Fact]
    public void CountSyllables_EmptyOrWhitespace_ReturnsZero()
    {
        Assert.Equal(0, SlovakTextAnalyzer.CountSyllables(""));
        Assert.Equal(0, SlovakTextAnalyzer.CountSyllables("   "));
    }

    [Fact]
    public void CountSyllables_SingleConsonantWord_ReturnsOne()
    {
        // Even consonant-only strings get minimum 1 syllable
        Assert.Equal(1, SlovakTextAnalyzer.CountSyllables("brk"));
    }

    [Fact]
    public void SplitIntoSentences_MultipleSentences_SplitsCorrectly()
    {
        var text = "Bol raz jeden kráľ. Mal tri dcéry. Všetky boli krásne.";

        var sentences = SlovakTextAnalyzer.SplitIntoSentences(text);

        Assert.Equal(3, sentences.Count);
        Assert.Contains("Bol raz jeden kráľ.", sentences);
    }

    [Fact]
    public void SplitIntoSentences_WithDialogue_HandlesQuotes()
    {
        var text = "Kr\u00e1\u013e povedal: \u201ePo\u010f sem!\u201c Princ pri\u0161iel.";

        var sentences = SlovakTextAnalyzer.SplitIntoSentences(text);

        Assert.True(sentences.Count >= 1);
    }

    [Fact]
    public void SplitIntoSentences_WithAbbreviations_DoesNotSplitOnAbbreviation()
    {
        var text = "Pozri napr. toto miesto. Je to pekné.";

        var sentences = SlovakTextAnalyzer.SplitIntoSentences(text);

        // "napr." should not cause a split
        Assert.True(sentences.Count <= 2,
            $"Expected at most 2 sentences but got {sentences.Count}: [{string.Join(" | ", sentences)}]");
    }

    [Fact]
    public void SplitIntoSentences_EmptyText_ReturnsEmptyList()
    {
        Assert.Empty(SlovakTextAnalyzer.SplitIntoSentences(""));
        Assert.Empty(SlovakTextAnalyzer.SplitIntoSentences("   "));
    }

    [Fact]
    public void SplitIntoWords_RemovesPunctuation()
    {
        var text = "Kráľ, kráľovná a princ!";

        var words = SlovakTextAnalyzer.SplitIntoWords(text);

        Assert.Contains("Kráľ", words);
        Assert.Contains("kráľovná", words);
        Assert.Contains("a", words);
        Assert.Contains("princ", words);
        Assert.DoesNotContain(",", words);
        Assert.DoesNotContain("!", words);
    }

    [Fact]
    public void SplitIntoWords_EmptyText_ReturnsEmptyList()
    {
        Assert.Empty(SlovakTextAnalyzer.SplitIntoWords(""));
    }

    [Theory]
    [InlineData("dom", true)]
    [InlineData("mama", true)]
    [InlineData("kôň", true)]
    public void IsSimpleWord_TwoOrFewerSyllables_ReturnsTrue(string word, bool expected)
    {
        Assert.Equal(expected, SlovakTextAnalyzer.IsSimpleWord(word));
    }

    [Theory]
    [InlineData("rozprávka", false)]
    [InlineData("priateľstvo", false)]
    public void IsSimpleWord_MoreThanTwoSyllables_ReturnsFalse(string word, bool expected)
    {
        Assert.Equal(expected, SlovakTextAnalyzer.IsSimpleWord(word));
    }
}
