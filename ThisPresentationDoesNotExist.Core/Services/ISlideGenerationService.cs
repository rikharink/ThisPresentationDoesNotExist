namespace ThisPresentationDoesNotExist.Services;

public interface ISlideGenerationService
{
    Task<string> GenerateSlideNonStreaming(string prompt);
    IAsyncEnumerable<string> GenerateSlide(string prompt);
    void AddResponseToHistory(string response);
    void ResetHistory();
}