
using System.Text;

namespace ThisPresentationDoesNotExist.Models;



public record ImagePrompt(
    string Positive,
    string Negative = ImagePrompt.DefaultNegative,
    int Width = 1024,
    int Height = 1024,
    int Steps = 30)
{
    private const string DefaultNegative = "no clothes, nudity, nsfw, nude, sexual, deformed iris, deformed pupils, text, cropped, out of frame, worst quality, low quality, jpeg artifacts, ugly, duplicate, morbid, mutilated, extra fingers, mutated hands, poorly drawn hands, poorly drawn face, mutation, deformed, blurry, dehydrated, bad anatomy, bad proportions, extra limbs, cloned face, disfigured, gross proportions, malformed limbs, missing arms, missing legs, extra arms, extra legs, fused fingers, too many fingers, long neck";
    
    public uint GetPromptHash()
    {
        const uint fnvPrime = 0x811C9DC5;
        var hash = 0x811C9DC5;
        var input = $"{Positive}|{Negative}|{Width}|{Height}|{Steps}";
        var inputBytes = Encoding.UTF8.GetBytes(input);
        foreach(var b in inputBytes)
        {
            hash ^= b;
            hash *= fnvPrime;
        }
        return hash;
    }
}
