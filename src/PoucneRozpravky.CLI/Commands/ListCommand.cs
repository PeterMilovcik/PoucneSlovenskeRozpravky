using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;

namespace PoucneRozpravky.CLI.Commands;

public static class ListCommand
{
    public static Command Create(IServiceProvider services)
    {
        var statusOption = new Option<string>("--status") { Description = "Filtrovať podľa stavu (napr. TextReady, AudioReady, FullyPublished)" };

        var command = new Command("list", "Zobrazí zoznam všetkých rozprávok");
        command.Add(statusOption);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var statusFilter = parseResult.GetValue(statusOption);
            try
            {
                var catalog = services.GetRequiredService<ICatalogManager>();
                var stories = await catalog.GetAllStoriesAsync(ct);

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (!Enum.TryParse<StoryStatus>(statusFilter, ignoreCase: true, out var filterStatus))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"❌ Neznámy stav '{statusFilter}'.");
                        Console.ResetColor();
                        Console.WriteLine("   Dostupné stavy:");
                        foreach (var s in Enum.GetValues<StoryStatus>())
                        {
                            Console.WriteLine($"     • {s} — {StatusTracker.GetStatusDescription(s)}");
                        }
                        return;
                    }

                    stories = stories.Where(s => s.Status == filterStatus).ToList();
                }

                if (stories.Count == 0)
                {
                    Console.WriteLine("📭 Žiadne rozprávky nenájdené.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"  {"ID",-30} {"NÁZOV",-25} {"STAV",-22} {"SKÓRE",-8} {"DÁTUM",-12}");
                Console.WriteLine($"  {new string('─', 30)} {new string('─', 25)} {new string('─', 22)} {new string('─', 8)} {new string('─', 12)}");

                foreach (var story in stories.OrderByDescending(s => s.CreatedAt))
                {
                    var title = story.Title.Length > 24
                        ? story.Title[..21] + "..."
                        : story.Title;
                    var statusDesc = StatusTracker.GetStatusDescription(story.Status);
                    var score = story.QualityReport?.Score.Total.ToString() ?? "—";
                    var date = story.CreatedAt.ToString("yyyy-MM-dd");

                    Console.Write($"  {story.Id,-30} ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{title,-25} ");
                    Console.ResetColor();
                    Console.Write($"{statusDesc,-22} ");
                    Console.Write($"{score,-8} ");
                    Console.WriteLine($"{date,-12}");
                }

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"  Celkom: {stories.Count} rozprávok");
                if (!string.IsNullOrEmpty(statusFilter))
                    Console.WriteLine($"  Filter: {statusFilter}");
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
}
