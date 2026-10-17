namespace Nexora.Storage;

/// <summary>Registro sencillo en archivo, un archivo por día, en %APPDATA%\NexoraPlayout\logs.</summary>
public static class Log
{
    private static readonly object Gate = new();

    public static void Info(string message) => Write("INFO", message);

    public static void Error(string message, Exception? ex = null) =>
        Write("ERROR", ex == null ? message : $"{message}{Environment.NewLine}{ex}");

    private static void Write(string level, string message)
    {
        try
        {
            var file = System.IO.Path.Combine(AppPaths.LogsDir, $"nexora-{DateTime.Now:yyyyMMdd}.log");
            var line = $"{DateTime.Now:HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}";
            lock (Gate) File.AppendAllText(file, line);
        }
        catch
        {
            // El registro nunca debe romper la aplicación.
        }
    }
}
