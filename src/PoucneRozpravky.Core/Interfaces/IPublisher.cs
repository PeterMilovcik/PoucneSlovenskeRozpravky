using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.Core.Interfaces;

public interface IPublisher
{
    Task<PublicationInfo> PublishAsync(StoryMetadata story, string platform, CancellationToken ct = default);
}
