using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using ThisPresentationDoesNotExist.Extensions;
using ThisPresentationDoesNotExist.Hubs;
using ThisPresentationDoesNotExist.Models;
using ThisPresentationDoesNotExist.Repositories;
using ThisPresentationDoesNotExist.Repositories.Implementations;
using ThisPresentationDoesNotExist.Services;
using ThisPresentationDoesNotExist.Services.Implementations;
using ThisPresentationDoesNotExist.Settings;

async Task PreloadImages(IServiceProvider appServices)
{
    var repository = appServices.GetRequiredService<ISlideImageRepository>();
    await repository.PreloadImages();
    Log.Logger.Information("Preloaded all images");
}

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
try
{
    Log.Information("Starting web application");
    builder.Services.AddLLama(builder.Configuration
        .GetRequiredSection(nameof(LLama))
        .Get<LLama>()!);

    var stableDiffusionSettings = builder.Configuration.GetRequiredSection("StableDiffusion").Get<StableDiffusion>()!;
    builder.Services.AddHttpClient<IImageGenerationService, StableDiffusionWebUiImageGenerationService>(client =>
    {
        client.BaseAddress = stableDiffusionSettings.WebUiUrl;
    });

    builder.Services.AddSingleton<IPromptRepository, JsonPromptRepository>();
    builder.Services.AddSingleton<ISlideImageRepository, CachingSlideImageRepository>();
    builder.Services.AddSingleton<ISlideGenerationService, SemanticKernelSlideGenerationService>();
    builder.Services.AddControllers();
    builder.Services.AddSerilog();
    builder.Services.AddSignalR();

    var app = builder.Build();
    app.UseHttpsRedirection();

    var rewriteOptions = new RewriteOptions()
        .AddRewrite(@"^\d+$", "slide.html", true);

    await PreloadImages(app.Services);

    app.UseRewriter(rewriteOptions);
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapGet("/api/slide/{id:int}/notes", (int id) => Results.Ok(app.Services.GetRequiredService<IPromptRepository>().GetNotes(id)));
    
    app.MapGet("/api/slide/{id:int}/prompts",
        (int id) => Results.Ok(app.Services.GetRequiredService<IPromptRepository>().GetPrompt(id)));

    app.MapGet("/api/slide/text/{prompt}",
        async (string prompt) =>
            await app.Services.GetRequiredService<ISlideGenerationService>().GenerateSlide(prompt));
    app.MapGet("/api/slide/image/{positivePrompt}/{negativePrompt}",
        async (string positivePrompt, string negativePrompt = "", [FromQuery] int width = 1024,
            [FromQuery] int height = 1024, [FromQuery] int steps = 30) =>
        {
            var prompt = new ImagePrompt(positivePrompt, negativePrompt, width, height, steps);
            var img = await app.Services.GetRequiredService<ISlideImageRepository>().GetImage(prompt);
            return Results.File(img, "image/png");
        });

    app.MapGet("/api/slide/text/reset", () =>
    {
        app.Services.GetRequiredService<ISlideGenerationService>().ResetHistory();
        return Results.Ok();
    });

    app.MapHub<PresentationHub>("/presentationHub");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}