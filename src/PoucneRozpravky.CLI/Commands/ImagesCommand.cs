using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class ImagesCommand
{
    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky" };

        var command = new Command("images", "Vygeneruje obrázky pre rozprávku cez DALL-E");
        command.Add(idArgument);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetRequiredValue(idArgument);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();
                var imageGenerator = services.GetRequiredService<IImageGenerator>();

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

                Console.WriteLine($"🎨 Generujem obrázky pre '{metadata.Title}'...");

                var text = await File.ReadAllTextAsync(storyPath, ct);
                var outputDir = Path.Combine(dirManager.GetStoryPath(id), "images");
                Directory.CreateDirectory(outputDir);

                var story = new Story
                {
                    Id = id,
                    Title = metadata.Title,
                    Text = text,
                    Outline = new StoryOutline
                    {
                        Title = metadata.Title,
                        Theme = metadata.Theme,
                        Moral = metadata.Moral,
                        Setting = "",
                    },
                    DirectoryPath = dirManager.GetStoryPath(id),
                    WordCount = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length,
                    CreatedAt = metadata.CreatedAt,
                };

                var imagesInfo = await imageGenerator.GenerateAllImagesAsync(story, outputDir, ct);

                metadata.Images = imagesInfo;
                metadata.Status = StoryStatus.ImagesReady;
                await catalog.UpdateStoryAsync(metadata, ct);
                await dirManager.SaveMetadataAsync(id, metadata);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Obrázky vygenerované!");
                Console.ResetColor();
                Console.WriteLine($"   Obálka: {imagesInfo.Cover}");
                Console.WriteLine($"   Scény:  {imagesInfo.Scenes.Count} obrázkov");
                foreach (var scene in imagesInfo.Scenes)
                {
                    Console.WriteLine($"     • {Path.GetFileName(scene)}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba pri generovaní obrázkov: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }
}
