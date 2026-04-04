using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class ReviewCommand
{
    public static Command Create(IServiceProvider services)
    {
        var idArgument = new Argument<string>("id") { Description = "Identifikátor rozprávky (napr. 2024-01-15-maly-hrdina)" };

        var command = new Command("review", "Spustí kontrolu kvality rozprávky (gramatika → štýl → obsah)");
        command.Add(idArgument);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var id = parseResult.GetRequiredValue(idArgument);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();
                var pipeline = services.GetRequiredService<IReviewPipeline>();

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

                Console.WriteLine($"📖 Načítavam rozprávku '{id}'...");
                var text = await File.ReadAllTextAsync(storyPath, ct);

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

                Console.WriteLine("🔍 Spúšťam kontrolu kvality...\n");

                Console.WriteLine("  ⏳ Gramatika...");
                Console.WriteLine("  ⏳ Štýl...");
                Console.WriteLine("  ⏳ Obsah...");

                var report = await pipeline.RunReviewAsync(story, ct);

                Console.WriteLine();
                DisplayQualityReport(report);

                // Save to metadata
                metadata.QualityReport = report;
                if (report.Score.IsPassing)
                    metadata.Status = StoryStatus.TextReady;

                await catalog.UpdateStoryAsync(metadata, ct);
                await dirManager.SaveMetadataAsync(id, metadata);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n💾 Výsledky uložené do metadata.json");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba pri kontrole: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }

    internal static void DisplayQualityReport(QualityReport report)
    {
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("         SPRÁVA O KVALITE");
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();

        var score = report.Score;
        WriteScoreLine("Gramatika", score.Grammar);
        WriteScoreLine("Štýl", score.Style);
        WriteScoreLine("Logická konzistencia", score.LogicalConsistency);
        WriteScoreLine("Vhodnosť pre deti", score.AgeAppropriateness);
        WriteScoreLine("Vzdelávacia hodnota", score.EducationalValue);

        Console.WriteLine("───────────────────────────────────────");
        Console.Write("  CELKOVÉ SKÓRE:          ");
        Console.ForegroundColor = score.IsPassing ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"{score.Total}/100");
        Console.ResetColor();

        Console.Write("  STATUS:                 ");
        if (score.IsPassing)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ PREŠLO");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ NEPREŠLO");
        }
        Console.ResetColor();
        Console.WriteLine();

        if (score.Issues.Count > 0)
        {
            Console.WriteLine("⚠️  Problémy:");
            foreach (var issue in score.Issues)
            {
                var icon = issue.Severity switch
                {
                    IssueSeverity.Error => "🔴",
                    IssueSeverity.Warning => "🟡",
                    _ => "🔵"
                };
                Console.WriteLine($"  {icon} [{issue.Category}] {issue.Description}");
                if (issue.Location is not null)
                    Console.WriteLine($"     Miesto: {issue.Location}");
            }
        }
    }

    private static void WriteScoreLine(string category, int score)
    {
        Console.Write($"  {category,-25} ");
        Console.ForegroundColor = score >= 16 ? ConsoleColor.Green
            : score >= 12 ? ConsoleColor.Yellow
            : ConsoleColor.Red;
        Console.WriteLine($"{score}/20");
        Console.ResetColor();
    }
}
