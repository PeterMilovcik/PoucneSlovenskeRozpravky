using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class VideoCommand
{
    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky" };

        var command = new Command("video", "Vytvorí video prezentáciu z audia a obrázkov");
        command.Add(idArgument);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetRequiredValue(idArgument);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();
                var videoGenerator = services.GetRequiredService<IVideoGenerator>();

                var metadata = await catalog.GetStoryAsync(id, ct);
                if (metadata is null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"❌ Rozprávka '{id}' sa nenachádza v katalógu.");
                    Console.ResetColor();
                    return;
                }

                if (metadata.Audio is null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("❌ Audio ešte nie je vygenerované. Spustite najprv 'rozpravky audio'.");
                    Console.ResetColor();
                    return;
                }

                if (metadata.Images is null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("❌ Obrázky ešte nie sú vygenerované. Spustite najprv 'rozpravky images'.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine($"🎬 Vytváram video pre '{metadata.Title}'...");

                var audioPath = metadata.Audio.File;
                var imagePaths = new List<string> { metadata.Images.Cover };
                imagePaths.AddRange(metadata.Images.Scenes);

                var outputPath = Path.Combine(dirManager.GetStoryPath(id), "video", "rozpravka.mp4");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                var videoInfo = await videoGenerator.GenerateVideoAsync(audioPath, imagePaths, outputPath, ct);

                metadata.Video = videoInfo;
                metadata.Status = StoryStatus.VideoReady;
                await catalog.UpdateStoryAsync(metadata, ct);
                await dirManager.SaveMetadataAsync(id, metadata);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Video vytvorené!");
                Console.ResetColor();
                Console.WriteLine($"   Súbor: {outputPath}");
                Console.WriteLine($"   Dĺžka: {videoInfo.DurationSeconds:F0} sekúnd ({videoInfo.DurationSeconds / 60:F1} min)");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba pri vytváraní videa: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }
}
