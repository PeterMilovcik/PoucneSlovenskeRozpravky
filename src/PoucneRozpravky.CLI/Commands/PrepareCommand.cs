using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Core.Models;
using PoucneRozpravky.Preparation;

namespace PoucneRozpravky.CLI.Commands;

public static class PrepareCommand
{
    public static Command Create(IServiceProvider services)
    {
        var minutesOption = new Option<int>("--minutes") { Description = "Cieľová dĺžka rozprávky v minútach", DefaultValueFactory = _ => 12 };

        var command = new Command("prepare", "Pripraví novú rozprávku — vyberie tému a vytvorí adresár");
        command.Add(minutesOption);

        command.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            var minutes = parseResult.GetValue(minutesOption);
            try
            {
                var themeSelector = services.GetRequiredService<IThemeSelector>();
                var catalog = services.GetRequiredService<ICatalogManager>();
                var dirManager = services.GetRequiredService<StoryDirectoryManager>();

                Console.WriteLine("🔍 Hľadám dostupné témy...\n");

                // Load themes file to display top 5 available
                var themesPath = Path.GetFullPath("config/themes.json");
                if (File.Exists(themesPath))
                {
                    var json = await File.ReadAllTextAsync(themesPath, ct);
                    var collection = JsonSerializer.Deserialize<ThemeCollection>(json);
                    var usedThemes = await catalog.GetUsedThemesAsync(ct);

                    var available = (collection?.Themes ?? [])
                        .Where(t => !usedThemes.Contains(t.Title, StringComparer.OrdinalIgnoreCase))
                        .Take(5)
                        .ToList();

                    if (available.Count > 0)
                    {
                        Console.WriteLine("📚 Dostupné témy:");
                        Console.WriteLine(new string('─', 60));
                        for (int i = 0; i < available.Count; i++)
                        {
                            var t = available[i];
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($"  {i + 1}. {t.Title}");
                            Console.ResetColor();
                            Console.WriteLine($" [{t.Category}]");
                            Console.WriteLine($"     {t.Description}");
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"     Morál: {t.ExampleMoral}");
                            Console.ResetColor();
                            Console.WriteLine();
                        }
                    }
                }

                // Select a unique theme
                var (theme, moral) = await themeSelector.SelectUniqueThemeAsync(ct);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Vybraná téma: {theme}");
                Console.WriteLine($"   Poučenie: {moral}");
                Console.ResetColor();

                // Create story directory with today's date
                var slug = theme
                    .ToLowerInvariant()
                    .Replace(" + ", "-a-")
                    .Replace(' ', '-')
                    .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                    .Replace("ó", "o").Replace("ú", "u").Replace("ý", "y")
                    .Replace("ä", "a").Replace("ô", "o").Replace("ľ", "l")
                    .Replace("š", "s").Replace("č", "c").Replace("ť", "t")
                    .Replace("ž", "z").Replace("ň", "n").Replace("ď", "d")
                    .Replace("ř", "r");
                slug = string.Concat(slug.Where(c => char.IsLetterOrDigit(c) || c == '-'));

                var storyId = $"{DateTime.Today:yyyy-MM-dd}-{slug}";
                dirManager.CreateStoryDirectory(storyId);

                // Create initial metadata
                var metadata = new StoryMetadata
                {
                    Id = storyId,
                    Title = theme,
                    Theme = theme,
                    Moral = moral,
                    CreatedAt = DateTimeOffset.Now,
                    TargetMinutes = minutes,
                    Status = StoryStatus.OutlineDraft,
                };

                await catalog.AddStoryAsync(metadata, ct);
                await dirManager.SaveMetadataAsync(storyId, metadata);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("📁 Adresár vytvorený:");
                Console.ResetColor();
                Console.WriteLine($"   rozpravky/{storyId}/");
                Console.WriteLine();
                Console.WriteLine($"📝 Ďalší krok: Vygenerujte osnovu a text rozprávky pre ID '{storyId}'");
                Console.WriteLine($"   Téma: {theme}");
                Console.WriteLine($"   Poučenie: {moral}");
                Console.WriteLine($"   Cieľová dĺžka: {minutes} min (~{minutes * 140} slov)");
                Console.WriteLine($"   Uložte text do: rozpravky/{storyId}/rozpravka.md");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"❌ Chyba pri príprave: {ex.Message}");
                Console.ResetColor();
            }
        });

        return command;
    }
}
