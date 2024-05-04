using OnnxStack.Core.Config;
using OnnxStack.StableDiffusion.Pipelines;
using ThisPresentationDoesNotExist.Settings;

namespace ThisPresentationDoesNotExist.Extensions;

public static class StableDiffusionExtensions
{
    public static IServiceCollection AddStableDiffusionPipeline(this IServiceCollection services,
        StableDiffusion settings)
    {
        services.AddSingleton<StableDiffusionPipeline>(StableDiffusionXLPipeline.CreatePipeline(settings.ModelDirectory, executionProvider: ExecutionProvider.Cuda));
        return services;
    }
}