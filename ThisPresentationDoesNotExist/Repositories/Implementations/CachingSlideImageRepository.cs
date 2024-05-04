using ThisPresentationDoesNotExist.Helpers;
using ThisPresentationDoesNotExist.Models;
using ThisPresentationDoesNotExist.Services;

namespace ThisPresentationDoesNotExist.Repositories.Implementations;

public class CachingSlideImageRepository(
    ILogger<CachingSlideImageRepository> logger,
    IPromptRepository promptRepository,
    IImageGenerationService imageGenerationService) : ISlideImageRepository
{
    private readonly Dictionary<ImagePrompt, byte[]> _imageCache = new();
    public async Task PreloadImages()
    {
        var imagePrompts = promptRepository.GetImagePrompts().ToList();
        var imageTotal = imagePrompts.Count;
        var imageCount = 1;
        logger.LogInformation("Preloading {Count} images", imageTotal);
        CleanCache(imagePrompts.Select(p => p.GetPromptHash()).ToArray());

        foreach (var prompt in imagePrompts)
        {
            logger.LogInformation("Start preloading image [{Count}/{Total}]", imageCount, imageTotal);
            if (await TryGetImageFromDisk(prompt) is (true, var image))
            {
                logger.LogInformation("Image found on disk");
                _imageCache[prompt] = image!;
            }
            else
            {
                image = await imageGenerationService.GenerateImageAsync(prompt);
                logger.LogInformation("Image generated");
                await CacheImageOnDisk(prompt, image);
            }

            _imageCache[prompt] = image!;
            imageCount++;
        }
    }
    
    public async Task<byte[]> GetImage(ImagePrompt prompt)
    {
        if (_imageCache.TryGetValue(prompt, out var image))
        {
            logger.LogInformation("Image found in cache for prompt: {Prompt}", prompt);
            return image;
        }

        logger.LogInformation("Image not found in cache, generating image for prompt: {Prompt}", prompt);
        image = await imageGenerationService.GenerateImageAsync(prompt);
        _imageCache[prompt] = image;
        return image;
    }

    private void CleanCache(IReadOnlyList<uint> hashes)
    {
        var cacheDir = FileSystem.GetCacheDirectory();
        var files = Directory.GetFiles(cacheDir);
        foreach (var file in files)
        {
            if (hashes.Contains(uint.Parse(Path.GetFileNameWithoutExtension(file))))
            {
                continue;
            }

            logger.LogInformation("Removing cached image {File} from disk", file);
            File.Delete(file);
        }
    }

    private async Task CacheImageOnDisk(ImagePrompt prompt, byte[] image)
    {
        var path = Path.Combine(FileSystem.GetCacheDirectory(), $"{prompt.GetPromptHash()}.png");
        logger.LogInformation("Caching image for prompt {Prompt} at {Path}", prompt.Positive, path);
        await File.WriteAllBytesAsync(path, image);
    }

    private static async Task<(bool, byte[]?)> TryGetImageFromDisk(ImagePrompt prompt)
    {
        var path = Path.Combine(FileSystem.GetCacheDirectory(), $"{prompt.GetPromptHash()}.png");
        return !File.Exists(path)
            ? (false, null)
            : (true, await File.ReadAllBytesAsync(path));
    }
}