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

    private abstract partial class DebuggedObject
    {
        public event CloseInstance AskedCloseInstance;

        private void InvokeAskedCloseInstance()
        {
            CloseInstance askedCloseInstance = AskedCloseInstance;
            if (askedCloseInstance != null)
            {
                askedCloseInstance();
            }
        }

        private void UnSubscribeOnActiveTabChangedEvent()
        {
            global::Quire.Wpf.DependencyPropertyChangeManager.RemovePropertyChangedHandler(
                _activeDocumentProperty,
                _spiritMainWindowDependencyObject,
                HandlerOnActiveTabChanged);
        }

        private void SubscribeOnActiveTabChangedEvent(DependencyProperty activeDocumentProperty, DependencyObject spiritMainWindowDependencyObject)
        {
            _activeDocumentProperty = activeDocumentProperty;
            _spiritMainWindowDependencyObject = spiritMainWindowDependencyObject;
            global::Quire.Wpf.DependencyPropertyChangeManager.AddPropertyChangedHandler(
                _activeDocumentProperty,
                _spiritMainWindowDependencyObject,
                HandlerOnActiveTabChanged);
        }

        private void HandlerOnActiveTabChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_isDebuggingEnabled)
            {
                UnSubscribeEvents();
                FillDataForDebugging(e.NewValue as global::Quire.Controls.Core.IEngineeringDocument);
                SubscribeEvents();
            }
        }

        public virtual void Dispose()
        {
            global::Quire.DebugTools.DevAssert(_isDebuggingEnabled, "Debugging already disabled.");
            UnSubscribeEvents();
            UnSubscribeOnActiveTabChangedEvent();
            DestructUI();
            _isDebuggingEnabled = false;
        }

        protected virtual void AskCloseInstance()
        {
            InvokeAskedCloseInstance();
        }

        protected virtual void InitializeUI()
        {
            SubscribeEvents();
        }
    }
}
