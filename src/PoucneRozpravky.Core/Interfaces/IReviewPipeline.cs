using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IReviewPipeline
{
    Task<QualityReport> RunReviewAsync(Story story, CancellationToken ct = default);
}
