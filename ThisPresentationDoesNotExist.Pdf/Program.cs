using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Markdown;
using Serilog;
using ThisPresentationDoesNotExist.Core.Extensions;
using ThisPresentationDoesNotExist.Helpers;
using ThisPresentationDoesNotExist.Repositories;
using ThisPresentationDoesNotExist.Services;

QuestPDF.Settings.License = LicenseType.Community;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

ConfigurationManager configuration = new();

configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// Load normal.png as a stream from EmbeddedResources
var services = new ServiceCollection();
services.AddThisPresentationDoesNotExist(configuration);
var serviceProvider = services.BuildServiceProvider();

var promptRepository = serviceProvider.GetRequiredService<IPromptRepository>();
var prompts = (await promptRepository.GetPrompts()).ToImmutableList();

var imageRepository = serviceProvider.GetRequiredService<ISlideImageRepository>();
await imageRepository.PreloadImages();
var slideGenerator = serviceProvider.GetRequiredService<ISlideGenerationService>();

byte[] titleImage;
byte[] normalImage;

await using var title =
    EmbeddedResourcesHelpers.GetEmbeddedResourceStreamByFilename(Assembly.GetExecutingAssembly(), "title.png");
await using var normal =
    EmbeddedResourcesHelpers.GetEmbeddedResourceStreamByFilename(Assembly.GetExecutingAssembly(), "normal.png");
{
    titleImage = title.ToByteArray();
    normalImage = normal.ToByteArray();
}

var slides = new List<(string? slideMarkdown, byte[]? image)>();
foreach (var prompt in prompts)
{
    (string? markdown, byte[]? image) slide = (null, null);
        
    if (prompt.TextPrompt is not null)
    {
        slide.markdown = await slideGenerator.GenerateSlideNonStreaming(prompt.TextPrompt);
    }

    if (prompt.ImagePrompt is not null)
    {
        slide.image = await imageRepository.GetImage(prompt.ImagePrompt);
    }
    slides.Add(slide);
}

Document
    .Create(container =>
{
    container.Page(page =>
    {
        page.Size(1280, 720, Unit.Point);
        page.Background().Image(titleImage).FitArea().UseOriginalImage();
        page.Content()
            .AlignCenter()
            .AlignMiddle()
            .Background(Color.FromHex("#ffffff"))
            .Column(column =>
            {
                column.Item().PaddingTop(40).PaddingHorizontal(80).AlignCenter().Text("404").Bold().FontSize(32);
                column.Item().PaddingBottom(40).PaddingHorizontal(80).AlignCenter()
                    .Text("this presentation does not exist").FontSize(16);
            });
    });

    foreach (var slide in slides)
    {
        container.Page(page =>
        {
            page.Size(1280, 720, Unit.Point);
            page.Background().Image(normalImage).FitArea().UseOriginalImage();
            page.Content()
                .AlignCenter()
                .AlignMiddle()
                .Row(row =>
                {
                    if (slide.slideMarkdown is not null)
                    {
                        row.RelativeItem()
                            .ExtendVertical().ExtendHorizontal()
                            .AlignCenter().AlignMiddle()
                            .PaddingHorizontal(80)
                            .Markdown(slide.slideMarkdown);
                    }

                    if (slide.image is not null)
                    {
                        row.RelativeItem()
                            .ExtendVertical().ExtendHorizontal()
                            .AlignCenter().AlignMiddle()
                            .Padding(100)
                            .Image(slide.image).FitArea().UseOriginalImage();
                    }
                });
        });
    }
}).GeneratePdfAndShow();