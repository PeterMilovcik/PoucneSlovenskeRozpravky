using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class PipelineCommand
{
    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky" };

        var command = new Command("pipeline", "Spustí celý post-generačný pipeline: kontrola → audio → obrázky → video");
        command.Add(idArgument);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetRequiredValue(idArgument);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();

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

                var text = await File.ReadAllTextAsync(storyPath, ct);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"🚀 Pipeline pre '{metadata.Title}'");
                Console.ResetColor();
                Console.WriteLine("═══════════════════════════════════════\n");

                // Step 1: Review
                Console.WriteLine("┌─ KROK 1/4: Kontrola kvality");
                Console.WriteLine("│");
                try
                {
                    var pipeline = services.GetRequiredService<IReviewPipeline>();
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

                    var report = await pipeline.RunReviewAsync(story, ct);
                    metadata.QualityReport = report;

                    Console.Write("│  Skóre: ");
                    Console.ForegroundColor = report.Score.IsPassing ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"{report.Score.Total}/100 {(report.Score.IsPassing ? "✅" : "❌")}");
                    Console.ResetColor();

                    if (!report.Score.IsPassing)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("│");
                        Console.WriteLine("└─ ❌ Kontrola neprešla (skóre < 80). Pipeline zastavený.");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine("   Spustite 'rozpravky review " + id + "' pre detailnú správu.");

                        metadata.Status = StoryStatus.TextDraft;
                        await catalog.UpdateStoryAsync(metadata, ct);
                        await dirManager.SaveMetadataAsync(id, metadata);
                        return;
                    }

                    metadata.Status = StoryStatus.TextReady;
                    await catalog.UpdateStoryAsync(metadata, ct);
                    await dirManager.SaveMetadataAsync(id, metadata);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"│  ❌ Chyba: {ex.Message}");
                    Console.WriteLine("└─ Pipeline zastavený.");
                    Console.ResetColor();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("└─ ✅ Kontrola dokončená\n");
                Console.ResetColor();

                // Step 2: Audio
                Console.WriteLine("┌─ KROK 2/4: Generovanie audia");
                Console.WriteLine("│");
                try
                {
                    var audioGenerator = services.GetRequiredService<IAudioGenerator>();
                    var audioPath = Path.Combine(dirManager.GetStoryPath(id), "audio", "rozpravka.mp3");
                    Directory.CreateDirectory(Path.GetDirectoryName(audioPath)!);

                    var audioInfo = await audioGenerator.GenerateAudioAsync(text, audioPath, ct);
                    metadata.Audio = audioInfo;
                    metadata.Status = StoryStatus.AudioReady;
                    await catalog.UpdateStoryAsync(metadata, ct);
                    await dirManager.SaveMetadataAsync(id, metadata);

                    Console.WriteLine($"│  Dĺžka: {audioInfo.DurationSeconds:F0}s ({audioInfo.DurationSeconds / 60:F1} min)");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("└─ ✅ Audio dokončené\n");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"│  ⚠️  Chyba: {ex.Message}");
                    Console.WriteLine("└─ Pokračujem na ďalší krok...\n");
                    Console.ResetColor();
                }

                // Step 3: Images
                Console.WriteLine("┌─ KROK 3/4: Generovanie obrázkov");
                Console.WriteLine("│");
                try
                {
                    var imageGenerator = services.GetRequiredService<IImageGenerator>();
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

                    Console.WriteLine($"│  Obálka + {imagesInfo.Scenes.Count} scén");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("└─ ✅ Obrázky dokončené\n");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"│  ⚠️  Chyba: {ex.Message}");
                    Console.WriteLine("└─ Pokračujem na ďalší krok...\n");
                    Console.ResetColor();
                }

                // Step 4: Video
                Console.WriteLine("┌─ KROK 4/4: Vytvorenie videa");
                Console.WriteLine("│");
                try
                {
                    if (metadata.Audio is null || metadata.Images is null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("│  ⚠️  Video vyžaduje audio aj obrázky. Preskakujem.");
                        Console.WriteLine("└─\n");
                        Console.ResetColor();
                    }
                    else
                    {
                        var videoGenerator = services.GetRequiredService<IVideoGenerator>();
                        var imagePaths = new List<string> { metadata.Images.Cover };
                        imagePaths.AddRange(metadata.Images.Scenes);

                        var videoPath = Path.Combine(dirManager.GetStoryPath(id), "video", "rozpravka.mp4");
                        Directory.CreateDirectory(Path.GetDirectoryName(videoPath)!);

                        var videoInfo = await videoGenerator.GenerateVideoAsync(
                            metadata.Audio.File, imagePaths, videoPath, ct);
                        metadata.Video = videoInfo;
                        metadata.Status = StoryStatus.VideoReady;
                        await catalog.UpdateStoryAsync(metadata, ct);
                        await dirManager.SaveMetadataAsync(id, metadata);

                        Console.WriteLine($"│  Dĺžka: {videoInfo.DurationSeconds:F0}s");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("└─ ✅ Video dokončené\n");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"│  ⚠️  Chyba: {ex.Message}");
                    Console.WriteLine("└─\n");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine($"🎉 Pipeline dokončený! Stav: {StatusTracker.GetStatusDescription(metadata.Status)}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Neočakávaná chyba: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }
}
