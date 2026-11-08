#nullable disable
using System.Windows.Documents;
using System.Windows.Input;
using Quire.Wpf;

namespace Quire.Controls;

public static partial class EngineeringDocCommands
{
    public static readonly RoutedUICommand Cut;
    public static readonly RoutedUICommand Copy;
    public static readonly RoutedUICommand Paste;
    public static readonly RoutedUICommand Delete;
    public static readonly RoutedUICommand ZoomOut;
    public static readonly RoutedUICommand ZoomIn;
    public static readonly RoutedUICommand Enter;
    public static readonly RoutedUICommand Traverse;

    static EngineeringDocCommands()
    {
        Cut = ApplicationCommands.Cut;
        Copy = ApplicationCommands.Copy;
        Paste = ApplicationCommands.Paste;
        Delete = ApplicationCommands.Delete;
        ZoomOut = NavigationCommands.DecreaseZoom;
        ZoomIn = NavigationCommands.IncreaseZoom;
        Enter = EditingCommands.EnterParagraphBreak;
        Traverse = new RoutedUICommand();
        Traverse.InputGestures.Add(Gestures.Tab);
    }
}
