using System.Text.Json.Serialization;

namespace Nexora.Core;

/// <summary>Un vídeo de la biblioteca o de una lista de reproducción.</summary>
public sealed class MediaItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public TimeSpan Duration { get; set; }

    [JsonIgnore] public bool IsMissing => !File.Exists(Path);
    [JsonIgnore] public string DurationText => TimeFormat.Hms(Duration);
    [JsonIgnore] public string StatusText => IsMissing ? "No encontrado" : "Disponible";

    public static MediaItem FromFile(string path) => new()
    {
        Path = path,
        Name = System.IO.Path.GetFileNameWithoutExtension(path)
    };
}
