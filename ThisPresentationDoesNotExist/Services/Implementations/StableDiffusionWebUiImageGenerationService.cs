using System.Text.Json;
using ThisPresentationDoesNotExist.Models;

namespace ThisPresentationDoesNotExist.Services.Implementations;

public class StableDiffusionWebUiImageGenerationService(HttpClient client) : IImageGenerationService
{
    private static readonly JsonSerializerOptions Options = new();

    public async Task<byte[]> GenerateImageAsync(ImagePrompt prompt, CancellationToken cancellationToken = default)
    {
        var txt2ImgRequest = new TextToImageRequest
        {
            Prompt = prompt.Positive,
            NegativePrompt = prompt.Negative,
            Width = prompt.Width,
            Height = prompt.Height,
            Steps = prompt.Steps
        };
        
        var response = await client.PostAsJsonAsync("/sdapi/v1/txt2img", txt2ImgRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to generate image: {response.ReasonPhrase}");
        }
        
        var txt2ImgResponse = await response.Content.ReadFromJsonAsync<TextToImageResponse>(Options, cancellationToken);
        if(txt2ImgResponse is null || txt2ImgResponse.Images.Length == 0)
        {
            throw new InvalidOperationException("No images returned");
        }
        return Convert.FromBase64String(txt2ImgResponse.Images[0]);
    }

    public Task LoadPipelineAsync()
    {
        //NOOP
        return Task.CompletedTask;
    }

    public Task UnloadPipelineAsync()
    {
        //NOOP
        return Task.CompletedTask;
    }
}