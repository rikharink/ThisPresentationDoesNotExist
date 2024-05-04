using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OnnxStack.StableDiffusion.Config;
using OnnxStack.StableDiffusion.Pipelines;
using ThisPresentationDoesNotExist.Models;

namespace ThisPresentationDoesNotExist.Services.Implementations;

public class StableDiffusionImageGenerationService(ILogger<StableDiffusionImageGenerationService> logger, IPipeline pipeline) : IImageGenerationService
{
    public async Task<byte[]> GenerateImageAsync(ImagePrompt prompt, CancellationToken cancellationToken = default)
    {
        await pipeline.LoadAsync();
        var promptOptions = new PromptOptions
        {
            Prompt = prompt.Positive,
            NegativePrompt = prompt.Negative,
        };

        var schedulerOptions = new SchedulerOptions
        {
            Width = prompt.Width,
            Height = prompt.Height,
            InferenceSteps = prompt.Steps
        };
        var result =
            await pipeline.GenerateImageAsync(promptOptions, schedulerOptions, cancellationToken: cancellationToken);
        
        if (result is null)
        {
            logger.LogError("Failed to generate image returning empty byte[]");
            return [];
        }
        
        await pipeline.UnloadAsync();
        return await result.GetImageBytesAsync();
    }
}