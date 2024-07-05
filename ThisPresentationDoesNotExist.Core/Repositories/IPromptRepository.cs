namespace ThisPresentationDoesNotExist.Repositories;

public interface IPromptRepository
{
    public Task<Prompt> GetPrompt(int slide);
    public Task<string> GetNotes(int slide);
    public Task<IEnumerable<Prompt>> GetPrompts();
    public Task<IEnumerable<ImagePrompt>> GetImagePrompts();
}