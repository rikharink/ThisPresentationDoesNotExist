using System.Reflection;

namespace ThisPresentationDoesNotExist.Helpers;

public static class EmbeddedResourcesHelpers
{
    public static bool TryGetEmbeddedResourceStreamByFilename(Assembly assembly, string filename, out Stream? stream)
    {
        var resources = assembly.GetManifestResourceNames();
        stream = assembly.GetManifestResourceStream(resources.First(r => r.EndsWith(filename)));
        return stream is not null;
    }
    
    public static Stream GetEmbeddedResourceStreamByFilename(Assembly assembly, string filename)
    {
        if (TryGetEmbeddedResourceStreamByFilename(assembly, filename, out var stream))
        {
            return stream!;
        }
        throw new FileNotFoundException($"Embedded resource {filename} not found in assembly {assembly.FullName}");
    }
}