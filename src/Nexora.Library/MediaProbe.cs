using System.Diagnostics;
using System.Globalization;

namespace Nexora.Library;

/// <summary>Lee la duración de un vídeo con ffprobe.</summary>
public sealed class MediaProbe
{
    public string? ToolPath { get; }
    public bool IsAvailable => ToolPath != null;

    public MediaProbe()
    {
        ToolPath = FfToolsLocator.Find("ffprobe");
    }

    public async Task<TimeSpan> GetDurationAsync(string file)
    {
        if (ToolPath == null) return TimeSpan.Zero;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        var psi = new ProcessStartInfo(ToolPath)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var arg in new[]
                 {
                     "-v", "error",
                     "-show_entries", "format=duration",
                     "-of", "default=noprint_wrappers=1:nokey=1",
                     file
                 })
        {
            psi.ArgumentList.Add(arg);
        }

        try
        {
            using var process = Process.Start(psi);
            if (process == null) return TimeSpan.Zero;

            var stdout = process.StandardOutput.ReadToEndAsync(cts.Token);
            _ = process.StandardError.ReadToEndAsync(cts.Token);
            await process.WaitForExitAsync(cts.Token);

            var text = (await stdout).Trim();
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) && seconds > 0
                ? TimeSpan.FromSeconds(seconds)
                : TimeSpan.Zero;
        }
        catch (Exception)
        {
            return TimeSpan.Zero;
        }
    }
}
