namespace Nexora.Core;

public static class TimeFormat
{
    /// <summary>Formato HH:MM:SS (las horas pueden superar 24). Devuelve --:--:-- si no hay valor.</summary>
    public static string Hms(TimeSpan t) =>
        t > TimeSpan.Zero
            ? $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}"
            : "--:--:--";
}
