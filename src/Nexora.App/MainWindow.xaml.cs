using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Nexora.Core;
using Nexora.Library;
using Nexora.Storage;

namespace Nexora.UI;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<MediaItem> _items = new();
    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromMilliseconds(250) };
    private readonly MediaProbe _probe = new();
    private readonly string _playlistFile = AppPaths.GetFile("playlist.json");

    public MainWindow()
    {
        InitializeComponent();

        PlaylistGrid.ItemsSource = _items;

        UpdateClock();
        _clock.Tick += (_, _) => UpdateClock();
        _clock.Start();

        Loaded += OnLoaded;
        Closing += (_, _) => SavePlaylist();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        ClockText.Text = now.ToString("HH:mm:ss");
        DateText.Text = now.ToString("dddd, dd MMM yyyy").ToUpperInvariant();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var saved = JsonStore.Load<Playlist>(_playlistFile);
        if (saved != null)
        {
            foreach (var item in saved.Items) _items.Add(item);
        }

        ProbeText.Text = _probe.IsAvailable
            ? "ffprobe: disponible"
            : "ffprobe no encontrado: no se pueden leer las duraciones (ver docs/INSTALACION.md)";

        RefreshSummary(_items.Count > 0 ? "Lista anterior cargada." : "Lista vacía.");
        await FillMissingDurationsAsync();
    }

    // ---------- Importación ----------

    private async Task AddPathsAsync(IEnumerable<string> paths)
    {
        var files = new List<string>();
        foreach (var path in paths)
        {
            if (Directory.Exists(path)) files.AddRange(MediaScanner.ScanFolder(path, recursive: true));
            else if (MediaScanner.IsSupported(path)) files.Add(path);
        }

        var known = _items.Select(i => i.Path).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var added = 0;
        foreach (var file in files)
        {
            if (!known.Add(file)) continue;
            _items.Add(MediaItem.FromFile(file));
            added++;
        }

        RefreshSummary(added == 0
            ? "No se encontraron vídeos nuevos compatibles."
            : $"Se añadieron {added} vídeo(s).");

        await FillMissingDurationsAsync();
        SavePlaylist();
    }

    private async Task FillMissingDurationsAsync()
    {
        if (!_probe.IsAvailable) return;

        foreach (var item in _items.ToList())
        {
            if (item.Duration > TimeSpan.Zero || item.IsMissing) continue;
            item.Duration = await _probe.GetDurationAsync(item.Path);
            PlaylistGrid.Items.Refresh();
        }

        RefreshSummary();
        SavePlaylist();
    }

    private async void AddFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = "Selecciona una o varias carpetas con vídeos", Multiselect = true };
        if (dialog.ShowDialog(this) == true) await AddPathsAsync(dialog.FolderNames);
    }

    private async void AddFiles_Click(object sender, RoutedEventArgs e)
    {
        var filter = string.Join(";", MediaScanner.VideoExtensions.Select(x => "*" + x));
        var dialog = new OpenFileDialog
        {
            Title = "Selecciona vídeos",
            Multiselect = true,
            Filter = $"Vídeos ({filter})|{filter}|Todos los archivos|*.*"
        };
        if (dialog.ShowDialog(this) == true) await AddPathsAsync(dialog.FileNames);
    }

    private void Window_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private async void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths) await AddPathsAsync(paths);
    }

    // ---------- Edición de la lista ----------

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        // Solo se quita de la lista; el archivo original en el disco no se toca.
        if (PlaylistGrid.SelectedItem is not MediaItem item) return;
        var index = _items.IndexOf(item);
        _items.Remove(item);
        if (_items.Count > 0) PlaylistGrid.SelectedIndex = Math.Min(index, _items.Count - 1);
        RefreshSummary("Elemento quitado de la lista (el archivo original no se borra).");
        SavePlaylist();
    }

    private void MoveUp_Click(object sender, RoutedEventArgs e) => Move(-1);
    private void MoveDown_Click(object sender, RoutedEventArgs e) => Move(1);

    private void Move(int delta)
    {
        var from = PlaylistGrid.SelectedIndex;
        var to = from + delta;
        if (from < 0 || to < 0 || to >= _items.Count) return;
        _items.Move(from, to);
        PlaylistGrid.SelectedIndex = to;
        SavePlaylist();
    }

    private void Verify_Click(object sender, RoutedEventArgs e)
    {
        PlaylistGrid.Items.Refresh();
        var missing = _items.Count(i => i.IsMissing);
        RefreshSummary(missing == 0
            ? "Todos los archivos están disponibles."
            : $"{missing} archivo(s) no se encuentran en su ubicación (marcados en rojo).");
    }

    // ---------- Utilidades ----------

    private void RefreshSummary(string? message = null)
    {
        var total = TimeSpan.FromTicks(_items.Sum(i => i.Duration.Ticks));
        var missing = _items.Count(i => i.IsMissing);
        var summary = $"{_items.Count} elemento(s) · {TimeFormat.Hms(total)} en total";
        if (missing > 0) summary += $" · {missing} no encontrado(s)";
        StatusText.Text = message == null ? summary : $"{message}  |  {summary}";
    }

    private void SavePlaylist() =>
        JsonStore.Save(_playlistFile, new Playlist { Name = "Principal", Items = _items.ToList() });
}
