using ThisPresentationDoesNotExist.Models;

namespace ThisPresentationDoesNotExist.Services;

public interface IImageGenerationService
{
    Task<byte[]> GenerateImageAsync(ImagePrompt prompt, CancellationToken cancellationToken = default);
    Task LoadPipelineAsync();
    Task UnloadPipelineAsync();
}