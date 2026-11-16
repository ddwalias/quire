using System.Windows;
using System.Windows.Input;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetControlDebugger
{
    void OnModeChanged(object spiritMainWindow, DependencyProperty activeDocumentProperty, ExecutedRoutedEventArgs e);
}
