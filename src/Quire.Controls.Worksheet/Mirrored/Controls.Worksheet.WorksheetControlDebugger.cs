using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Quire.Controls.Worksheet;

public partial class WorksheetControlDebugger
{
    private readonly bool _isDebugMode = global::Quire.DebugTools.IsDebug;

    public static WorksheetControlDebugger Instance => _instance ?? (_instance = new WorksheetControlDebugger());

    private WorksheetControlDebugger()
    {
        if (!_isDebugMode)
        {
            return;
        }

        Application.Current.MainWindow.Closing += OnSpiritMainWindowClosing;
    }

    private void DestroyInstance()
    {
        _instance = null;
    }

    private void OnAskedCloseInstanceOfWorksheetControlUIDebuggedObjects()
    {
        _worksheetControlUI.Dispose();
        _worksheetControlUI = null;
    }

    private void OnAskedCloseInstanceOfVisibleObjectsDebuggedObjects()
    {
        _visibleObjects.Dispose();
        _visibleObjects = null;
    }

    private void OnSpiritMainWindowClosing(object sender, CancelEventArgs e)
    {
        if (_visibleObjects != null)
        {
            _visibleObjects.AskedCloseInstance -= OnAskedCloseInstanceOfVisibleObjectsDebuggedObjects;
            _visibleObjects.Dispose();
            _visibleObjects = null;
        }
        if (_worksheetControlUI != null)
        {
            _worksheetControlUI.AskedCloseInstance -= OnAskedCloseInstanceOfWorksheetControlUIDebuggedObjects;
            _worksheetControlUI.Dispose();
            _worksheetControlUI = null;
        }
        Application.Current.MainWindow.Closing -= OnSpiritMainWindowClosing;
        DestroyInstance();
    }
}
