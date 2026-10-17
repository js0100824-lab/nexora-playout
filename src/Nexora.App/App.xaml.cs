using System.Windows;
using Nexora.Storage;

namespace Nexora.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error("Excepción no controlada en la interfaz", args.Exception);
            MessageBox.Show(
                "Ocurrió un error inesperado. Se registró en la carpeta de logs.\n\n" + args.Exception.Message,
                "NEXORA PLAYOUT", MessageBoxButton.OK, MessageBoxImage.Warning);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            Log.Error("Excepción fatal", args.ExceptionObject as Exception);

        Log.Info("NEXORA PLAYOUT iniciado");
        base.OnStartup(e);
    }
}
