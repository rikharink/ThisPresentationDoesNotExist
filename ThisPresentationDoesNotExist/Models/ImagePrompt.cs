
using System.Text;

namespace ThisPresentationDoesNotExist.Models;

public record ImagePrompt(
    string Positive,
    string Negative = "",
    int Width = 1024,
    int Height = 1024,
    int Steps = 30)
{
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
