using System.Reflection;
using System.Runtime.InteropServices;

namespace ThisPresentationDoesNotExist.Helpers;

public static class FileSystem
{
    public static string GetCacheDirectory()
    {
        var appName = Assembly.GetExecutingAssembly().GetName().Name!;
        string cacheFolder;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // For Windows, use the Local Application Data folder
            cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // For macOS, use the cache directory
            var homeDirectory = Environment.GetEnvironmentVariable("HOME")!;
            var cacheHome = Path.Combine(homeDirectory, "Library", "Caches");
            cacheFolder = Path.Combine(cacheHome, appName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // For Linux, use the XDG_CACHE_HOME environment variable, or default to $HOME/.cache
            var homeDirectory = Environment.GetEnvironmentVariable("HOME")!;
            var cacheHome = Environment.GetEnvironmentVariable("XDG_CACHE_HOME") ??
                               Path.Combine(homeDirectory, ".cache");
            cacheFolder = Path.Combine(cacheHome, appName);
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS platform.");
        }
        
        if (!Directory.Exists(cacheFolder))
        {
            Directory.CreateDirectory(cacheFolder);
        }

        return cacheFolder;
    }
}