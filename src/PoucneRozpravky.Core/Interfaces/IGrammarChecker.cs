using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IGrammarChecker
{
    Task<GrammarCheckResult> CheckGrammarAsync(string text, CancellationToken ct = default);
    Task<string> AutoCorrectAsync(string text, CancellationToken ct = default);
}
