namespace ThisPresentationDoesNotExist.Services;

public interface ISlideGenerationService
{
    Task<IResult> GenerateSlide(string prompt);
}