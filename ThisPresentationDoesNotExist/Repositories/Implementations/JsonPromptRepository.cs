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
                "Generate a slide summarizing why this slide does not exist (yet). The title for this specific slide should be: 404 - This Slide Does Not Exist. Please think of some funny reasons why the slide doesn't exist yet and include those as bullet points.");
        }

        return prompts[slide - 1];
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