using System.Windows;

namespace Quire;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        int failures = 0;
        DispatcherUnhandledException += (_, args) =>
        {
            // keep the worksheet (and the user's unsaved work) alive after an unexpected UI error,
            // but don't loop forever on an error that repeats during layout or rendering
            if (++failures > 3) return;
            MessageBox.Show(MainWindow, $"Something went wrong:\n{args.Exception.Message}\n\nYour worksheet is still open; please save your work.",
                "Quire", MessageBoxButton.OK, MessageBoxImage.Warning);
            args.Handled = true;
        };
        string? open = null, render = null;
        for (int i = 0; i < e.Args.Length; i++)
        {
            if (e.Args[i] == "--render" && i + 1 < e.Args.Length) render = e.Args[++i];
            else open ??= e.Args[i];
        }
        var window = new MainWindow();
        MainWindow = window;
        window.Show();
        if (open != null) window.OpenFile(open);
        else window.LoadSample();
        if (render != null) _ = window.RenderToFileAndExitAsync(render);
    }
}
