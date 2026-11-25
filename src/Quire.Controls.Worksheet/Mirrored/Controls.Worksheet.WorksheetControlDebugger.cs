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

        protected DebuggedObject(object spiritMainWindow, DependencyProperty activeDocumentProperty)
        {
            global::Quire.DebugTools.DevAssert(!_isDebuggingEnabled, "Debugging already enabled.");
            _isDebuggingEnabled = true;
            DependencyObject spiritMainWindowDependencyObject = spiritMainWindow as DependencyObject;
            global::Quire.DebugTools.DevAssert(spiritMainWindowDependencyObject != null, "SpiritMainWindow should be DependencyObject.");
            global::Quire.Controls.Core.IEngineeringDocument engineeringDocument =
                spiritMainWindowDependencyObject.GetValue(activeDocumentProperty) as global::Quire.Controls.Core.IEngineeringDocument;
            global::Quire.DebugTools.DevAssert(engineeringDocument != null, "IEngineeringDocument should not be null.");
            FillDataForDebugging(engineeringDocument);
            SubscribeOnActiveTabChangedEvent(activeDocumentProperty, spiritMainWindowDependencyObject);
        }

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

        private void FillDataForDebugging(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument)
        {
            WorksheetControlInst = ((global::System.Windows.Controls.ContentControl)engineeringDocument).Content as WorksheetControl;
            global::Quire.DebugTools.DevAssert(WorksheetControlInst != null, "WorksheetControl should not be null.");
            global::Quire.Wpf.WpfUtils.DoEvents();
            WorksheetViewModelInst = WorksheetControlInst.DataContext as WorksheetViewModel;
            global::Quire.DebugTools.DevAssert(WorksheetViewModelInst != null, "WorksheetViewModel should not be null.");
            global::System.Windows.Rect mainScrollViewerBounds;
            // Keep the helper in a local so Release IL uses Mathcad's callvirt here.
            WorksheetVisualHelper visualHelper = new WorksheetVisualHelper();
            global::System.Windows.Controls.Primitives.IScrollInfo scrollViewerRect =
                visualHelper.GetScrollViewerRect(WorksheetControlInst, out mainScrollViewerBounds);
            global::Quire.DebugTools.DevAssert(scrollViewerRect != null, "IScrollInfo should not be null.");
            ScrollViewerInst = scrollViewerRect.ScrollOwner;
            global::Quire.DebugTools.DevAssert(ScrollViewerInst != null, "ScrollViewer should not be null.");
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

    private sealed partial class WorksheetControlUI
    {
        private void UpdateWindowCellWorksheetItemsCanvas(PropertyName propertyName, string text)
        {
            UpdateDebugWindowCell(propertyName, CoordinateSystemName.WorksheetItems, text);
        }

        private void UpdateWindowCellWorksheet(PropertyName propertyName, string text)
        {
            UpdateDebugWindowCell(propertyName, CoordinateSystemName.Worksheet, text);
        }

        private void UpdateDebugWindowCellPage(PropertyName propertyName, string text)
        {
            UpdateDebugWindowCell(propertyName, CoordinateSystemName.Page, text);
        }

        private void UpdateDebugWindowCellScrollViewer(PropertyName propertyName, string text)
        {
            UpdateDebugWindowCell(propertyName, CoordinateSystemName.ScrollViewer, text);
        }

        private void UpdateDebugWindowCellHeaderFooter(PropertyName propertyName, string text)
        {
            UpdateDebugWindowCell(propertyName, CoordinateSystemName.HeaderFooter, text);
        }

        private void UpdateDebugWindowCellsAll(PropertyName propertyName, string text)
        {
            for (int index = 1; index < _coordinateSystemNameEnumCount; index++)
            {
                UpdateDebugWindowCell(propertyName, (CoordinateSystemName)index, text);
            }
        }

        private Point TranslatePoint(Point point, object source, object target)
        {
            return global::Quire.Controls.Whiteboard.WhiteboardViewModel.WhiteboardItemMgr.TranslatePoint(source, target, point);
        }
    }
}
