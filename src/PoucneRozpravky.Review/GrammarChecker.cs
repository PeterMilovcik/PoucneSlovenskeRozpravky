using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Review;

/// <summary>
/// Checks and auto-corrects Slovak grammar via the LanguageTool HTTP API.
/// </summary>
public class GrammarChecker : IGrammarChecker
{
    private readonly HttpClient _http;
    private readonly LanguageToolOptions _options;
    private readonly ILogger<GrammarChecker> _logger;

    public GrammarChecker(HttpClient http, LanguageToolOptions options, ILogger<GrammarChecker> logger)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GrammarCheckResult> CheckGrammarAsync(string text, CancellationToken ct = default)
    {
        var matches = await FetchMatchesAsync(text, ct);

        var autoCorrected = 0;
        var manualReview = new List<string>();

        foreach (var match in matches)
        {
            if (IsAutoCorrectable(match))
            {
                autoCorrected++;
            }
            else
            {
                var snippet = ExtractSnippet(text, match.Offset, match.Length);
                manualReview.Add($"[{match.Rule.Id}] {match.Message} (at \"{snippet}\")");
            }
        }

        _logger.LogInformation(
            "Grammar check complete: {Total} errors, {Auto} auto-correctable, {Manual} need manual review",
            matches.Count, autoCorrected, manualReview.Count);

        return new GrammarCheckResult(matches.Count, autoCorrected, manualReview);
    }

    public async Task<string> AutoCorrectAsync(string text, CancellationToken ct = default)
    {
        var matches = await FetchMatchesAsync(text, ct);

        // Sort by offset descending so replacements don't shift earlier offsets
        var correctable = matches
            .Where(IsAutoCorrectable)
            .OrderByDescending(m => m.Offset)
            .ToList();

        var result = text;
        foreach (var match in correctable)
        {
            var replacement = match.Replacements[0].Value;
            var original = result.Substring(match.Offset, match.Length);
            result = string.Concat(
                result.AsSpan(0, match.Offset),
                replacement,
                result.AsSpan(match.Offset + match.Length));

            _logger.LogInformation(
                "Auto-corrected [{RuleId}]: \"{Original}\" → \"{Replacement}\"",
                match.Rule.Id, original, replacement);
        }

        _logger.LogInformation("Auto-correction applied {Count} fix(es)", correctable.Count);
        return result;
    }

    private async Task<List<LanguageToolMatch>> FetchMatchesAsync(string text, CancellationToken ct)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/check";

        var parameters = new List<KeyValuePair<string, string>>
        {
            new("text", text),
            new("language", _options.Language),
            new("enabledOnly", "false"),
        };

        if (_options.EnabledRules is { Length: > 0 })
            parameters.Add(new("enabledRules", string.Join(",", _options.EnabledRules)));

        if (_options.DisabledRules is { Length: > 0 })
            parameters.Add(new("disabledRules", string.Join(",", _options.DisabledRules)));

        using var content = new FormUrlEncodedContent(parameters);
        using var response = await _http.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LanguageToolResponse>(
            (JsonSerializerOptions?)null, ct);

        return body?.Matches ?? [];
    }

    /// <summary>
    /// A match is auto-correctable when it has exactly one replacement suggestion.
    /// </summary>
    private static bool IsAutoCorrectable(LanguageToolMatch match) =>
        match.Replacements.Count == 1;

    private static string ExtractSnippet(string text, int offset, int length)
    {
        if (offset >= 0 && offset + length <= text.Length)
            return text.Substring(offset, length);

        return string.Empty;
    }
}
