namespace ThisPresentationDoesNotExist.Repositories;

public interface ISlideImageRepository
{
    Task PreloadImages();
    Task<byte[]> GetImage(ImagePrompt prompt);
}