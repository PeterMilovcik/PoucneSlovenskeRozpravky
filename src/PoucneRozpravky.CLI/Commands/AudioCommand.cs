using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class AudioCommand
{
    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky" };

        var command = new Command("audio", "Vygeneruje audio nahrávku rozprávky cez ElevenLabs");
        command.Add(idArgument);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetRequiredValue(idArgument);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();
                var audioGenerator = services.GetRequiredService<IAudioGenerator>();

                var storyPath = Path.Combine(dirManager.GetStoryPath(id), "rozpravka.md");
                if (!File.Exists(storyPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"❌ Súbor nenájdený: {storyPath}");
                    Console.ResetColor();
                    return;
                }

                var metadata = await catalog.GetStoryAsync(id, ct);
                if (metadata is null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"❌ Rozprávka '{id}' sa nenachádza v katalógu.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine($"🎙️  Generujem audio pre '{metadata.Title}'...");

                var text = await File.ReadAllTextAsync(storyPath, ct);
                var outputPath = Path.Combine(dirManager.GetStoryPath(id), "audio", "rozpravka.mp3");

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                var audioInfo = await audioGenerator.GenerateAudioAsync(text, outputPath, ct);

                metadata.Audio = audioInfo;
                metadata.Status = StoryStatus.AudioReady;
                await catalog.UpdateStoryAsync(metadata, ct);
                await dirManager.SaveMetadataAsync(id, metadata);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Audio vygenerované!");
                Console.ResetColor();
                Console.WriteLine($"   Súbor: {outputPath}");
                Console.WriteLine($"   Dĺžka: {audioInfo.DurationSeconds:F0} sekúnd ({audioInfo.DurationSeconds / 60:F1} min)");
                Console.WriteLine($"   Hlas: {audioInfo.VoiceId}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba pri generovaní audia: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }
}
