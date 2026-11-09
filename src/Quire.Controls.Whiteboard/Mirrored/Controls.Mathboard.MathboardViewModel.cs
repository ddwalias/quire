#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Text;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Services;
using Quire.Undo;

namespace Quire.Controls.Mathboard;

public partial class MathboardViewModel : WhiteboardViewModel, IMathboardViewModel, ICurrentNumberFormatProperty, INotifyPropertyChanged, ICurrentMatrixFormatProperty, ICurrentTextFormatProperty, IWhiteboardViewModel, IWhiteboardItemContainerWithActiveItem, IDisposable
{
    private NumberFormat _currentNumberFormat;
    private MatrixFormat _currentMatrixFormat;
    private TextFormat _currentTextFormat;
    protected IMathboardModel _mathboardModel;
    private bool _hideFromCalculationButtonEnabled = true;
    private bool _hideFromCalculationButtonChecked;
    private double _containerFrameThickness = 0.5;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal MathboardViewModel(IMathboardModel model)
        : this(model, (Size?)null)
    {
    }

    protected override void HandleItemAdded(IWhiteboardItem item)
    {
        base.HandleItemAdded(item);
        IIORegionContainer ioRegionContainer = item as IIORegionContainer;
        if (ioRegionContainer != null && ioRegionContainer.InputOutputRegionManager == null)
        {
            ioRegionContainer.InputOutputRegionManager = _mathboardModel.IoRegionManager;
        }
        IMathboardControl mathboardControl = item as IMathboardControl;
        if (mathboardControl != null && mathboardControl.BookmarksManager == null)
        {
            mathboardControl.BookmarksManager = _mathboardModel.BookmarksManager;
        }
        IRichTextWithEmbeddedItem richText = item as IRichTextWithEmbeddedItem;
        if (richText != null && richText.BookmarksManager == null)
        {
            richText.BookmarksManager = _mathboardModel.BookmarksManager;
        }
    }

    protected override void HandleItemRemoved(IWhiteboardItem item)
    {
        IIORegionContainer ioRegionContainer = item as IIORegionContainer;
        if (ioRegionContainer != null)
        {
            ioRegionContainer.InputOutputRegionManager = null;
        }
        IMathboardControl mathboardControl = item as IMathboardControl;
        if (mathboardControl != null)
        {
            mathboardControl.BookmarksManager = null;
        }
        IRichTextWithEmbeddedItem richText = item as IRichTextWithEmbeddedItem;
        if (richText != null)
        {
            richText.BookmarksManager = null;
        }
        base.HandleItemRemoved(item);
    }

    protected override void SelectedItemsChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
        base.SelectedItemsChangedHandler(sender, e);
        IsCrosshairVisible = ActiveOrSelectedItems.Count() == 0;
        if (e.NewItems != null && DebugTools.AssertNotNull(_mathboardModel.IoRegionManager, null).Passed)
        {
            _mathboardModel.IoRegionManager.HandleElementsListSelectedInParentContainer(e.NewItems.OfType<IIORegionBase>());
        }
        UpdateHideFromCalculationButton();
        if (SelectedDescendants.Any())
        {
            IRegionCollectionWithProperty regionCollection = AppServiceRegistry.GetService<IRegionCollectionWithPropertyCreator>().CreateRegionCollection(SelectedDescendants);
            CurrentNumberFormat = regionCollection.CurrentNumberFormat;
            CurrentMatrixFormat = regionCollection.CurrentMatrixFormat;
            CurrentTextFormat = regionCollection.CurrentTextFormat;
        }
        else
        {
            CurrentNumberFormat = null;
            CurrentMatrixFormat = null;
            CurrentTextFormat = null;
        }
    }

    protected override void DeactivateActiveItem()
    {
        base.DeactivateActiveItem();
        ICalculatable calculatable = ActiveItem as ICalculatable;
        if (calculatable != null)
        {
            ICalculatableInfo calculatableInfo = calculatable as ICalculatableInfo;
            if (calculatableInfo == null || !calculatableInfo.IsCommitted)
            {
                calculatable.Commit();
            }
        }
        ICalculatableContainer container = ActiveItem as ICalculatableContainer;
        if (container != null && !(container is ICalculatableAggregate))
        {
            foreach (ICalculatable child in container.CalculatableChildren)
            {
                ICalculatableInfo childInfo = child as ICalculatableInfo;
                if (childInfo == null || !childInfo.IsCommitted)
                {
                    child.Commit();
                }
            }
        }
        RichTextBase richText = ActiveItem as RichTextBase;
        if (richText != null)
        {
            richText.SelectionChanged -= OnActiveTextRegionContentChanged;
            richText.TextChanged -= OnActiveTextRegionContentChanged;
        }
        ICurrentMatrixFormatProperty matrixFormat = ActiveItem as ICurrentMatrixFormatProperty;
        if (matrixFormat != null)
        {
            matrixFormat.PropertyChanged -= ActiveItemPropertyChangedHandler;
        }
    }

    protected override void HandleChildItemGotFocus(IWhiteboardItem item)
    {
        base.HandleChildItemGotFocus(item);
        if (item is ICurrentNumberFormatProperty currentNumberFormatProperty)
        {
            CurrentNumberFormat = currentNumberFormatProperty.CurrentNumberFormat;
        }
        else
        {
            CurrentNumberFormat = null;
        }
        if (item is ICurrentMatrixFormatProperty currentMatrixFormatProperty)
        {
            CurrentMatrixFormat = currentMatrixFormatProperty.CurrentMatrixFormat;
        }
        else
        {
            CurrentMatrixFormat = null;
        }
        if (item is RichTextBase richTextBase)
        {
            richTextBase.SelectionChanged += OnActiveTextRegionContentChanged;
            richTextBase.TextChanged += OnActiveTextRegionContentChanged;
            if (item is ICurrentTextFormatProperty currentTextFormatProperty)
            {
                CurrentTextFormat = currentTextFormatProperty.CurrentTextFormat;
            }
        }
        if (item is ICurrentIdLabelProperty currentIdLabelProperty)
        {
            currentIdLabelProperty.PropertyChanged += ActiveItemPropertyChangedHandler;
        }
        UpdateHideFromCalculationButton();
    }

    public override RemoveItemsCommand GetRemoveItemsCommand(string commandName, IDictionary<IWhiteboardItem, Point> itemsAtLocations)
    {
        return new RemoveMathboardItemsCommand(this, commandName, itemsAtLocations);
    }

    public override PasteItemsCommand GetPasteItemsCommand(string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        return new PasteMathboardItemsCommand(this, commandName, pasteItemsAtLocation);
    }

    protected override void SetupCommandBindings()
    {
        base.SetupCommandBindings();
        CreateCommandBinding(IORegionCommands.ToggleInputRegion, (object param) =>
        {
            ToggleInputRegions();
        }, (object param) => CanToggleRegionsAsInput());
        CreateCommandBinding(IORegionCommands.ToggleOutputRegion, (object param) =>
        {
            ToggleOutputRegions();
        }, (object param) => CanToggleRegionsAsOutput());
        CommandBindings.Add(new CommandBinding(RegionCommands.TagCommand, HandleBookmarkTag, CanBookmarkTag));
        CommandBindings.Add(new CommandBinding(RegionCommands.RemoveTagCommand, HandleRemoveBookmarkTag, CanRemoveBookmarkTag));
    }

    private void CanBookmarkTag(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ActiveOrSelectedItems.Count() == 1;
        if (ActiveOrSelectedItems.Any())
        {
            e.Handled = true;
        }
    }

    private void HandleBookmarkTag(object sender, ExecutedRoutedEventArgs e)
    {
        IWhiteboardItem whiteboardItem = ActiveItem ?? SelectedItems.First();
        if (DebugTools.AssertNotNull(whiteboardItem, null).Failed)
        {
            return;
        }

        string bookmarkTag = BookmarkTag.GetBookmarkTag(whiteboardItem as DependencyObject);
        BookmarkTagViewModel bookmarkTagViewModel = new BookmarkTagViewModel(bookmarkTag);
        if (DebugTools.AssertNotNull(_mathboardModel.BookmarksManager, null).Passed && BookmarkTagDialog.ShowBookmarkTagsDialog(bookmarkTagViewModel, Application.Current.MainWindow))
        {
            _mathboardModel.BookmarksManager.SetBookmarkTagCommand(whiteboardItem, bookmarkTag, bookmarkTagViewModel.Tag);
        }
    }

    private void CanRemoveBookmarkTag(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ActiveOrSelectedItems.Any((IWhiteboardItem item) => BookmarkTag.HasBookmarkTag(item as DependencyObject));
        if (ActiveOrSelectedItems.Any())
        {
            e.Handled = true;
        }
    }

    private void HandleRemoveBookmarkTag(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.AssertNotNull(ActiveOrSelectedItems, null).Failed || DebugTools.AssertNotNull(_mathboardModel.BookmarksManager, null).Failed)
        {
            return;
        }
        foreach (IWhiteboardItem activeOrSelectedItem in ActiveOrSelectedItems)
        {
            string bookmarkTag = BookmarkTag.GetBookmarkTag(activeOrSelectedItem as DependencyObject);
            _mathboardModel.BookmarksManager.RemoveBookmarkTagCommand(activeOrSelectedItem, bookmarkTag);
        }
    }

    protected override SeparateOverlappedItemsCommand CreateSeparateOverlappedItemsCommand(IEnumerable<IWhiteboardItem> itemsToSeparate, Dimension dimension)
    {
        return new SeparateOverlappedItemsCommand(this, itemsToSeparate, dimension, allowGrowthDownOrRight: true);
    }

    protected MathboardViewModel(IMathboardModel model, Size? gridCellSize)
        : base(model, handleUserInput: true, gridCellSize)
    {
        Initialize(model);
    }

    protected internal MathboardViewModel(IMathboardModel model, IAlignmentGrid grid)
        : base(model, grid)
    {
        Initialize(model);
    }

    public double ContainerFrameThickness
    {
        get => _containerFrameThickness;
        set => _containerFrameThickness = value;
    }

    protected MathboardViewModel(MathboardViewModel original)
        : base(original)
    {
        _mathboardModel = (IMathboardModel)_whiteboardModel;
        if (original.IsInPrinting)
        {
            _containerFrameThickness = 0.0;
        }
        _currentNumberFormat = ((original._currentNumberFormat != null) ? original._currentNumberFormat.Clone() : null);
        _currentMatrixFormat = ((original._currentMatrixFormat != null) ? original._currentMatrixFormat.Clone() : null);
        _currentTextFormat = ((original._currentTextFormat != null) ? original._currentTextFormat.Clone() : null);
        _hideFromCalculationButtonEnabled = original._hideFromCalculationButtonEnabled;
        _hideFromCalculationButtonChecked = original._hideFromCalculationButtonChecked;
        HookupEventHandlers();
    }

    protected void Initialize(IMathboardModel model)
    {
        _mathboardModel = model;
        MaximumSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        HookupEventHandlers();
    }

    private void HookupEventHandlers()
    {
        PropertyChanged += PropertyChangedHandler;
        _mathboardModel.CalculatableHiddenStatusChanged += UpdateTabStopStatus;
    }

    public IMathboardModel Model => _mathboardModel;

    public virtual void Commit()
    {
        WhiteboardItems.Each(Commit);
    }

    public void RecalculateCmdHandler()
    {
        if (ActiveItem != null)
        {
            Commit(ActiveItem);
        }
        else if (!SelectedItems.IsNullOrEmpty())
        {
            SelectedItems.Each(Commit);
        }
        else
        {
            Commit();
        }
    }

    public void Commit(IWhiteboardItem item)
    {
        _mathboardModel.CommitItem(item);
    }

    public bool CanToggleRegionsAsInput()
    {
        IInputRegion[] activeOrSelectedItemsOfType = GetActiveOrSelectedItemsOfType<IInputRegion>();
        if (_mathboardModel.IoRegionManager != null && activeOrSelectedItemsOfType.Length != 0)
        {
            return _mathboardModel.IoRegionManager.CanToggleRegionsAsInput(activeOrSelectedItemsOfType);
        }
        return false;
    }

    public void ToggleInputRegions()
    {
        IInputRegion[] activeOrSelectedItemsOfType = GetActiveOrSelectedItemsOfType<IInputRegion>();
        if (activeOrSelectedItemsOfType.Length != 0 && DebugTools.AssertNotNull(_mathboardModel.IoRegionManager, null).Passed)
        {
            _mathboardModel.IoRegionManager.ToggleInputRegions(activeOrSelectedItemsOfType);
        }
    }

    public bool CanToggleRegionsAsOutput()
    {
        IOutputRegion[] activeOrSelectedItemsOfType = GetActiveOrSelectedItemsOfType<IOutputRegion>();
        if (_mathboardModel.IoRegionManager != null && activeOrSelectedItemsOfType.Length != 0)
        {
            return _mathboardModel.IoRegionManager.CanToggleRegionsAsOutput(activeOrSelectedItemsOfType);
        }
        return false;
    }

    public void ToggleOutputRegions()
    {
        IOutputRegion[] activeOrSelectedItemsOfType = GetActiveOrSelectedItemsOfType<IOutputRegion>();
        if (activeOrSelectedItemsOfType.Length != 0 && DebugTools.AssertNotNull(_mathboardModel.IoRegionManager, null).Passed)
        {
            _mathboardModel.IoRegionManager.ToggleOutputRegions(activeOrSelectedItemsOfType);
        }
    }

    public new static class PropertyNames
    {
        public const string HideFromCalculationButtonEnabled = "HideFromCalculationButtonEnabled";
        public const string HideFromCalculationButtonChecked = "HideFromCalculationButtonChecked";
    }

    public virtual void ToggleHideFromCalculationContext()
    {
        List<IWhiteboardItem> targetChildren = new List<IWhiteboardItem>(ActiveOrSelectedItems);
        if (targetChildren.IsNullOrEmpty())
        {
            targetChildren.AddRange(WhiteboardItems);
        }
        IEnumerable<ICalculationHideableChild> calculatingChildren = from hideable in targetChildren.OfType<ICalculationHideableChild>()
            where !hideable.HideFromCalculationContext
            select hideable;
        CompoundCommand hideFromCalculationContext = new CompoundCommand();
        DelegateFunction0<bool> doMethod = () =>
        {
            using (new CommandScope(hideFromCalculationContext))
            {
                if (calculatingChildren.IsNotEmpty())
                {
                    calculatingChildren.Each((ICalculationHideableChild hideable) =>
                    {
                        hideable.HideFromCalculationContext = true;
                    });
                }
                else
                {
                    targetChildren.OfType<ICalculationHideableChild>().Each((ICalculationHideableChild hideable) =>
                    {
                        hideable.HideFromCalculationContext = false;
                    });
                }
            }
            UpdateHideFromCalculationButton();
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            hideFromCalculationContext.Undo();
            UpdateHideFromCalculationButton();
        };
        Delegate0 redoMethod = () =>
        {
            hideFromCalculationContext.Redo();
            UpdateHideFromCalculationButton();
        };
        CommandBuffer.Execute("Changing HideFromCalculationContext", doMethod, undoMethod, redoMethod);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public NumberFormat CurrentNumberFormat
    {
        get => _currentNumberFormat;
        set
        {
            _currentNumberFormat = value;
            OnPropertyChanged("CurrentNumberFormat");
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MatrixFormat CurrentMatrixFormat
    {
        get => _currentMatrixFormat;
        set
        {
            _currentMatrixFormat = value;
            OnPropertyChanged("CurrentMatrixFormat");
        }
    }

    public TextFormat CurrentTextFormat
    {
        get => _currentTextFormat;
        set
        {
            _currentTextFormat = value;
            OnPropertyChanged("CurrentTextFormat");
        }
    }

    public bool HideFromCalculationButtonEnabled
    {
        get => _hideFromCalculationButtonEnabled;
        set
        {
            _hideFromCalculationButtonEnabled = value;
            OnPropertyChanged("HideFromCalculationButtonEnabled");
        }
    }

    public bool HideFromCalculationButtonChecked
    {
        get => _hideFromCalculationButtonChecked;
        set
        {
            _hideFromCalculationButtonChecked = value;
            OnPropertyChanged("HideFromCalculationButtonChecked");
        }
    }

    private void OnActiveTextRegionContentChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBase richTextBase)
        {
            CurrentTextFormat = richTextBase.CurrentTextFormat;
        }
    }

    public override IWhiteboardItem ActiveItem
    {
        get => base.ActiveItem;
        set
        {
            if (value == base.ActiveItem)
            {
                return;
            }
            base.ActiveItem = value;
            UpdateHideFromCalculationButton();
        }
    }

    public bool IsInPrinting
    {
        get => _mathboardModel.IsInPrinting;
        set => _mathboardModel.IsInPrinting = value;
    }

    public override void Activate(bool isActive)
    {
        base.Activate(isActive);
        if (isActive)
        {
            UpdateHideFromCalculationButton();
        }
    }

    private void ActiveItemPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        if (sender is ICurrentMatrixFormatProperty currentMatrixFormatProperty && e.PropertyName == "CurrentMatrixFormat")
        {
            CurrentMatrixFormat = currentMatrixFormatProperty.CurrentMatrixFormat;
        }
    }

    protected override void HookupActiveItem(IWhiteboardItem item)
    {
        base.HookupActiveItem(item);
        item.PropertyChanged += ChildWhiteboardPropertyChangedHandler;
    }

    protected override void UnhookActiveItem(IWhiteboardItem item)
    {
        base.UnhookActiveItem(item);
        item.PropertyChanged -= ChildWhiteboardPropertyChangedHandler;
    }

    private void ChildWhiteboardPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "HideFromCalculationContext")
        {
            ICalculationHideableChild calculationHideableChild = ActiveItem as ICalculationHideableChild;
            DebugTools.AssertNotNull(calculationHideableChild, null);
            HideFromCalculationButtonEnabled = true;
            HideFromCalculationButtonChecked = calculationHideableChild.HideFromCalculationContext;
        }
    }

    private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsCrosshairVisible")
        {
            if (IsCrosshairVisible)
            {
                CurrentNumberFormat = null;
                CurrentMatrixFormat = null;
                CurrentTextFormat = null;
            }
        }
        else if ((e.PropertyName == "ContentWidth" || e.PropertyName == "ContentHeight") && IsCrosshairVisible)
        {
            Point insertionPoint = InsertionPoint;
            double x = insertionPoint.X;
            Size contentSize = ContentSize;
            if (x > contentSize.Width)
            {
                NavigateInto(Direction.Left, InsertionPoint);
            }
            insertionPoint = InsertionPoint;
            double y = insertionPoint.Y;
            contentSize = ContentSize;
            if (y > contentSize.Height)
            {
                NavigateInto(Direction.Up, InsertionPoint);
            }
        }
    }

    private T[] GetActiveOrSelectedItemsOfType<T>()
    {
        if (ActiveItem != null && ActiveItem is T)
        {
            return new T[1] { (T)ActiveItem };
        }
        return SelectedItems.OfType<T>().ToArray();
    }

    protected Pair<double, double> GetDefaultDimensions(double top, double minHeight)
    {
        double second = Math.Min(minHeight, ContentSize.Height - top);
        return new Pair<double, double>(top, second);
    }

    protected Pair<double, double> IndicatorRange(IEnumerable<IWhiteboardItem> sbCalculatables)
    {
        IWhiteboardItem whiteboardItem = sbCalculatables.First();
        IWhiteboardItem whiteboardItem2 = sbCalculatables.Last();
        double num = 0.0;
        if (whiteboardItem != null)
        {
            num = WhiteboardViewModel.WhiteboardItemMgr.GetItemLocation(whiteboardItem).Y + whiteboardItem.VerticalAlignmentOffset;
        }
        double num2 = ContentSize.Height;
        if (whiteboardItem2 != null)
        {
            num2 = WhiteboardViewModel.WhiteboardItemMgr.GetItemLocation(whiteboardItem2).Y + WhiteboardViewModel.WhiteboardItemMgr.GetDesiredSize(whiteboardItem2).Height + whiteboardItem2.VerticalAlignmentOffset;
        }
        return new Pair<double, double>(num, num2 - num);
    }

    protected virtual void UpdateHideFromCalculationButton()
    {
        List<ICalculationHideableChild> list = new List<ICalculationHideableChild>();
        if (ActiveItem == null && SelectedItems.Count() == 0)
        {
            if (WhiteboardItems != null)
            {
                WhiteboardItems.OfType<ICalculationHideableChild>().Each(list.Add);
            }
        }
        else
        {
            GetActiveOrSelectedItemsOfType<ICalculationHideableChild>().Each(list.Add);
        }
        int num = list.Count();
        IEnumerable<IHideFromCalculationButtonStatus> source = list.OfType<IHideFromCalculationButtonStatus>();
        bool hideFromCalculationButtonEnabled = num > 0 && (source.Count() < num || source.Any((IHideFromCalculationButtonStatus status) => status.HideFromCalculationButtonEnabled));
        bool hideFromCalculationButtonChecked = list.Count > 0;
        foreach (ICalculationHideableChild item in list)
        {
            if (!item.HideFromCalculationContext)
            {
                hideFromCalculationButtonChecked = false;
                break;
            }
        }
        HideFromCalculationButtonEnabled = hideFromCalculationButtonEnabled;
        HideFromCalculationButtonChecked = hideFromCalculationButtonChecked;
    }

    private IWhiteboardItem GetWhiteboardItem(ICalculatable calculatable)
    {
        if (calculatable is IWhiteboardItem result)
        {
            return result;
        }
        return WhiteboardItems.OfType<ICalculatableContainer>().First((ICalculatableContainer container) => container.CalculatableChildren.Contains(calculatable)) as IWhiteboardItem;
    }

    private void UpdateTabStopStatus(ICalculationHideableChild hideable)
    {
        IWhiteboardItem whiteboardItem = GetWhiteboardItem(hideable as ICalculatable);
        if (whiteboardItem == null)
        {
            return;
        }
        if (hideable.HideFromCalculationContext)
        {
            WhiteboardViewModel.WhiteboardItemMgr.SetIsTabNavigatable(whiteboardItem, false);
        }
        else
        {
            WhiteboardViewModel.WhiteboardItemMgr.SetIsTabNavigatable(whiteboardItem, true);
        }
    }

    CommandBindingCollection IWhiteboardViewModel.CommandBindings => base.CommandBindings;

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Mathboard.MathboardViewModel+RemoveMathboardItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class RemoveMathboardItemsCommand : RemoveItemsCommand
    {
        public RemoveMathboardItemsCommand(IWhiteboardViewModel viewModel, string commandName, IDictionary<IWhiteboardItem, Point> itemsAtLocations)
            : base(viewModel, commandName, itemsAtLocations)
        {
        }

        public override void Undo()
        {
            base.Undo();
            IMathboardViewModel viewModel = (IMathboardViewModel)_viewModel;
            _itemsAtLocations.Each((KeyValuePair<IWhiteboardItem, Point> itemLocation) =>
            {
                viewModel.Commit(itemLocation.Key);
            });
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Mathboard.MathboardViewModel+PasteMathboardItemsCommand, Ptc.Controls.Whiteboard")]
    public partial class PasteMathboardItemsCommand : PasteItemsCommand
    {
        public PasteMathboardItemsCommand(IWhiteboardViewModel viewModel, string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
            : base(viewModel, commandName, pasteItemsAtLocation)
        {
        }

        protected override bool DoWithResize()
        {
            bool result = base.DoWithResize();
            IMathboardViewModel viewModel = (IMathboardViewModel)_viewModel;
            _itemsAtLocation.Each((KeyValuePair<IWhiteboardItem, Point> itemLocation) =>
            {
                viewModel.Commit(itemLocation.Key);
            });
            return result;
        }
    }
}
