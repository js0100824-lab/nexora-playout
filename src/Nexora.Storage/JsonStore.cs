using System.Text.Json;

namespace Nexora.Storage;

/// <summary>Guarda y carga objetos en JSON. La escritura es atómica (archivo temporal + reemplazo).</summary>
public static class JsonStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static T? Load<T>(string path) where T : class
    {
        try
        {
            if (!File.Exists(path)) return null;
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options);
        }
        catch (Exception ex)
        {
            Log.Error($"No se pudo leer {path}", ex);
            return null;
        }
    }

    public static bool Save<T>(string path, T value)
    {
        try
        {
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(value, Options));
            File.Move(tmp, path, overwrite: true);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"No se pudo guardar {path}", ex);
            return false;
        }
    }
}
