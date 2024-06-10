using System.Text.Json;
using ThisPresentationDoesNotExist.Models;

namespace ThisPresentationDoesNotExist.Repositories.Implementations;

public class JsonPromptRepository : IPromptRepository
{
    public Prompt GetPrompt(int slide)
    {
        var prompts = GetPrompts().ToArray();
        if (slide < 1 || slide > prompts.Length)
        {
            return new Prompt(
                "Please summarize some funny reasons why the slide doesn't exist yet and title it: 404 - This Slide Does Not Exist.");
        }

        return prompts[slide - 1];
    }
    
    public string GetNotes(int slide)
    {
        var prompts = GetPrompts().ToArray();
        if (slide < 1 || slide > prompts.Length)
        {
            return string.Empty;
        }
        return prompts[slide - 1].Notes;
    }

    public IEnumerable<Prompt> GetPrompts()
    {
        return JsonSerializer.Deserialize<Prompt[]>(File.ReadAllText("prompts.json")) ?? [];
    }

    public IEnumerable<ImagePrompt> GetImagePrompts()
    {
        return GetPrompts()
            .Where(p => p.ImagePrompt != null)
            .Select(p => p.ImagePrompt!);
    }
}