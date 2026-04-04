using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Review;

/// <summary>
/// Analyses the writing style of Slovak fairy-tale text and produces a
/// <see cref="StyleAnalysisResult"/> with readability and diversity metrics.
/// </summary>
public sealed class StyleAnalyzer : IStyleAnalyzer
{
    private readonly StyleGuideOptions _options;

    public StyleAnalyzer(StyleGuideOptions? options = null)
    {
        _options = options ?? new StyleGuideOptions();
    }

    public Task<StyleAnalysisResult> AnalyzeStyleAsync(string text, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(text);

        var sentences = SlovakTextAnalyzer.SplitIntoSentences(text);
        var allWords = SlovakTextAnalyzer.SplitIntoWords(text);

        if (sentences.Count == 0 || allWords.Count == 0)
        {
            return Task.FromResult(new StyleAnalysisResult(
                AverageSentenceLength: 0,
                MaxSentenceLength: 0,
                FleschKincaidGrade: 0,
                SimpleWordPercentage: 100,
                PassiveVoicePercentage: 0,
                TypeTokenRatio: 0));
        }

        // Words per sentence
        var wordsPerSentence = sentences
            .Select(s => SlovakTextAnalyzer.SplitIntoWords(s).Count)
            .ToList();

        double avgSentenceLength = wordsPerSentence.Average();
        int maxSentenceLength = wordsPerSentence.Max();

        // Syllable counts
        int totalSyllables = allWords.Sum(SlovakTextAnalyzer.CountSyllables);

        // Flesch-Kincaid adapted for Slovak
        double fk = 0.39 * ((double)allWords.Count / sentences.Count)
                   + 11.8 * ((double)totalSyllables / allWords.Count)
                   - 15.59;

        // Simple word percentage (≤ 2 syllables)
        int simpleCount = allWords.Count(SlovakTextAnalyzer.IsSimpleWord);
        double simpleWordPercentage = (double)simpleCount / allWords.Count * 100.0;

        // Passive voice percentage
        int passiveSentences = sentences.Count(SlovakTextAnalyzer.IsPassiveConstruction);
        double passiveVoicePercentage = (double)passiveSentences / sentences.Count * 100.0;

        // Type-token ratio (vocabulary diversity)
        var uniqueWords = allWords
            .Select(w => w.ToLowerInvariant())
            .Distinct()
            .Count();
        double typeTokenRatio = (double)uniqueWords / allWords.Count;

        var result = new StyleAnalysisResult(
            AverageSentenceLength: Math.Round(avgSentenceLength, 2),
            MaxSentenceLength: maxSentenceLength,
            FleschKincaidGrade: Math.Round(fk, 2),
            SimpleWordPercentage: Math.Round(simpleWordPercentage, 2),
            PassiveVoicePercentage: Math.Round(passiveVoicePercentage, 2),
            TypeTokenRatio: Math.Round(typeTokenRatio, 4));

        return Task.FromResult(result);
    }
}
