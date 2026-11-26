using System.ComponentModel;
using System.Linq;
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
        private readonly int _propertyNameEnumCount = global::System.Enum.GetNames(typeof(PropertyName)).Count();
        private readonly int _coordinateSystemNameEnumCount = global::System.Enum.GetNames(typeof(CoordinateSystemName)).Count();

        public WorksheetControlUI(object spiritMainWindow, DependencyProperty activeDocumentProperty)
            : base(spiritMainWindow, activeDocumentProperty)
        {
            InitializeDebugWindow();
            base.InitializeUI();
        }

        protected override void DestructUI()
        {
            DestroyDebugWindow();
        }

        protected override void SubscribeEvents()
        {
            if (WorksheetControlInst.PageManager != null)
            {
                WorksheetControlInst.PageManager.VisiblePages.RangeChanged += OnVisiblePagesCollectionChanged;
            }
            if (WorksheetViewModelInst != null)
            {
                WorksheetViewModelInst.PropertyChanged += OnWorksheetViewModelPropertyChanged;
                WorksheetViewModelInst.WorksheetContent.InsertionPointSet += OnWorksheetContentInsertionPointSet;
                WorksheetViewModelInst.Header.Activated += OnHeaderActivated;
                WorksheetViewModelInst.Footer.Activated += OnFooterActivated;
                WorksheetViewModelInst.PageLayout.PropertyChanged += OnPageLayoutPropertyChanged;
            }
            if (ScrollViewerInst != null)
            {
                ScrollViewerInst.ScrollChanged += OnScrollViewerChanged;
            }
            OnHeaderActivated();
            OnFooterActivated();
            OnVisiblePagesCollectionChanged(null, null);
            OnWorksheetViewModelPropertyChanged(null, null);
            OnWorksheetContentInsertionPointSet();
            OnScrollViewerChanged(null, null);
            OnPageLayoutPropertyChanged(null, null);
        }

        protected override void UnSubscribeEvents()
        {
            if (WorksheetControlInst.PageManager != null)
            {
                WorksheetControlInst.PageManager.VisiblePages.RangeChanged -= OnVisiblePagesCollectionChanged;
            }
            if (WorksheetViewModelInst != null)
            {
                WorksheetViewModelInst.PropertyChanged -= OnWorksheetViewModelPropertyChanged;
                WorksheetViewModelInst.WorksheetContent.InsertionPointSet -= OnWorksheetContentInsertionPointSet;
                WorksheetViewModelInst.Header.Activated -= OnHeaderActivated;
                WorksheetViewModelInst.Footer.Activated -= OnFooterActivated;
                WorksheetViewModelInst.PageLayout.PropertyChanged -= OnPageLayoutPropertyChanged;
            }
            if (ScrollViewerInst != null)
            {
                ScrollViewerInst.ScrollChanged -= OnScrollViewerChanged;
            }
        }

        private void OnWorksheetViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateWindowCellWorksheet(PropertyName.WorksheetHeight, WorksheetViewModelInst.WorksheetHeight.ToString("0.00"));
            UpdateDebugWindowCellsAll(PropertyName.ActivePageNumber, WorksheetViewModelInst.CurrentPageNumber.ToString());
            UpdateDebugWindowCellsAll(PropertyName.PagesCount, WorksheetControlInst.PageManager.Pages.Count.ToString());
        }

        private void OnVisiblePagesCollectionChanged(
            object sender,
            global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateDebugWindowCellsAll(PropertyName.VisiblePagesCount, WorksheetControlInst.PageManager.VisiblePages.Count.ToString());
            if (WorksheetControlInst.PageManager.VisiblePages.Count > 0)
            {
                IWorksheetPage firstVisiblePage = WorksheetControlInst.PageManager.VisiblePages.First();
                IWorksheetPage lastVisiblePage = WorksheetControlInst.PageManager.VisiblePages.Last();
                UpdateDebugWindowCellsAll(
                    PropertyName.VisiblePagesNumbers,
                    string.Format(
                        "{0},...,{1}",
                        WorksheetControlInst.PageManager.Pages.IndexOf(firstVisiblePage) + 1,
                        WorksheetControlInst.PageManager.Pages.IndexOf(lastVisiblePage) + 1));
            }
        }

        private void WorksheetControlUIDebugClosed(object sender, EventArgs e)
        {
            AskCloseInstance();
        }

        private void DestroyDebugWindow()
        {
            if (_win != null)
            {
                _win.Closed -= WorksheetControlUIDebugClosed;
                _grid = null;
                _win.Close();
                _win = null;
            }
        }

        private void UpdateDebugWindowCell(PropertyName propertyName, CoordinateSystemName coordinateSystemName, string text)
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }

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

        private void OnHeaderActivated()
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }

        private void OnFooterActivated()
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
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

    private sealed partial class VisibleObjects
    {
        protected override void DestructUI()
        {
            RemoveStatusBarItemFromStatusBar();
            DestroyPreviewWindow();
        }

        protected override void SubscribeEvents()
        {
            SubscribePagesEvents();
            SubscribeWorksheetItemsEvents();
            EnableAdornerUpdates();
        }

        protected override void UnSubscribeEvents()
        {
            UnsSubscribePagesEvents();
            UnSubscribeWorksheetItemsEvents();
            DisableAdornerUpdates();
        }

        private void RemoveStatusBarItemFromStatusBar()
        {
            if (_indicationStatusBarItem != null && _statusBar.Items.Contains(_indicationStatusBarItem))
            {
                _statusBar.Items.Remove(_indicationStatusBarItem);
            }
            _indicationTextBlock = null;
            _indicationStatusBarItem = null;
        }

        private void UpdateIndicationInStatusBar()
        {
            if (_indicationTextBlock != null)
            {
                _indicationTextBlock.Text = string.Format("{0} | {1} | {2}", _textPages, _textVisiblePages, _textItemsPart);
            }
        }

        private void SubscribePagesEvents()
        {
            WorksheetViewModelInst.Pages.RangeChanged += WorksheetPagesCollectionChanged;
            WorksheetViewModelInst.VisiblePages.RangeChanged += WorksheetVisiblePagesCollectionChanged;
            WorksheetViewModelInst.PageManager.ContentActivated += OnPageManagerContentActivated;
            WorksheetControlInst.SizeChanged += WorksheetControlSizeChanged;
            WorksheetPagesCollectionChanged(null, null);
            WorksheetVisiblePagesCollectionChanged(null, null);
            foreach (IWorksheetPage visiblePage in WorksheetViewModelInst.VisiblePages)
            {
                UpdatePageZombiesVisibility(visiblePage, toBeClipped: true);
            }
            UpdateWorksheetItemsVisibilityAdorner(WorksheetViewModelInst.Header, isItemOnVisiblePage: true);
            UpdateWorksheetItemsVisibilityAdorner(WorksheetViewModelInst.Footer, isItemOnVisiblePage: true);
            UpdateWorksheetItemsVisibilityAdorner(
                ((global::System.Windows.Controls.Grid)(object)ScrollViewerInst.Parent).Children[1],
                isItemOnVisiblePage: true,
                toBeClipped: true);
            UpdatePreviewVisual();
        }

        private void UnsSubscribePagesEvents()
        {
            WorksheetViewModelInst.Pages.RangeChanged -= WorksheetPagesCollectionChanged;
            WorksheetViewModelInst.VisiblePages.RangeChanged -= WorksheetVisiblePagesCollectionChanged;
            WorksheetViewModelInst.PageManager.ContentActivated -= OnPageManagerContentActivated;
            WorksheetControlInst.SizeChanged -= WorksheetControlSizeChanged;
            foreach (IWorksheetPage visiblePage in WorksheetViewModelInst.VisiblePages)
            {
                UpdatePageZombiesVisibility(visiblePage);
            }
            UpdateWorksheetItemsVisibilityAdorner(WorksheetViewModelInst.Header, isItemOnVisiblePage: false);
            UpdateWorksheetItemsVisibilityAdorner(WorksheetViewModelInst.Footer, isItemOnVisiblePage: false);
            UpdateWorksheetItemsVisibilityAdorner(
                ((global::System.Windows.Controls.Grid)(object)ScrollViewerInst.Parent).Children[1],
                isItemOnVisiblePage: false);
        }

        private void InitializePreviewWindow()
        {
            if (_win2 == null)
            {
                _grid2 = new global::System.Windows.Controls.Grid
                {
                    Background = WorksheetControlInst.TryFindResource("WorksheetBackgroundBrush") as global::System.Windows.Media.LinearGradientBrush,
                    HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = global::System.Windows.VerticalAlignment.Top
                };
                _grid2.RowDefinitions.Add(new global::System.Windows.Controls.RowDefinition());
                _grid2.RowDefinitions.Add(new global::System.Windows.Controls.RowDefinition());
                global::System.Windows.Controls.ScrollViewer content = new global::System.Windows.Controls.ScrollViewer
                {
                    Background = global::System.Windows.Media.Brushes.Gray,
                    HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch,
                    VerticalAlignment = global::System.Windows.VerticalAlignment.Stretch,
                    VerticalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = global::System.Windows.Controls.ScrollBarVisibility.Auto,
                    Content = _grid2
                };
                _win2 = new global::System.Windows.Window
                {
                    Content = content,
                    HorizontalAlignment = global::System.Windows.HorizontalAlignment.Stretch,
                    SizeToContent = global::System.Windows.SizeToContent.Width,
                    Title = "Preview",
                    Topmost = true,
                    ShowActivated = false,
                    ResizeMode = global::System.Windows.ResizeMode.CanResizeWithGrip,
                    Height = 731.0,
                    MinWidth = 200.0,
                    MaxWidth = 600.0
                };
                _win2.Show();
                _win2.Closed += WorksheetPreviewClosed;
                _win2.SizeChanged += OnWin2SizeChanged;
            }
        }

        private void WorksheetControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePreviewVisual();
        }

        private void WorksheetPagesCollectionChanged(
            object sender,
            global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _textPages = string.Format("Pages: [{0}]", WorksheetControlInst.PageManager.Pages.Count);
        }

        private void OnPageManagerContentActivated()
        {
            foreach (IWorksheetPage page in WorksheetViewModelInst.Pages)
            {
                UpdatePageZombiesVisibility(page);
            }
            foreach (IWorksheetPage visiblePage in WorksheetViewModelInst.VisiblePages)
            {
                UpdatePageZombiesVisibility(visiblePage, toBeClipped: true);
            }
        }

        private void UpdatePageZombiesVisibility(IWorksheetPage page, bool toBeClipped = false)
        {
            if (page == null)
            {
                return;
            }
            UpdateWorksheetItemsVisibilityAdorner(page.HeaderZombie, toBeClipped);
            UpdateWorksheetItemsVisibilityAdorner(page.FooterZombie, toBeClipped);
            if (page.FirstPageHeaderZombie != null)
            {
                UpdateWorksheetItemsVisibilityAdorner(page.FirstPageHeaderZombie, toBeClipped);
            }
            if (page.FirstPageFooterZombie != null)
            {
                UpdateWorksheetItemsVisibilityAdorner(page.FirstPageFooterZombie, toBeClipped);
            }
        }

        private void SubscribeWorksheetItemsEvents()
        {
            if (WorksheetControlInst != null && WorksheetControlInst.WorksheetItems != null)
            {
                WorksheetControlInst.WorksheetItems.RangeChanged += WorksheetItemsCollectionChanged;
                foreach (global::Quire.Controls.Core.IWhiteboardItem worksheetItem in WorksheetControlInst.WorksheetItems)
                {
                    AddItemIsOnVisiblePageChangedHandler(worksheetItem, OnWhiteboardItemVisibilityChange);
                }
                UpdateVisibleItemsCounter();
            }
        }

        private void UnSubscribeWorksheetItemsEvents()
        {
            if (WorksheetControlInst != null && WorksheetControlInst.WorksheetItems != null)
            {
                WorksheetControlInst.WorksheetItems.RangeChanged -= WorksheetItemsCollectionChanged;
                foreach (global::Quire.Controls.Core.IWhiteboardItem worksheetItem in WorksheetControlInst.WorksheetItems)
                {
                    RemoveItemIsOnVisiblePageChangedHandler(worksheetItem, OnWhiteboardItemVisibilityChange);
                }
            }
        }

        private void WorksheetItemsCollectionChanged(
            object sender,
            global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            global::System.Collections.IList newItems = e.NewItems;
            global::System.Collections.IList oldItems = e.OldItems;
            if (oldItems != null)
            {
                foreach (global::Quire.Controls.Core.IWhiteboardItem item in oldItems)
                {
                    RemoveItemIsOnVisiblePageChangedHandler(item, OnWhiteboardItemVisibilityChange);
                    if (item is global::Quire.Controls.Core.Whiteboard.IWhiteboardControl)
                    {
                        (item as global::Quire.Controls.Core.Whiteboard.IWhiteboardControl).WhiteboardItems.RangeChanged -= WorksheetItemsCollectionChanged;
                    }
                }
            }
            if (newItems != null)
            {
                foreach (global::Quire.Controls.Core.IWhiteboardItem item in newItems)
                {
                    AddItemIsOnVisiblePageChangedHandler(item, OnWhiteboardItemVisibilityChange);
                    if (item is global::Quire.Controls.Core.Whiteboard.IWhiteboardControl)
                    {
                        (item as global::Quire.Controls.Core.Whiteboard.IWhiteboardControl).WhiteboardItems.RangeChanged += WorksheetItemsCollectionChanged;
                    }
                }
            }
            UpdateVisibleItemsCounter();
            DebugTools.DevAssert(oldItems != null || newItems != null, null);
        }

        private void OnWhiteboardItemVisibilityChange(object sender, DependencyPropertyChangedEventArgs args)
        {
            UpdateVisibleItemsCounter();
        }

        private void AddItemIsOnVisiblePageChangedHandler(
            global::Quire.Controls.Core.IWhiteboardItem whiteboardItem,
            DependencyPropertyChangedEventHandler handler)
        {
            WorksheetViewModel.ItemManager.AddItemIsOnVisiblePageChangedHandler(whiteboardItem, handler);
        }

        private void RemoveItemIsOnVisiblePageChangedHandler(
            global::Quire.Controls.Core.IWhiteboardItem item,
            DependencyPropertyChangedEventHandler handler)
        {
            WorksheetViewModel.ItemManager.RemoveItemIsOnVisiblePageChangedHandler(item, handler);
        }

        private void EnableAdornerUpdates()
        {
            WorksheetControlInst.PageManager.Pages.RangeChanged += PagesCollectionChanged;
            foreach (IWorksheetPage page in WorksheetControlInst.PageManager.Pages)
            {
                page.WhiteboardItems.RangeChanged += PageWhiteboardItemsCollectionChanged;
                foreach (global::Quire.Controls.Core.IWhiteboardItem whiteboardItem in page.WhiteboardItems)
                {
                    AddItemIsOnVisiblePageChangedHandler(whiteboardItem, OnPageWhiteboardItemVisibilityChange);
                    UpdateWorksheetItemsVisibilityAdorner(whiteboardItem, WorksheetViewModel.ItemManager.GetItemIsOnVisiblePage(whiteboardItem));
                }
            }
        }

        private void DisableAdornerUpdates()
        {
            WorksheetControlInst.PageManager.Pages.RangeChanged -= PagesCollectionChanged;
            foreach (IWorksheetPage page in WorksheetControlInst.PageManager.Pages)
            {
                page.WhiteboardItems.RangeChanged -= PageWhiteboardItemsCollectionChanged;
                foreach (global::Quire.Controls.Core.IWhiteboardItem whiteboardItem in page.WhiteboardItems)
                {
                    UpdateWorksheetItemsVisibilityAdorner(whiteboardItem, isItemOnVisiblePage: false);
                    RemoveItemIsOnVisiblePageChangedHandler(whiteboardItem, OnPageWhiteboardItemVisibilityChange);
                }
            }
        }

        private void PagesCollectionChanged(
            object sender,
            global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            global::System.Collections.IList newItems = e.NewItems;
            global::System.Collections.IList oldItems = e.OldItems;
            if (oldItems != null)
            {
                foreach (IWorksheetPage page in oldItems)
                {
                    page.WhiteboardItems.RangeChanged -= PageWhiteboardItemsCollectionChanged;
                    foreach (global::Quire.Controls.Core.IWhiteboardItem whiteboardItem in page.WhiteboardItems)
                    {
                        UpdateWorksheetItemsVisibilityAdorner(whiteboardItem, isItemOnVisiblePage: false);
                        RemoveItemIsOnVisiblePageChangedHandler(whiteboardItem, OnPageWhiteboardItemVisibilityChange);
                    }
                }
            }
            if (newItems != null)
            {
                foreach (IWorksheetPage page in newItems)
                {
                    page.WhiteboardItems.RangeChanged += PageWhiteboardItemsCollectionChanged;
                    foreach (global::Quire.Controls.Core.IWhiteboardItem whiteboardItem in page.WhiteboardItems)
                    {
                        AddItemIsOnVisiblePageChangedHandler(whiteboardItem, OnPageWhiteboardItemVisibilityChange);
                        UpdateWorksheetItemsVisibilityAdorner(whiteboardItem, WorksheetViewModel.ItemManager.GetItemIsOnVisiblePage(whiteboardItem));
                    }
                }
            }
            DebugTools.DevAssert(oldItems != null || newItems != null, null);
        }

        private void PageWhiteboardItemsCollectionChanged(
            object sender,
            global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            global::System.Collections.IList newItems = e.NewItems;
            global::System.Collections.IList oldItems = e.OldItems;
            if (oldItems != null)
            {
                foreach (global::Quire.Controls.Core.IWhiteboardItem item in oldItems)
                {
                    UpdateWorksheetItemsVisibilityAdorner(item, isItemOnVisiblePage: false);
                    RemoveItemIsOnVisiblePageChangedHandler(item, OnPageWhiteboardItemVisibilityChange);
                }
            }
            if (newItems != null)
            {
                foreach (global::Quire.Controls.Core.IWhiteboardItem item in newItems)
                {
                    AddItemIsOnVisiblePageChangedHandler(item, OnPageWhiteboardItemVisibilityChange);
                    UpdateWorksheetItemsVisibilityAdorner(item, WorksheetViewModel.ItemManager.GetItemIsOnVisiblePage(item));
                }
            }
            DebugTools.DevAssert(oldItems != null || newItems != null, null);
        }

        private void OnPageWhiteboardItemVisibilityChange(
            object sender,
            DependencyPropertyChangedEventArgs args)
        {
            UpdateWorksheetItemsVisibilityAdorner(sender as global::Quire.Controls.Core.IWhiteboardItem, (bool)args.NewValue);
        }

        private void OnWin2SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePreviewVisual();
        }

        private void DestroyPreviewWindow()
        {
            if (_win2 != null)
            {
                _win2.Closed -= WorksheetPreviewClosed;
                _win2.SizeChanged -= OnWin2SizeChanged;
                _grid2 = null;
                _viewbox = null;
                _win2.Close();
                _win2 = null;
            }
        }

        private void WorksheetPreviewClosed(object sender, System.EventArgs e)
        {
            _win2.Closed -= WorksheetPreviewClosed;
            _win2.SizeChanged -= OnWin2SizeChanged;
            _grid2 = null;
            _win2 = null;
        }

        private partial class IndicationVisibilityAdorner
        {
            public global::System.Windows.UIElement Child
            {
                get => _child;
                set
                {
                    AddVisualChild(value);
                    _child = value;
                }
            }

            protected override int VisualChildrenCount => _child != null ? 1 : 0;

            protected override global::System.Windows.Size ArrangeOverride(global::System.Windows.Size finalSize)
            {
                if (_child != null)
                {
                    _child.Arrange(new global::System.Windows.Rect(finalSize));
                }
                return finalSize;
            }

            protected override global::System.Windows.Media.Visual GetVisualChild(int index)
            {
                if (index == 0 && _child != null)
                {
                    return _child;
                }
                return base.GetVisualChild(index);
            }

            private void UpdateOnScrollViewerChanged(object sender, System.EventArgs e)
            {
                if (_toBeClipped && AdornedElement.Clip != null)
                {
                    InvalidateVisual();
                }
            }
        }
    }
}
