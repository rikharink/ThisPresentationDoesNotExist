using System.Text.Json.Serialization;

namespace ThisPresentationDoesNotExist.Models;

public class TextToImageResponse
{
    [JsonPropertyName("images")] public string[] Images { get; set; } = [];
    
    [JsonPropertyName("info")]
    public string Info { get; set; } = "";
}