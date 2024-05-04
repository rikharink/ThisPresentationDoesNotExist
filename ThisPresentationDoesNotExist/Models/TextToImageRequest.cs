using System.Text.Json.Serialization;

namespace ThisPresentationDoesNotExist.Models;

public class TextToImageRequest
{
    [JsonPropertyName("prompt")] public string Prompt { get; set; } = "";

    [JsonPropertyName("negative_prompt")] public string NegativePrompt { get; set; } = "";
    
    [JsonPropertyName("steps")]
    public int Steps { get; set; }
    
    [JsonPropertyName("width")]
    public int Width { get; set; }
    
    [JsonPropertyName("height")]
    public int Height { get; set; }
}