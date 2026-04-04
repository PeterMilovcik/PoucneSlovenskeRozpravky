using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoucneRozpravky.Audio;
using PoucneRozpravky.Catalog;
using PoucneRozpravky.CLI.Commands;
using PoucneRozpravky.Core.Interfaces;
using PoucneRozpravky.Images;
using PoucneRozpravky.Preparation;
using PoucneRozpravky.Publisher;
using PoucneRozpravky.Review;
using PoucneRozpravky.Video;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(
    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "config", "appsettings.json")),
    optional: true,
    reloadOnChange: false);

// Also try from CWD-relative path (works when run from repo root)
builder.Configuration.AddJsonFile(
    Path.GetFullPath("config/appsettings.json"),
    optional: true,
    reloadOnChange: false);

var config = builder.Configuration;

// Catalog & Directory
builder.Services.AddSingleton<ICatalogManager>(sp =>
    new CatalogManager(config["Paths:CatalogFile"]));
builder.Services.AddSingleton(sp =>
    new StoryDirectoryManager());

// Preparation
builder.Services.AddSingleton<IThemeSelector>(sp =>
    new ThemeSelector(
        sp.GetRequiredService<ICatalogManager>(),
        config["Paths:ThemesFile"] ?? "config/themes.json"));
builder.Services.AddSingleton<IUniquenessChecker>(sp =>
    new UniquenessChecker(sp.GetRequiredService<ICatalogManager>()));
builder.Services.AddSingleton<MetadataExtractor>();

// Review
var ltOptions = new LanguageToolOptions
{
    BaseUrl = config["LanguageTool:BaseUrl"] ?? "http://localhost:8010/v2",
    Language = config["LanguageTool:Language"] ?? "sk"
};
builder.Services.AddSingleton(ltOptions);
builder.Services.AddSingleton<IGrammarChecker>(sp =>
    new GrammarChecker(new HttpClient(), ltOptions, sp.GetRequiredService<ILogger<GrammarChecker>>()));

var sgOptions = new StyleGuideOptions
{
    AverageSentenceLengthMin = double.TryParse(config["StyleGuide:AverageSentenceLength:Min"], out var aslMin) ? aslMin : 8,
    AverageSentenceLengthMax = double.TryParse(config["StyleGuide:AverageSentenceLength:Max"], out var aslMax) ? aslMax : 12,
    MaxSentenceLength = int.TryParse(config["StyleGuide:MaxSentenceLength"], out var msl) ? msl : 25,
};
builder.Services.AddSingleton<IStyleAnalyzer>(new StyleAnalyzer(sgOptions));

var crOptions = new ContentReviewOptions();
builder.Services.AddSingleton<IContentReviewer>(new ContentReviewer(crOptions));

builder.Services.AddSingleton<IReviewPipeline, ReviewPipeline>();

// Audio
var elevenLabsOptions = new ElevenLabsOptions
{
    ApiKey = config["ElevenLabs:ApiKey"] ?? Environment.GetEnvironmentVariable("ELEVENLABS_API_KEY") ?? "CONFIGURE_ME",
    BaseUrl = config["ElevenLabs:BaseUrl"] ?? "https://api.elevenlabs.io/v1",
    VoiceId = config["ElevenLabs:VoiceId"] ?? "CONFIGURE_ME",
    ModelId = config["ElevenLabs:ModelId"] ?? "eleven_multilingual_v2",
};
builder.Services.AddSingleton(elevenLabsOptions);
builder.Services.AddSingleton<IAudioGenerator>(sp =>
    new ElevenLabsAudioGenerator(elevenLabsOptions, new HttpClient()));

// Images
var dallEOptions = new DallEOptions
{
    ApiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "CONFIGURE_ME",
    Model = config["OpenAI:DallEModel"] ?? "dall-e-3",
    ImageSize = config["OpenAI:ImageSize"] ?? "1024x1024",
    ImageQuality = config["OpenAI:ImageQuality"] ?? "standard",
    ImageStyle = config["OpenAI:ImageStyle"] ?? "natural",
};
builder.Services.AddSingleton(dallEOptions);
builder.Services.AddSingleton<IImageGenerator>(sp =>
    new DallEImageGenerator(dallEOptions, new HttpClient()));

// Video
var videoOptions = new VideoOptions
{
    FfmpegPath = config["FFmpeg:Path"] ?? "ffmpeg",
    Resolution = config["FFmpeg:VideoResolution"] ?? "1920x1080",
};
builder.Services.AddSingleton(videoOptions);
builder.Services.AddSingleton<IVideoGenerator>(new SlideshowVideoGenerator(videoOptions));

// Publisher
var pubOptions = new PublisherOptions();
builder.Services.AddSingleton(pubOptions);
builder.Services.AddSingleton(new PublisherFactory(pubOptions));

using var host = builder.Build();

var rootCommand = new RootCommand("Nástroj pre správu poučných slovenských rozprávok");

var sp = host.Services;
rootCommand.Add(PrepareCommand.Create(sp));
rootCommand.Add(ReviewCommand.Create(sp));
rootCommand.Add(AudioCommand.Create(sp));
rootCommand.Add(ImagesCommand.Create(sp));
rootCommand.Add(VideoCommand.Create(sp));
rootCommand.Add(PublishCommand.Create(sp));
rootCommand.Add(StatusCommand.Create(sp));
rootCommand.Add(ListCommand.Create(sp));
rootCommand.Add(PipelineCommand.Create(sp));

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();
