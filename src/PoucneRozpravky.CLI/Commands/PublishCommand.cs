using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;
using PoucneRozpravky.Publisher;

namespace PoucneRozpravky.CLI.Commands;

public static class PublishCommand
{
    private static readonly string[] SupportedTargets = ["blog", "spotify", "youtube", "all"];

    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky" };
        var targetOption = new Option<string>("--target") { Description = "Cieľová platforma: blog, spotify, youtube alebo all", DefaultValueFactory = _ => "all" };

        var command = new Command("publish", "Publikuje rozprávku na zvolené platformy");
        command.Add(idArgument);
        command.Add(targetOption);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetRequiredValue(idArgument);
            var target = parseResult.GetValue(targetOption) ?? "all";
            try
            {
                target = target.ToLowerInvariant();
                if (!SupportedTargets.Contains(target))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"❌ Neznáma platforma '{target}'. Podporované: blog, spotify, youtube, all");
                    Console.ResetColor();
                    return;
                }

                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();
                var factory = services.GetRequiredService<PublisherFactory>();

                var metadata = await catalog.GetStoryAsync(id, ct);
                if (metadata is null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"❌ Rozprávka '{id}' sa nenachádza v katalógu.");
                    Console.ResetColor();
                    return;
                }

                var targets = target == "all"
                    ? new[] { "blog", "spotify", "youtube" }
                    : new[] { target };

                foreach (var platform in targets)
                {
                    Console.WriteLine($"📤 Publikujem na {platform}...");
                    try
                    {
                        var publisher = factory.GetPublisher(platform);
                        var pubInfo = await publisher.PublishAsync(metadata, platform, ct);
                        metadata.Publications.Add(pubInfo);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"  ✅ {platform}: {pubInfo.Url}");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Error.WriteLine($"  ⚠️  {platform}: {ex.Message}");
                        Console.ResetColor();
                    }
                }

                // Update status based on what was published
                var publishedPlatforms = metadata.Publications
                    .Select(p => p.Platform.ToLowerInvariant())
                    .Distinct()
                    .ToHashSet();

                if (publishedPlatforms.Contains("blog"))
                    metadata.Status = StoryStatus.PublishedText;
                if (publishedPlatforms.Contains("spotify"))
                    metadata.Status = StoryStatus.PublishedAudio;
                if (publishedPlatforms.Contains("youtube"))
                    metadata.Status = StoryStatus.PublishedVideo;
                if (publishedPlatforms.Contains("blog") &&
                    publishedPlatforms.Contains("spotify") &&
                    publishedPlatforms.Contains("youtube"))
                    metadata.Status = StoryStatus.FullyPublished;

                await catalog.UpdateStoryAsync(metadata, ct);
                await dirManager.SaveMetadataAsync(id, metadata);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Publikovanie dokončené. Stav: {StatusTracker.GetStatusDescription(metadata.Status)}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba pri publikovaní: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }
}
