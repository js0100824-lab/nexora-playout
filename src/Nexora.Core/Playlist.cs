namespace Nexora.Core;

/// <summary>Lista ordenada de contenidos. Se guarda tal cual en JSON.</summary>
public sealed class Playlist
{
    public string Name { get; set; } = "Principal";
    public List<MediaItem> Items { get; set; } = new();
}
