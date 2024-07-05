namespace ThisPresentationDoesNotExist.Services;

public interface IImageGenerationService
{
    Task<byte[]> GenerateImageAsync(ImagePrompt prompt, CancellationToken cancellationToken = default);
}