using System.Text.RegularExpressions;

namespace PoucneRozpravky.Review;

/// <summary>
/// Utilities for analysing Slovak-language text: syllable counting,
/// sentence splitting, tokenisation, and passive-voice detection.
/// </summary>
public static partial class SlovakTextAnalyzer
{
    // Slovak vowels (including diacritics and ä)
    private static readonly HashSet<char> Vowels =
    [
        'a', 'á', 'ä', 'e', 'é', 'i', 'í', 'o', 'ó', 'u', 'ú', 'y', 'ý'
    ];

    // Diphthongs that count as a single syllable nucleus
    private static readonly string[] Diphthongs = ["ia", "ie", "iu"];

    // Common Slovak abbreviations that should not trigger sentence splits
    private static readonly HashSet<string> Abbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        "napr", "atď", "sv", "tzv", "resp", "obr", "str", "tel",
        "prof", "doc", "ing", "mgr", "phd", "bc", "jr", "sr",
        "tj", "tzn", "evt", "príp", "pozn", "por", "roč", "č"
    };

    // Passive auxiliary forms of "byť" in past tense
    private static readonly HashSet<string> PassiveAuxiliaries = new(StringComparer.OrdinalIgnoreCase)
    {
        "bol", "bola", "bolo", "boli"
    };

    // Regex matching a Slovak past participle ending (-ný/-ná/-né/-ní, -tý/-tá/-té/-tí, etc.)
    [GeneratedRegex(@"^.+(ný|ná|né|ní|tý|tá|té|tí|ený|ená|ené|ení|aný|aná|ané|aní)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PastParticiplePattern();

    // Regex matching reflexive passive ("sa" + verb)
    [GeneratedRegex(@"\bsa\s+\w+[aeiouyáéíóúýť]\w*\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ReflexivePassivePattern();

    // Sentence-ending punctuation (including ellipsis)
    [GeneratedRegex(@"(?<=[.!?…])\s+(?=[A-ZÁÄČĎÉÍĽĹŇÓÔŔŠŤÚÝŽ""„\p{Lu}])")]
    private static partial Regex SentenceBoundary();

    [GeneratedRegex(@"[^\w\s'-]", RegexOptions.Compiled)]
    private static partial Regex PunctuationPattern();

    /// <summary>
    /// Count syllables in a single Slovak word.
    /// Diphthongs (ia, ie, iu) and ô count as one syllable nucleus.
    /// </summary>
    public static int CountSyllables(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return 0;

        var lower = word.ToLowerInvariant();
        int count = 0;
        int i = 0;

        while (i < lower.Length)
        {
            // ô is a diphthong (uo) that counts as 1 syllable
            if (lower[i] == 'ô')
            {
                count++;
                i++;
                continue;
            }

            if (Vowels.Contains(lower[i]))
            {
                count++;

                // Check for diphthongs ia, ie, iu – consume the second vowel
                if (i + 1 < lower.Length)
                {
                    string pair = lower.Substring(i, 2);
                    if (Array.Exists(Diphthongs, d => d == pair))
                    {
                        i++; // skip the second vowel of the diphthong
                    }
                }

                i++;
                continue;
            }

            i++;
        }

        // Every word has at least one syllable
        return Math.Max(count, 1);
    }

    /// <summary>
    /// Split text into sentences, respecting Slovak abbreviations and dialogue.
    /// </summary>
    public static List<string> SplitIntoSentences(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        // Protect known abbreviations by temporarily replacing their dots
        const string placeholder = "\x00";
        var working = text;

        foreach (var abbr in Abbreviations)
        {
            // Match abbreviation followed by a dot (case-insensitive)
            working = Regex.Replace(
                working,
                $@"\b{Regex.Escape(abbr)}\.",
                $"{abbr}{placeholder}",
                RegexOptions.IgnoreCase);
        }

        // Handle ellipsis – treat as single sentence terminator
        working = working.Replace("...", "…");

        // Split on sentence boundaries
        var parts = SentenceBoundary().Split(working);

        var sentences = new List<string>();
        foreach (var part in parts)
        {
            var restored = part.Replace(placeholder, ".").Trim();
            if (!string.IsNullOrWhiteSpace(restored))
                sentences.Add(restored);
        }

        return sentences;
    }

    /// <summary>
    /// Tokenise text into words, stripping punctuation.
    /// </summary>
    public static List<string> SplitIntoWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var cleaned = PunctuationPattern().Replace(text, " ");
        return cleaned
            .Split((char[]?) null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(w => w.Length > 0 && !w.All(c => c == '-' || c == '\''))
            .ToList();
    }

    /// <summary>
    /// Detect Slovak passive voice constructions in a sentence:
    /// 1) "bol/bola/bolo/boli" + past participle (e.g. "bola potrestaná")
    /// 2) Reflexive passive with "sa" (e.g. "hovorí sa")
    /// </summary>
    public static bool IsPassiveConstruction(string sentence)
    {
        if (string.IsNullOrWhiteSpace(sentence))
            return false;

        // Check for reflexive passive ("sa" + verb)
        if (ReflexivePassivePattern().IsMatch(sentence))
            return true;

        // Check for "bol/bola/bolo/boli" + past participle
        var words = SplitIntoWords(sentence);
        for (int i = 0; i < words.Count - 1; i++)
        {
            if (PassiveAuxiliaries.Contains(words[i])
                && PastParticiplePattern().IsMatch(words[i + 1]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// A word is considered simple if it has at most 2 syllables.
    /// </summary>
    public static bool IsSimpleWord(string word) =>
        CountSyllables(word) <= 2;
}
