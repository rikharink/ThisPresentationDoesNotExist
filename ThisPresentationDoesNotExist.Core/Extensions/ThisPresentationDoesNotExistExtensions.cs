using Serilog;
using ThisPresentationDoesNotExist.Core.Repositories.Implementations;
using ThisPresentationDoesNotExist.Core.Services.Implementations;
using ThisPresentationDoesNotExist.Repositories;
using ThisPresentationDoesNotExist.Services;

namespace ThisPresentationDoesNotExist.Core.Extensions;

public static class ThisPresentationDoesNotExistExtensions
{
    public static IServiceCollection AddThisPresentationDoesNotExist(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSerilog();
        
        services.AddLLama(configuration
            .GetRequiredSection(nameof(LLama))
            .Get<LLama>()!);
        
        var stableDiffusionSettings = configuration.GetRequiredSection("StableDiffusion").Get<StableDiffusion>()!;
        services.AddHttpClient<IImageGenerationService, StableDiffusionWebUiImageGenerationService>(client =>
        {
            client.BaseAddress = stableDiffusionSettings.WebUiUrl;
        });

        services.AddSingleton<IPromptRepository, JsonPromptRepository>();
        services.AddSingleton<ISlideImageRepository, CachingSlideImageRepository>();
        services.AddSingleton<ISlideGenerationService, SemanticKernelSlideGenerationService>();
        return services;
    }
    
}