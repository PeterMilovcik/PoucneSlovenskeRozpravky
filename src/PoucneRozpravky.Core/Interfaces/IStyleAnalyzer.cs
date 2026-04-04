using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IStyleAnalyzer
{
    Task<StyleAnalysisResult> AnalyzeStyleAsync(string text, CancellationToken ct = default);
}
