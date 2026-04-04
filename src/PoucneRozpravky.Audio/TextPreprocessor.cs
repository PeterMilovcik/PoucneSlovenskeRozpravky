using System.Text;
using System.Text.RegularExpressions;

namespace PoucneRozpravky.Audio;

public static partial class TextPreprocessor
{
    /// <summary>
    /// Prepares story markdown text for TTS by stripping formatting and normalizing whitespace.
    /// </summary>
    public static string PreprocessForTts(string markdownText)
    {
        if (string.IsNullOrWhiteSpace(markdownText))
            return string.Empty;

        var text = markdownText;

        // Remove YAML front matter (--- ... ---)
        text = YamlFrontMatterRegex().Replace(text, string.Empty);

        // Remove Markdown headings (# ... ######)
        text = MarkdownHeadingRegex().Replace(text, "$1");

        // Remove bold/italic markers
        text = BoldItalicRegex().Replace(text, "$1");
        text = BoldRegex().Replace(text, "$1");
        text = ItalicRegex().Replace(text, "$1");

        // Remove inline code
        text = InlineCodeRegex().Replace(text, "$1");

        // Remove Markdown links, keep text
        text = MarkdownLinkRegex().Replace(text, "$1");

        // Remove Markdown images
        text = MarkdownImageRegex().Replace(text, string.Empty);

        // Remove horizontal rules
        text = HorizontalRuleRegex().Replace(text, "\n");

        // Remove list markers (-, *, numbered)
        text = UnorderedListRegex().Replace(text, string.Empty);
        text = OrderedListRegex().Replace(text, string.Empty);

        // Remove blockquote markers
        text = BlockquoteRegex().Replace(text, string.Empty);

        // Convert dialogue markers (em dash, quotation marks) to natural pauses
        text = text.Replace('\u2014', ','); // em dash
        text = text.Replace('\u2013', ','); // en dash
        text = text.Replace("\u201E", "\""); // lower quotation mark „
        text = text.Replace("\u201C", "\""); // left double quotation mark "
        text = text.Replace("\u201D", "\""); // right double quotation mark "

        // Add pauses at paragraph breaks (double newlines → period + space)
        text = ParagraphBreakRegex().Replace(text, ". ");

        // Normalize whitespace
        text = MultipleSpacesRegex().Replace(text, " ");
        text = text.Trim();

        return text;
    }

    /// <summary>
    /// Splits text into segments that respect sentence boundaries and max character limits.
    /// </summary>
    public static List<string> SplitIntoSegments(string text, int maxChars)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxChars, 0);

        if (string.IsNullOrWhiteSpace(text))
            return [];

        if (text.Length <= maxChars)
            return [text];

        var sentences = SplitIntoSentences(text);
        var segments = new List<string>();
        var current = new StringBuilder();

        foreach (var sentence in sentences)
        {
            // If a single sentence exceeds the limit, we must include it as its own segment
            if (sentence.Length > maxChars)
            {
                if (current.Length > 0)
                {
                    segments.Add(current.ToString().Trim());
                    current.Clear();
                }
                segments.Add(sentence.Trim());
                continue;
            }

            if (current.Length + sentence.Length > maxChars)
            {
                if (current.Length > 0)
                {
                    segments.Add(current.ToString().Trim());
                    current.Clear();
                }
            }

            current.Append(sentence);
        }

        if (current.Length > 0)
            segments.Add(current.ToString().Trim());

        return segments;
    }

    private static List<string> SplitIntoSentences(string text)
    {
        var sentences = new List<string>();
        var matches = SentenceBoundaryRegex().Matches(text);

        int lastIndex = 0;
        foreach (Match match in matches)
        {
            int endIndex = match.Index + match.Length;
            sentences.Add(text[lastIndex..endIndex]);
            lastIndex = endIndex;
        }

        if (lastIndex < text.Length)
            sentences.Add(text[lastIndex..]);

        return sentences;
    }

    [GeneratedRegex(@"\A---\s*\n.*?\n---\s*\n", RegexOptions.Singleline)]
    private static partial Regex YamlFrontMatterRegex();

    [GeneratedRegex(@"^#{1,6}\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex MarkdownHeadingRegex();

    [GeneratedRegex(@"\*\*\*(.+?)\*\*\*")]
    private static partial Regex BoldItalicRegex();

    [GeneratedRegex(@"\*\*(.+?)\*\*")]
    private static partial Regex BoldRegex();

    [GeneratedRegex(@"[*_](.+?)[*_]")]
    private static partial Regex ItalicRegex();

    [GeneratedRegex(@"`(.+?)`")]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"\[(.+?)\]\(.+?\)")]
    private static partial Regex MarkdownLinkRegex();

    [GeneratedRegex(@"!\[.*?\]\(.+?\)")]
    private static partial Regex MarkdownImageRegex();

    [GeneratedRegex(@"^[-*_]{3,}\s*$", RegexOptions.Multiline)]
    private static partial Regex HorizontalRuleRegex();

    [GeneratedRegex(@"^[\s]*[-*+]\s+", RegexOptions.Multiline)]
    private static partial Regex UnorderedListRegex();

    [GeneratedRegex(@"^[\s]*\d+\.\s+", RegexOptions.Multiline)]
    private static partial Regex OrderedListRegex();

    [GeneratedRegex(@"^>\s?", RegexOptions.Multiline)]
    private static partial Regex BlockquoteRegex();

    [GeneratedRegex(@"\n{2,}")]
    private static partial Regex ParagraphBreakRegex();

    [GeneratedRegex(@"[ \t]+")]
    private static partial Regex MultipleSpacesRegex();

    [GeneratedRegex(@"(?<=[.!?…])\s+")]
    private static partial Regex SentenceBoundaryRegex();
}
