using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class StatusCommand
{
    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky (voliteľné)", Arity = ArgumentArity.ZeroOrOne };

        var command = new Command("status", "Zobrazí stav rozprávky alebo prehľad všetkých rozprávok");
        command.Add(idArgument);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetValue(idArgument);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();

                if (string.IsNullOrEmpty(id))
                {
                    await ShowAllStoriesStatus(catalog, ct);
                }
                else
                {
                    await ShowStoryDetail(catalog, id, ct);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }

    private static async Task ShowAllStoriesStatus(ICatalogManager catalog, CancellationToken ct)
    {
        var stories = await catalog.GetAllStoriesAsync(ct);

        if (stories.Count == 0)
        {
            Console.WriteLine("📭 Žiadne rozprávky v katalógu.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"  {"ID",-30} {"STAV",-22} {"SKÓRE",-8} {"DÁTUM",-12}");
        Console.WriteLine($"  {new string('─', 30)} {new string('─', 22)} {new string('─', 8)} {new string('─', 12)}");

        foreach (var story in stories.OrderByDescending(s => s.CreatedAt))
        {
            var statusDesc = StatusTracker.GetStatusDescription(story.Status);
            var score = story.QualityReport?.Score.Total.ToString() ?? "—";
            var date = story.CreatedAt.ToString("yyyy-MM-dd");

            Console.ForegroundColor = GetStatusColor(story.Status);
            Console.Write($"  {story.Id,-30} ");
            Console.Write($"{statusDesc,-22} ");
            Console.ResetColor();
            Console.Write($"{score,-8} ");
            Console.WriteLine($"{date,-12}");
        }

        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($"  Celkom: {stories.Count} rozprávok");
    }

    private static async Task ShowStoryDetail(ICatalogManager catalog, string id, CancellationToken ct)
    {
        var metadata = await catalog.GetStoryAsync(id, ct);
        if (metadata is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"❌ Rozprávka '{id}' sa nenachádza v katalógu.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {metadata.Title}");
        Console.ResetColor();
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine($"  ID:          {metadata.Id}");
        Console.WriteLine($"  Téma:        {metadata.Theme}");
        Console.WriteLine($"  Poučenie:    {metadata.Moral}");
        Console.WriteLine($"  Vytvorené:   {metadata.CreatedAt:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Cieľ:        {metadata.TargetMinutes} min");
        Console.WriteLine($"  Počet slov:  {metadata.WordCount}");

        Console.Write("  Stav:        ");
        Console.ForegroundColor = GetStatusColor(metadata.Status);
        Console.WriteLine(StatusTracker.GetStatusDescription(metadata.Status));
        Console.ResetColor();

        // Quality report
        if (metadata.QualityReport is not null)
        {
            Console.WriteLine();
            Console.WriteLine("  📊 Kvalita:");
            var score = metadata.QualityReport.Score;
            Console.WriteLine($"     Celkové skóre: {score.Total}/100 {(score.IsPassing ? "✅" : "❌")}");
            Console.WriteLine($"     Gramatika: {score.Grammar}/20 | Štýl: {score.Style}/20");
            Console.WriteLine($"     Logika: {score.LogicalConsistency}/20 | Vhodnosť: {score.AgeAppropriateness}/20 | Vzdelávanie: {score.EducationalValue}/20");
        }

        // Audio info
        if (metadata.Audio is not null)
        {
            Console.WriteLine();
            Console.WriteLine($"  🎙️  Audio: {metadata.Audio.File}");
            Console.WriteLine($"     Dĺžka: {metadata.Audio.DurationSeconds:F0}s | Hlas: {metadata.Audio.VoiceId}");
        }

        // Images info
        if (metadata.Images is not null)
        {
            Console.WriteLine();
            Console.WriteLine($"  🎨 Obrázky: {metadata.Images.Scenes.Count} scén + obálka");
        }

        // Video info
        if (metadata.Video is not null)
        {
            Console.WriteLine();
            Console.WriteLine($"  🎬 Video: {metadata.Video.File}");
            Console.WriteLine($"     Dĺžka: {metadata.Video.DurationSeconds:F0}s");
        }

        // Publications
        if (metadata.Publications.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("  📤 Publikácie:");
            foreach (var pub in metadata.Publications)
            {
                Console.WriteLine($"     • {pub.Platform}: {pub.Url} ({pub.PublishedAt:yyyy-MM-dd})");
            }
        }

        Console.WriteLine();
    }

    private static ConsoleColor GetStatusColor(StoryStatus status) => status switch
    {
        StoryStatus.FullyPublished => ConsoleColor.Green,
        StoryStatus.PublishedText or StoryStatus.PublishedAudio or StoryStatus.PublishedVideo => ConsoleColor.Cyan,
        StoryStatus.VideoReady or StoryStatus.ImagesReady or StoryStatus.AudioReady or StoryStatus.TextReady => ConsoleColor.Blue,
        StoryStatus.OutlineDraft or StoryStatus.TextDraft => ConsoleColor.Yellow,
        _ => ConsoleColor.White,
    };
}
