using System.Reflection;
using ThisPresentationDoesNotExist.Helpers;
using ThisPresentationDoesNotExist.Repositories;

namespace ThisPresentationDoesNotExist.Core.Repositories.Implementations;

public class JsonPromptRepository : IPromptRepository
{
    public async Task<Prompt> GetPrompt(int slide)
    {
        var prompts = (await GetPrompts()).ToArray();
        if (slide < 1 || slide > prompts.Length)
        {
            return new Prompt(
                "Please summarize some funny reasons why the slide doesn't exist yet and title it: 404 - This Slide Does Not Exist.");
        }

        return prompts[slide - 1];
    }
    
    public async Task<string> GetNotes(int slide)
    {
        var prompts = (await GetPrompts()).ToArray();
        if (slide < 1 || slide > prompts.Length)
        {
            return string.Empty;
        }
        return prompts[slide - 1].Notes;
    }

    public async Task<IEnumerable<Prompt>> GetPrompts()
    {
        await using var stream =
            EmbeddedResourcesHelpers.GetEmbeddedResourceStreamByFilename(Assembly.GetExecutingAssembly(),
                "prompts.json");
        using var reader = new StreamContent(stream);
        var json = await reader.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Prompt[]>(json) ?? [];
    }

    public async Task<IEnumerable<ImagePrompt>> GetImagePrompts()
    {
        return (await GetPrompts())
            .Where(p => p.ImagePrompt != null)
            .Select(p => p.ImagePrompt!);
    }
}