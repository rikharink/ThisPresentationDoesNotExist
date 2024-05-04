using System.Collections.Immutable;
using System.Reflection;
using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using LLamaSharp.SemanticKernel.TextCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.TextGeneration;
using Serilog;

namespace ThisPresentationDoesNotExist.Extensions;

public static class SemanticKernelExtensions
{
    public static IServiceCollection AddLLama(this IServiceCollection services, Settings.LLama settings)
    {
        SetupNativeLibrary(settings.NativeLibraryDirectory);
        services.AddSingleton<Kernel>(_ =>
        {
            var builder = Kernel.CreateBuilder();
            builder.Services.AddSerilog();
            builder.Services.AddSingleton<ILLamaExecutor, StatelessExecutor>(_ =>
            {
                var parameters = new ModelParams(settings.ModelFile);
                var model = LLamaWeights.LoadFromFile(parameters);
                return new DisposableStatelessExecutor(model, parameters);
            });
            builder.Services.AddKeyedSingleton<ITextGenerationService>("local-llama",
                (provider, _) => new LLamaSharpTextCompletion(provider.GetRequiredService<ILLamaExecutor>()));

            return builder.Build();
        });
        return services;
    }

    private static void SetupNativeLibrary(string nativeLibraryDirectory)
    {
        NativeLibraryConfig.Instance.WithLibrary(Path.Combine(nativeLibraryDirectory, "libllama.so"), null);
        NativeLibraryConfig.Instance.WithLogCallback((level, message) =>
        {
            switch (level)
            {
                case LLamaLogLevel.Error:
                    Log.Logger.Error(message);
                    break;
                case LLamaLogLevel.Warning:
                    Log.Logger.Warning(message);
                    break;
                case LLamaLogLevel.Info:
                    Log.Logger.Information(message);
                    break;
                case LLamaLogLevel.Debug:
                    Log.Logger.Debug(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        });
        NativeApi.llama_empty_call();
    }
}