namespace Nexora.Storage;

/// <summary>Carpetas de datos de la aplicación: %APPDATA%\NexoraPlayout</summary>
public static class AppPaths
{
    public static string Root
    {
        get
        {
            var dir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NexoraPlayout");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    public static string LogsDir
    {
        get
        {
            var dir = System.IO.Path.Combine(Root, "logs");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    public static string GetFile(string name) => System.IO.Path.Combine(Root, name);
}
