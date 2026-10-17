namespace Nexora.Library;

/// <summary>Detecta vídeos compatibles dentro de carpetas.</summary>
public static class MediaScanner
{
    public static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm",
        ".mpg", ".mpeg", ".m4v", ".ts", ".mts", ".m2ts", ".ogv", ".3gp"
    };

    public static bool IsSupported(string path) =>
        VideoExtensions.Contains(System.IO.Path.GetExtension(path));

    public static IReadOnlyList<string> ScanFolder(string folder, bool recursive = true)
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = recursive,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.Hidden | FileAttributes.System
        };

        return Directory.EnumerateFiles(folder, "*", options)
            .Where(IsSupported)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
