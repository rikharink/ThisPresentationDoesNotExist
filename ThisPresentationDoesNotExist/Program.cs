using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using ThisPresentationDoesNotExist.Hubs;
using ThisPresentationDoesNotExist.Models;
using ThisPresentationDoesNotExist.Repositories;
using ThisPresentationDoesNotExist.Results;
using ThisPresentationDoesNotExist.Services;
using ThisPresentationDoesNotExist.Core.Extensions;

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
    builder.Services.AddThisPresentationDoesNotExist(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddSignalR();

    var app = builder.Build();
    app.UseHttpsRedirection();

    var rewriteOptions = new RewriteOptions()
        .AddRewrite(@"^\d+$", "slide.html", true);

    await PreloadImages(app.Services);

    app.UseRewriter(rewriteOptions);
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapGet("/api/slide/{id:int}/notes",
        (int id) => Results.Ok(app.Services.GetRequiredService<IPromptRepository>().GetNotes(id)));

    app.MapGet("/api/slide/{id:int}/prompts",
        async (int id) => Results.Ok(await app.Services.GetRequiredService<IPromptRepository>().GetPrompt(id)));

    app.MapGet("/api/slide/text/{prompt}",
        (string prompt) =>
        {
            var slideGenerationService = app.Services.GetRequiredService<ISlideGenerationService>();
            return new SemanticKernelResult(slideGenerationService.GenerateSlide(prompt), slideGenerationService);
        });

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