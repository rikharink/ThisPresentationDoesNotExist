using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using ThisPresentationDoesNotExist.Results;

namespace ThisPresentationDoesNotExist.Services.Implementations;

public class OllamaSlideGenerationService(
    ILogger<OllamaSlideGenerationService> logger, Kernel kernel) : ISlideGenerationService
{
    public async Task<IResult> GenerateSlide(string prompt)
    {
        logger.LogInformation("Prompt: {Prompt}", prompt);
        using StreamReader reader = new(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ThisPresentationDoesNotExist.Prompts.writeSlide.prompt.yaml")!);
        var writeSlide = kernel.CreateFunctionFromPromptYaml(
            await reader.ReadToEndAsync(),
            promptTemplateFactory: new HandlebarsPromptTemplateFactory()
        );
        return new OllamaResult(kernel, writeSlide, prompt);
    }
}