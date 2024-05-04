using ThisPresentationDoesNotExist.Models;

namespace ThisPresentationDoesNotExist.Repositories;

public interface IPromptRepository
{
    public Prompt GetPrompt(int slide);
    public IEnumerable<Prompt> GetPrompts();
    public IEnumerable<ImagePrompt> GetImagePrompts();
}