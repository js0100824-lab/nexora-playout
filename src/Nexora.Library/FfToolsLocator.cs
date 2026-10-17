namespace Nexora.Library;

/// <summary>
/// Localiza ffmpeg/ffprobe: primero en la carpeta "ffmpeg" junto al ejecutable,
/// luego junto al ejecutable y por último en el PATH del sistema.
/// </summary>
public static class FfToolsLocator
{
    public static string? Find(string toolName)
    {
        var fileName = OperatingSystem.IsWindows() ? toolName + ".exe" : toolName;

        var candidates = new List<string>
        {
            System.IO.Path.Combine(AppContext.BaseDirectory, "ffmpeg", fileName),
            System.IO.Path.Combine(AppContext.BaseDirectory, fileName)
        };

        var pathVar = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dir in pathVar.Split(System.IO.Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            candidates.Add(System.IO.Path.Combine(dir.Trim(), fileName));
        }

        return candidates.FirstOrDefault(File.Exists);
    }
}
