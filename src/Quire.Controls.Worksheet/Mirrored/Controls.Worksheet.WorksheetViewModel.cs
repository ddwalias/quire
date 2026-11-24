using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.ExcelComponent;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Text;
using Quire.Controls.Worksheet.Printing;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.IO;
using Quire.Wpf;
using Quire.Wpf.Printing;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetViewModel
{
    public bool DisplayGrid
    {
        get
        {
            return _displayGrid;
        }
        set
        {
            if (_displayGrid == value)
            {
                return;
            }
            _displayGrid = value;
            OnPropertyChanged(nameof(DisplayGrid));
            FireChanged();
        }
    }

    public bool DisplayHFGrid
    {
        get
        {
            return _displayHFGrid;
        }
        set
        {
            if (_displayHFGrid == value)
            {
                return;
            }
            _displayHFGrid = value;
            OnPropertyChanged(nameof(DisplayHFGrid));
            FireChanged();
        }
    }

    public bool DisplayFrame
    {
        get
        {
            return _displayFrame;
        }
        set
        {
            if (_displayFrame == value)
            {
                return;
            }
            _displayFrame = value;
            OnPropertyChanged(nameof(DisplayFrame));
            FireChanged();
        }
    }

    public bool DisplayHeaderFrame
    {
        get
        {
            return _displayHeaderFrame;
        }
        set
        {
            if (_displayHeaderFrame == value)
            {
                return;
            }
            _displayHeaderFrame = value;
            OnPropertyChanged(nameof(DisplayHeaderFrame));
            FireChanged();
        }
    }

    public bool DisplayFooterFrame
    {
        get
        {
            return _displayFooterFrame;
        }
        set
        {
            if (_displayFooterFrame == value)
            {
                return;
            }
            _displayFooterFrame = value;
            OnPropertyChanged(nameof(DisplayFooterFrame));
            FireChanged();
        }
    }

    public bool UseMajorGridLines
    {
        get
        {
            return _useMajorGrid;
        }
        set
        {
            if (_useMajorGrid == value)
            {
                return;
            }
            _useMajorGrid = value;
            OnPropertyChanged(nameof(UseMajorGridLines));
            FireChanged();
        }
    }

    public bool UseHFMajorGridLines
    {
        get
        {
            return _useHFMajorGridLines;
        }
        set
        {
            if (_useHFMajorGridLines == value)
            {
                return;
            }
            _useHFMajorGridLines = value;
            OnPropertyChanged(nameof(UseHFMajorGridLines));
            FireChanged();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetViewModel+WorksheetCommand, Ptc.Controls.Worksheet")]
    internal abstract partial class WorksheetCommand : global::Quire.Undo.IUndoableCommand
    {
        protected global::Quire.Controls.Worksheet.IWorksheetViewModel _viewModel;
        protected global::Quire.Controls.Worksheet.IWorksheetSection _activeSection;
        protected global::Quire.Controls.Worksheet.WorksheetViewModel ViewModel => _viewModel as global::Quire.Controls.Worksheet.WorksheetViewModel;
        public string Name { get; private set; }

        protected WorksheetCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel, string name)
        {
            _viewModel = viewModel;
            Name = name;
        }

        public virtual bool Do()
        {
            _activeSection = _viewModel.ActiveSection;
            return true;
        }

        public virtual void Undo() => ViewModel.ActiveSection = _activeSection;
        public virtual void Redo() => ViewModel.ActiveSection = _activeSection;
        public bool Execute() => global::Quire.Undo.CommandBuffer.Execute(this);
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetViewModel+AddWordToDictionaryCommand, Ptc.Controls.Worksheet")]
    internal partial class AddWordToDictionaryCommand : WorksheetCommand
    {
        private string _word;
        public AddWordToDictionaryCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel, string word)
            : base(viewModel, "Add word to Dictionary") => _word = word;
        public override bool Do() => ViewModel.AddWordToDictionary(_word);
        public override void Undo() => ViewModel.RemoveFromDictionary(_word);
        public override void Redo() => ViewModel.AddWordToDictionary(_word);
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetViewModel+AddItemAtInsertionPointAndActivateCommand, Ptc.Controls.Worksheet")]
    internal partial class AddItemAtInsertionPointAndActivateCommand : WorksheetCommand
    {
        private readonly global::Quire.Controls.Core.IWhiteboardItem _item;
        private global::System.Windows.Point _insertionPoint;
        private readonly bool _shouldInsertionPointOnUndo;

        public AddItemAtInsertionPointAndActivateCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel, string name, bool shouldInsertionPointOnUndo = true) : base(viewModel, name) => _shouldInsertionPointOnUndo = shouldInsertionPointOnUndo;
        public AddItemAtInsertionPointAndActivateCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel, global::Quire.Controls.Core.IWhiteboardItem item, string name, bool shouldInsertionPointOnUndo = true) : base(viewModel, name) { _item = item; _shouldInsertionPointOnUndo = shouldInsertionPointOnUndo; }
        public AddItemAtInsertionPointAndActivateCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel, global::Quire.Controls.Core.IWhiteboardItem item, string name, bool shouldInsertionPointOnUndo, global::System.Windows.Point insertionPoint) : base(viewModel, name) { _item = item; _insertionPoint = insertionPoint; _shouldInsertionPointOnUndo = shouldInsertionPointOnUndo; }
        public override bool Do() { base.Do(); if (_shouldInsertionPointOnUndo) _insertionPoint = _viewModel.InsertionPoint; AddItem(); return true; }
        public override void Undo() { base.Undo(); ViewModel.RemoveActiveSectionItemAndRestoreInsertionPoint(_item, _insertionPoint); }
        public override void Redo() { base.Redo(); AddItem(); }
        private void AddItem() { ViewModel.AddItemAtLocation(_item, _insertionPoint); _item.Activate(isActive: true); ViewModel.ActiveSection.ActiveItem = _item; }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetViewModel+RemoveActiveItemCommand, Ptc.Controls.Worksheet")]
    internal partial class RemoveActiveItemCommand : WorksheetCommand
    {
        private global::Quire.Controls.Core.IWhiteboardItem _item;
        private global::System.Windows.Point _itemLocation;
        public RemoveActiveItemCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel) : base(viewModel, "Remove Active Item") { }
        public override bool Do() { base.Do(); _item = _viewModel.ActiveItem; if (_item != null) { _itemLocation = _viewModel.GetWorksheetSectionItemLocation(_item); RemoveItem(); } return true; }
        public override void Undo() { base.Undo(); new AddItemAtInsertionPointAndActivateCommand(_viewModel, _item, "Undo RemoveActiveItemCommand", false, _itemLocation).Do(); _viewModel.WorksheetContent.CommitItem(_item); }
        public override void Redo() { base.Redo(); RemoveItem(); }
        private void RemoveItem() { ViewModel._model.RemoveItem(_item); _viewModel.InsertionPoint = _itemLocation; }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetViewModel+CreateTextRegionFromEquationControlCommand, Ptc.Controls.Worksheet")]
    internal partial class CreateTextRegionFromEquationControlCommand : WorksheetCommand
    {
        private global::Quire.Controls.Core.IWhiteboardItem _mathRegion;
        private readonly global::Quire.Controls.Core.IWhiteboardItem _textRegion;
        private global::System.Windows.Point _insertionPoint;
        private global::System.Windows.Point _itemLocation;
        public CreateTextRegionFromEquationControlCommand(global::Quire.Controls.Worksheet.IWorksheetViewModel viewModel, global::Quire.Controls.Core.IWhiteboardItem textRegion, global::Quire.Controls.Core.IWhiteboardItem mathRegion) : base(viewModel, "create text region from equation control") { _textRegion = textRegion; _mathRegion = mathRegion; }
        public override bool Do() { base.Do(); if (_mathRegion != null) { _itemLocation = _viewModel.GetWorksheetSectionItemLocation(_mathRegion); RemoveItem(_mathRegion); } _insertionPoint = _viewModel.InsertionPoint; AddItem(_textRegion); _viewModel.WorksheetContent.CommitItem(_textRegion); return true; }
        private void AddItem(global::Quire.Controls.Core.IWhiteboardItem control) { ViewModel.AddItemAtLocation(control, _insertionPoint); control.Activate(isActive: true); ViewModel.ActiveSection.ActiveItem = control; }
        public override void Undo() { base.Undo(); RemoveItem(_textRegion); AddItem(_mathRegion); _viewModel.WorksheetContent.CommitItem(_mathRegion); }
        public override void Redo() { base.Redo(); RemoveItem(_mathRegion); AddItem(_textRegion); _viewModel.WorksheetContent.CommitItem(_textRegion); }
        private void RemoveItem(global::Quire.Controls.Core.IWhiteboardItem control) { ViewModel.ActiveSection.RemoveItem(control); _viewModel.InsertionPoint = _itemLocation; }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Worksheet.WorksheetViewModel+CreateTextRegionFromWhiteboardControlCommand, Ptc.Controls.Worksheet")]
    internal partial class CreateTextRegionFromWhiteboardControlCommand : WorksheetCommand
    {
        private readonly global::Quire.Controls.Core.IWhiteboardItem _mathRegion;
        private readonly global::Quire.Controls.Core.IWhiteboardItem _textRegion;
        private new readonly global::Quire.Controls.Mathboard.IMathboardViewModel _viewModel;
        public CreateTextRegionFromWhiteboardControlCommand(global::Quire.Controls.Mathboard.IMathboardViewModel viewModel, global::Quire.Controls.Core.IWhiteboardItem textRegion, global::Quire.Controls.Core.IWhiteboardItem mathRegion) : base(null, null) { _textRegion = textRegion; _mathRegion = mathRegion; _viewModel = viewModel; }
        public override bool Do() { if (_mathRegion != null) RemoveItem(_mathRegion); AddItem(_textRegion); return true; }
        private void AddItem(global::Quire.Controls.Core.IWhiteboardItem control) => _viewModel.AddItemAndActivate(control);
        public override void Undo() { RemoveItem(_textRegion); AddItem(_mathRegion); }
        public override void Redo() { RemoveItem(_mathRegion); AddItem(_textRegion); }
        private void RemoveItem(global::Quire.Controls.Core.IWhiteboardItem control) => _viewModel.RemoveWhiteboardItem(control);
    }
    public event global::System.EventHandler Changed;
    public event global::System.Action StartingPageNumberChanged;
    public event global::System.Action CurrentElementChanged;
    public event global::System.Action DictionaryUpdated;

    private global::Quire.Core.TextFormat _contentTextFormat;
    private readonly global::Quire.Controls.Worksheet.IWorksheetModel _model;
    private const string SpaceKeyCode = " ";
    private readonly global::Quire.Controls.Worksheet.WorksheetViewModel.WorksheetMouseDownHandler _worksheetMouseDownHandler;
    private readonly global::Quire.Controls.Worksheet.IWorksheetPageManager _pageManager;
    private readonly global::Quire.Controls.Worksheet.IWorksheetPasteHandler _pasteHandler;
    private int _pageCount;
    private readonly global::Quire.Controls.Core.IRestrictedAreaAlignmentGrid _worksheetGrid;
    private global::Quire.Controls.Core.PageLayoutMode _viewMode;
    private global::Quire.Controls.Worksheet.IWorksheetSection _activeSection;
    private bool _isContentActive;
    private global::Quire.Controls.Worksheet.IWorksheetPage _activePage;
    private double _widthWithNonprintableSpace;
    private global::System.Windows.Thickness _nonprintableSpaceButtonPosition;
    private double _nonprintableSpaceButtonHeight;
    private bool _displayGrid;
    private bool _displayHFGrid;
    private bool _displayFrame;
    private bool _displayHeaderFrame;
    private bool _displayFooterFrame;
    private bool _useMajorGrid;
    private bool _useHFMajorGridLines;
    private bool _hasSelection;
    private bool _isInLimbo;
    private int _currentPageNumber;
    private bool _enableEditCurrentPageNumber;
    private bool _hideFromCalculationButtonEnabled;
    private bool _hideFromCalculationButtonChecked;
    private bool _oleObjectAutoResize;
    private string _displayFilePath;
    private bool _isSolveBlockLabelVisible;
    private global::System.Windows.Markup.XmlLanguage _currentProofingLanguage;
    private global::Quire.Controls.Worksheet.DisposeStatus _disposeStatus;
    private bool _isRegionBorderShown;
    private static readonly global::System.Collections.Generic.IDictionary<global::System.Type, global::System.Collections.Generic.IEnumerable<global::System.Type>> _whiteboardExclusionMap;
    private static readonly global::System.Collections.Generic.IDictionary<global::System.Type, global::System.Collections.Generic.IEnumerable<string>> _operatorExclusionMap;
    private static readonly global::System.Collections.Generic.IEnumerable<global::System.Type> _gridRestrictionTypes;
    private static global::Quire.Controls.Worksheet.IWorksheetItemManager _itemManager;
    private global::Quire.Controls.Core.ITextStylesManager _textStylesManager;
    private static global::Quire.Controls.Worksheet.IControlFactory _controlCreator;
    private static global::Quire.Services.IMouseFacade _mouseFacadeService;
    private int _startingPageNumber;
    private int _offset;
    private bool _isDifferentFirstPage;
    private double _worksheetHeight;
    private double _headerTop;
    private double _footerTop;
    private global::Quire.Controls.Worksheet.WorksheetDragger _worksheetDragHandler;
    private bool _preRecalculateAll;
    private global::System.Windows.ResourceDictionary Resources;
    private readonly global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IWhiteboardItem, global::System.Collections.Generic.IList<global::Quire.Controls.Core.Whiteboard.WhiteboardItemSizeChangedEventHandler>> _itemSizeChangedHandlerMap;
    private string _textCache;
    private bool _headerFooterZombiesReady;
    public ObservableRangeCollection<IWorksheetPage> VisiblePages => _pageManager.VisiblePages;

    public ObservableRangeCollection<IWorksheetPage> Pages => _pageManager.Pages;

    public int TrailingEmptyPageCount => _pageManager.GetTrailingEmptyPageCount();

    public bool IsAutoCalcChecked => IsAutoCalcOn;

    public bool IsCalculating => _model.IsCalculating;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UNITTEST_Evaluate()
    {
        Evaluate();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UNITTEST_DirectionalMoveHandler(Direction direction)
    {
        DirectionalMoveHandler(direction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UNITTEST_CanMoveInDirection(Direction direction)
    {
        return CanMoveInDirection(direction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UNITTEST_CanPaste()
    {
        return CanPaste();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UNITTEST_Paste()
    {
        Paste();
    }

    public void HandleSelectAll()
    {
        ActiveSection.SelectAllItems();
    }

    public void ToggleItemSelection(IWhiteboardItem item, bool includeActiveItem)
    {
        ActiveSection.ToggleItemSelection(item, includeActiveItem);
    }

    public void SelectContiguous(IWhiteboardItem item)
    {
        ActiveSection.SelectContiguous(item);
    }

    public void SelectItem(IWhiteboardItem item)
    {
        ActiveSection.SelectItem(item);
    }

    public void SelectItems(IEnumerable<IWhiteboardItem> items)
    {
        ActiveSection.SelectItems(items);
    }

    public void ClearSelection()
    {
        ActiveSection.ClearSelection();
    }

    public void PageDown()
    {
        MoveByPage(Direction.Down);
    }

    public void PageUp()
    {
        MoveByPage(Direction.Up);
    }

    public bool HasScriptableControls { get; set; }

    private EngineError CurrentErrorChain { get; set; }

    public IHeaderFooterControl Header { get; private set; }

    public IHeaderFooterControl Footer { get; private set; }

    public IHeaderFooterControl FirstPageHeader { get; private set; }

    public IHeaderFooterControl FirstPageFooter { get; private set; }

    public bool IsSavedWorksheet { private get; set; }

    public IWorksheetContent WorksheetContent { get; private set; }

    public NumberFormat DisplayNumberFormat { get; set; }

    public MatrixFormat ResultMatrixFormat { get; set; }

    public DragMode CurrentDragMode { get; set; }

    private void CollapseIfPresent(IHeaderFooterZombie zombie)
    {
        zombie?.Collapse();
    }

    private void ShowIfPresent(IHeaderFooterZombie zombie)
    {
        zombie?.Show();
    }

    private void ResetVisualBrushIfPresent(IHeaderFooterZombie zombie)
    {
        zombie?.ResetVisualBrush();
    }

    private void OnStyleCollectionChanged()
    {
        FireChanged();
    }

    public bool IsValidInsertionPoint(Point point)
    {
        return WorksheetContent.IsValidInsertionPoint(point);
    }

    private bool CanCreateOleObjectControl()
    {
        return CanCreateControlOfType<IOleObjectControl>();
    }

    private bool CanCreateChartComponent()
    {
        return CanCreateControlOfType<IChartComponentControl>();
    }

    public IWorksheetPageBreak CreatePageBreakControl()
    {
        return ControlCreator.CreatePageBreakControl();
    }

    public void HandleToggleAutoCalculation(bool newState)
    {
        IsAutoCalcOn = newState;
        FireChanged();
    }

    public bool CanReCalculateAll()
    {
        return IsCalculationsEnabled;
    }

    public bool CanRemoveSpace(Direction direction)
    {
        return CanDeleteVerticalSpace(direction);
    }

    private bool CanCopySelection()
    {
        return SelectedItems.Count() > 0;
    }

    private bool CanCutSelection()
    {
        return SelectedItems.Count() > 0;
    }

    private Rect GetItemRect(IWhiteboardItem item)
    {
        return ItemManager.GetItemRect(item);
    }

    private bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        return ItemManager.ItemsOverlapped(items);
    }

    private static bool IsPageWideSizablePredicate(IWhiteboardItem item)
    {
        return item is IPageWideSizable;
    }

    public void ImportData(string fileName)
    {
        InsertFileName(fileName);
    }

    public IDictionary<IdLabels, ExtendedFont> GetDefaultLabeledIdFormat()
    {
        return VisualHelper.GetDefaultLabeledIdFormat();
    }

    private IEnumerable<IIORegion> GetInputRegions()
    {
        return GetIORegions<IInputRegion>();
    }

    private IEnumerable<IIORegion> GetOutputRegions()
    {
        return GetIORegions<IOutputRegion>();
    }

    public void HandleSetCurrentProofingLanguage(CultureInfo culture)
    {
        CurrentProofingLanguage = XmlLanguageHelper.ConvertCultureInfoToXmlLanguage(culture);
    }

    public void OnRequestRestoreInsertionPoint(IWorksheetControl worksheetControl, RoutedEventArgs eventArgs)
    {
        Activate(worksheetControl);
        eventArgs.Handled = true;
    }

    public bool IsContentActive
    {
        get
        {
            return _isContentActive;
        }
        set
        {
            if (_isContentActive == value)
            {
                return;
            }
            _isContentActive = value;
            OnPropertyChanged("IsContentActive");
        }
    }

    public bool IsHeaderActive
    {
        get
        {
            if (ActiveSection != Header)
            {
                return ActiveSection == FirstPageHeader;
            }
            return true;
        }
    }

    public bool IsFooterActive
    {
        get
        {
            if (ActiveSection != Footer)
            {
                return ActiveSection == FirstPageFooter;
            }
            return true;
        }
    }

    public Point InsertionPoint
    {
        get
        {
            return ActiveSection.InsertionPoint;
        }
        set
        {
            ActiveSection.InsertionPoint = value;
        }
    }

    public IWhiteboardItem ActiveItem
    {
        get
        {
            if (ActiveSection == null)
            {
                return null;
            }
            return ActiveSection.ActiveItem;
        }
    }

    public IWhiteboardItem ActiveDescendant
    {
        get
        {
            if (ActiveSection == null)
            {
                return null;
            }
            return ActiveSection.ActiveDescendant;
        }
    }

    public IList<IWhiteboardItem> SelectedDescendants
    {
        get
        {
            if (ActiveSection == null)
            {
                return null;
            }
            return ActiveSection.SelectedDescendants;
        }
    }

    public int PageCount
    {
        get
        {
            return _pageCount;
        }
        private set
        {
            _pageCount = value;
            OnPropertyChanged("PageCount");
        }
    }

    public int StartingPageNumber
    {
        get
        {
            return _startingPageNumber;
        }
        set
        {
            _startingPageNumber = value;
            OnPropertyChanged("StartingPageNumber");
            FireStartingPageNumberChanged();
            FireChanged();
        }
    }

    public int TotalPagesOffset
    {
        get
        {
            return _offset;
        }
        set
        {
            _offset = value;
            OnPropertyChanged("TotalPagesOffset");
        }
    }

    public double WorksheetHeight
    {
        get
        {
            return _worksheetHeight;
        }
        private set
        {
            if (_worksheetHeight == value)
            {
                return;
            }
            _worksheetHeight = value;
            OnPropertyChanged("WorksheetHeight");
        }
    }

    public double HeaderTop
    {
        get
        {
            return _headerTop;
        }
        private set
        {
            if (_headerTop == value)
            {
                return;
            }
            _headerTop = value;
            OnPropertyChanged("HeaderTop");
        }
    }

    public double FooterTop
    {
        get
        {
            return _footerTop;
        }
        private set
        {
            if (_footerTop == value)
            {
                return;
            }
            _footerTop = value;
            OnPropertyChanged("FooterTop");
        }
    }

    public double WorksheetPageLayoutDisplayHeight
    {
        get
        {
            if (WorksheetPageLayoutMode != PageLayoutMode.PageMode)
            {
                return PageLayout.PageBodyHeight;
            }
            return PageLayout.DisplayHeight;
        }
    }

    public double NonprintableSpaceButtonHeight
    {
        get
        {
            return _nonprintableSpaceButtonHeight;
        }
        private set
        {
            if (_nonprintableSpaceButtonHeight == value)
            {
                return;
            }
            _nonprintableSpaceButtonHeight = value;
            OnPropertyChanged("NonprintableSpaceButtonHeight");
        }
    }

    public TextFormat HeaderTextFormat
    {
        get
        {
            return Header.TextFormat;
        }
        set
        {
            Header.TextFormat = value;
        }
    }

    public TextFormat FooterTextFormat
    {
        get
        {
            return Footer.TextFormat;
        }
        set
        {
            Footer.TextFormat = value;
        }
    }

    public TextFormat FirstPageHeaderTextFormat
    {
        get
        {
            return FirstPageHeader.TextFormat;
        }
        set
        {
            FirstPageHeader.TextFormat = value;
        }
    }

    public TextFormat FirstPageFooterTextFormat
    {
        get
        {
            return FirstPageFooter.TextFormat;
        }
        set
        {
            FirstPageFooter.TextFormat = value;
        }
    }

    public double WidthWithNonprintableSpace
    {
        get
        {
            return _widthWithNonprintableSpace;
        }
        private set
        {
            if (_widthWithNonprintableSpace == value)
            {
                return;
            }
            _widthWithNonprintableSpace = value;
            OnPropertyChanged("WidthWithNonprintableSpace");
        }
    }

    public bool HasSelection
    {
        get
        {
            return _hasSelection;
        }
        set
        {
            if (_hasSelection == value)
            {
                return;
            }
            _hasSelection = value;
            OnPropertyChanged("HasSelection");
        }
    }

    public bool IsInLimbo
    {
        get
        {
            return _isInLimbo;
        }
        set
        {
            if (_isInLimbo == value)
            {
                return;
            }
            _isInLimbo = value;
            OnPropertyChanged("IsInLimbo");
        }
    }

    public bool HideFromCalculationButtonEnabled
    {
        get
        {
            return _hideFromCalculationButtonEnabled;
        }
        set
        {
            _hideFromCalculationButtonEnabled = value;
            OnPropertyChanged("HideFromCalculationButtonEnabled");
        }
    }

    public bool HideFromCalculationButtonChecked
    {
        get
        {
            return _hideFromCalculationButtonChecked;
        }
        set
        {
            _hideFromCalculationButtonChecked = value;
            OnPropertyChanged("HideFromCalculationButtonChecked");
        }
    }

    public WorksheetDragger WorksheetDragHandler
    {
        get
        {
            return _worksheetDragHandler;
        }
        set
        {
            _worksheetDragHandler = value;
            OnPropertyChanged("WorksheetDragHandler");
        }
    }

    public bool PreRecalculateAll
    {
        get
        {
            return _preRecalculateAll;
        }
        set
        {
            _preRecalculateAll = value;
            OnPropertyChanged("PreRecalculateAll");
        }
    }

    public StaticMathFormat ContentMathFormat
    {
        get
        {
            return WorksheetContent.MathFormat;
        }
        set
        {
            WorksheetContent.MathFormat = value;
        }
    }

    public bool EnableEditCurrentPageNumber
    {
        get
        {
            return _enableEditCurrentPageNumber;
        }
        set
        {
            _enableEditCurrentPageNumber = value;
            OnPropertyChanged("EnableEditCurrentPageNumber");
        }
    }

    public bool HeaderFooterZombiesReady
    {
        get
        {
            return _headerFooterZombiesReady;
        }
        set
        {
            _headerFooterZombiesReady = value;
            OnPropertyChanged("HeaderFooterZombiesReady");
        }
    }

    public bool IsRegionBorderShown
    {
        get
        {
            return _isRegionBorderShown;
        }
        set
        {
            if (_isRegionBorderShown == value)
            {
                return;
            }
            _isRegionBorderShown = value;
            OnPropertyChanged("IsRegionBorderShown");
        }
    }

    private void FireStartingPageNumberChanged()
    {
        StartingPageNumberChanged?.Invoke();
    }

    public void UpdateTotalPagesOffsetOnPageChange(int updatedPageNumberDifference)
    {
        if (TotalPagesOffset != 0)
        {
            TotalPagesOffset = Math.Max(0, TotalPagesOffset - updatedPageNumberDifference);
        }
    }

    private void UpdateWorksheetHeight()
    {
        WorksheetHeight = WorksheetPageLayoutDisplayHeight * (double)Pages.Count;
    }

    public void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems)
    {
        WorksheetContent.SetCopyWhiteboardItemsDelegate(copyWhiteboardItems);
    }

    public IPageSnapshot GetPageSnapshot(int pageIndex)
    {
        return VisualHelper.CreatePageSnapshot(this, pageIndex);
    }

    public PageIndexRange GetPagesContaining(IWhiteboardItem item)
    {
        return _pageManager.GetPagesContaining(item);
    }

    public IList<IWhiteboardItem> GetWorksheetItemParts(IWhiteboardItem item)
    {
        return _pageManager.GetWorksheetItemParts(item);
    }

    public bool IsCalculableHiddenFromCalculationContext(ICalculatable calc)
    {
        if (calc is ICalculationHideableChild calculationHideableChild)
        {
            return calculationHideableChild.HideFromCalculationContext;
        }
        return false;
    }

    public Point GetWorksheetSectionItemLocation(IWhiteboardItem item)
    {
        return ActiveSection.GetItemLocation(item);
    }

    public void SetWorksheetSectionItemLocation(IWhiteboardItem item, Point worksheetLocation)
    {
        ActiveSection.SetItemLocation(item, worksheetLocation);
    }

    public bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item)
    {
        return ItemManager.GetIgnoreForHorizontalMovementAndWidth(item);
    }

    public bool CanInsertSpace()
    {
        if (!IsContentActive)
        {
            return false;
        }
        return ActiveOrSelectedItems.Count() == 0;
    }

    public int GetItemWorksheetPageIndex(IWhiteboardItem item)
    {
        return _pageManager.GetPageIndex(item);
    }

    public void ToggleInputRegions()
    {
        _model.InputOutputRegionManager.ToggleInputRegions(GetInputRegions());
    }

    public void ToggleOutputRegions()
    {
        _model.InputOutputRegionManager.ToggleOutputRegions(GetOutputRegions());
    }

    private bool CanDeleteSelectedItems()
    {
        return !SelectedItems.IsNullOrEmpty();
    }

    private bool CanDeleteActiveOrSelectedItems()
    {
        return !ActiveOrSelectedItems.IsNullOrEmpty();
    }

    private bool CanDeleteActiveItem()
    {
        if (IsContentActive)
        {
            return ActiveItem != null;
        }
        return false;
    }

    private bool CanToggleHideFromCalculationContext()
    {
        return HideFromCalculationButtonEnabled;
    }

    private bool CanBookmarkTag()
    {
        if (!IsHeaderActive && !IsFooterActive)
        {
            return ActiveOrSelectedItems.Count() == 1;
        }
        return false;
    }

    private bool ErrorCanTraceBackward()
    {
        if (CurrentError != null && CurrentError.OriginalError != null)
        {
            return true;
        }
        return false;
    }

    private bool ErrorCanTraceForward()
    {
        if (CurrentError != null && CurrentErrorChain != null && CurrentErrorChain.FindNextError(CurrentError) != null)
        {
            return true;
        }
        return false;
    }

    private void UpdateHasSelectionProperty()
    {
        HasSelection = SelectedDescendants.Any();
    }

    private void UpdateIsInLimboProperty()
    {
        IsInLimbo = ActiveDescendant == null || ActiveDescendant is IWhiteboardControl;
    }

    private bool CanAddToDictionary()
    {
        if (ActiveDescendant is ISpellCheckRegion spellCheckRegion)
        {
            return spellCheckRegion.CanAddToDictionary();
        }
        return false;
    }

    internal bool AddWordToDictionary(string word)
    {
        if (!DictionaryHelper.AddWordToDictionary(word))
        {
            return false;
        }
        RefreshDictionary();
        FireDictionaryUpdated();
        return true;
    }

    internal void RemoveFromDictionary(string word)
    {
        if (!DictionaryHelper.RemoveFromDictionary(word))
        {
            return;
        }
        RefreshDictionary();
        FireDictionaryUpdated();
    }

    private void FireDictionaryUpdated()
    {
        DictionaryUpdated?.Invoke();
    }

    private void HandleSelectedDescendantsChanged()
    {
        if (IsContentActive)
        {
            UpdateIsRegionBorderShownState();
            OnPropertyChanged("SelectedDescendants");
        }
    }

    protected void FireChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void ActivateContent()
    {
        MakeContentActive();
        WorksheetContent.RestoreVisualState();
    }

    public void ToggleShowGrid()
    {
        if (IsContentActive)
        {
            DisplayGrid = !DisplayGrid;
        }
        else if (IsHeaderActive || IsFooterActive)
        {
            DisplayHFGrid = !DisplayHFGrid;
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
    }

    public void ToggleShowFrame()
    {
        if (IsContentActive)
        {
            DisplayFrame = !DisplayFrame;
        }
        else if (IsHeaderActive)
        {
            DisplayHeaderFrame = !DisplayHeaderFrame;
        }
        else if (IsFooterActive)
        {
            DisplayFooterFrame = !DisplayFooterFrame;
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
    }

    private void ToggleShowMajorGridlines()
    {
        if (IsContentActive)
        {
            UseMajorGridLines = !UseMajorGridLines;
        }
        else
        {
            UseHFMajorGridLines = !UseHFMajorGridLines;
        }
    }

    private bool CanToggleIsSolveBlockLabelVisible()
    {
        return !SelectedDescendants.Any();
    }

    public void HandleStopRunningCalculations()
    {
        IsCalculationsEnabled = false;
        FireChanged();
        if (IsAutoCalcOn)
        {
            IsAutoCalcOn = false;
        }
        IsCalculationsEnabled = true;
        FireChanged();
    }

    public void FireCalculateNow()
    {
        if (IsAutoCalcOn || IsCalculating)
        {
            return;
        }
        if (ActiveItem != null)
        {
            IWorksheetControl worksheetControl = VisualHelper.FindWorksheetControlAncestor(ActiveItem);
            if (worksheetControl != null && VisualHelper.IsItemInView(ActiveItem, worksheetControl))
            {
                _model.FireCalculateRegionNow(ActiveItem);
            }
        }
        _model.LastVisibleCalculatable = GetLastVisibleCalculatable();
        _model.FireCalculateNow();
    }

    public void GoToNextError()
    {
        if (ErrorCanTraceForward())
        {
            for (EngineError engineError = CurrentErrorChain.FindNextError(CurrentError); engineError != null; engineError = CurrentErrorChain.FindNextError(engineError))
            {
                if (_model.ContainsError(engineError))
                {
                    _model.ShowError(engineError);
                    break;
                }
            }
        }
    }

    public void GoToFirstError()
    {
        if (ErrorCanTraceBackward())
        {
            if (CurrentErrorChain == null || !CurrentErrorChain.IsAncestorOf(CurrentError))
            {
                CurrentErrorChain = CurrentError;
            }
            EngineError engineError = CurrentError;
            while (engineError.OriginalError != null)
            {
                engineError = engineError.OriginalError;
            }
            _model.ShowError(engineError);
        }
    }

    public void GoToPreviousError()
    {
        if (ErrorCanTraceBackward())
        {
            if (CurrentErrorChain == null || !CurrentErrorChain.IsAncestorOf(CurrentError))
            {
                CurrentErrorChain = CurrentError;
            }
            for (EngineError originalError = CurrentError.OriginalError; originalError != null; originalError = originalError.OriginalError)
            {
                if (_model.ContainsError(originalError))
                {
                    _model.ShowError(originalError);
                    break;
                }
            }
        }
    }

    public void GoToLastError()
    {
        if (ErrorCanTraceForward() && CurrentErrorChain != null)
        {
            _model.ShowError(CurrentErrorChain);
        }
    }

    public void MoveItem(IWhiteboardItem item, Point point)
    {
        Point worksheetSectionItemLocation = GetWorksheetSectionItemLocation(item);
        Vector offset = point - worksheetSectionItemLocation;
        MoveItemsBy(new[] { item }, offset);
    }

    public void InsertFileName(string fileName)
    {
        CalculationVariables docCalculationVariables = _model.DocCalculationVariables;
        if (docCalculationVariables != null)
        {
            string parameter = FileUtils.EvaluateRelativePath(docCalculationVariables.CurrentWorkingDirectory, fileName, stripSingleLeadingDot: false);
            if (ActiveOrSelectedItems.Count() == 0)
            {
                CreateNewEquation(EquationCommands.InsertString, parameter);
            }
            else if (ActiveDescendant != null)
            {
                EquationCommands.InsertString.Execute(parameter, ActiveDescendant as IInputElement);
            }
        }
    }

    public Point GetDescendantInsertionPoint()
    {
        if (SelectedItems.Any())
        {
            return default;
        }
        if (ActiveItem is IWhiteboardControl whiteboardControl)
        {
            Point descendantInsertionPoint = whiteboardControl.GetDescendantInsertionPoint();
            Point itemWorksheetLocation = ItemManager.GetItemWorksheetLocation(ActiveItem);
            return new Point(itemWorksheetLocation.X + descendantInsertionPoint.X, itemWorksheetLocation.Y + descendantInsertionPoint.Y);
        }
        if (ActiveItem != null)
        {
            return ItemManager.GetItemWorksheetLocation(ActiveItem);
        }
        return InsertionPoint;
    }

    public bool CanToggleRegionsAsInput()
    {
        if (!CanToggleIORegions<IInputRegion>())
        {
            return false;
        }
        IEnumerable<IIORegion> inputRegions = GetInputRegions();
        if (inputRegions.Count() == 0)
        {
            return false;
        }
        return _model.InputOutputRegionManager.CanToggleRegionsAsInput(inputRegions);
    }

    public bool CanToggleRegionsAsOutput()
    {
        if (!CanToggleIORegions<IOutputRegion>())
        {
            return false;
        }
        IEnumerable<IIORegion> outputRegions = GetOutputRegions();
        if (outputRegions.Count() == 0)
        {
            return false;
        }
        return _model.InputOutputRegionManager.CanToggleRegionsAsOutput(outputRegions);
    }

    private void ToggleHeaderActivation()
    {
        if (!IsHeaderActive)
        {
            ActivateHeader();
        }
        else
        {
            ActivateContent();
        }
    }

    private void ToggleFooterActivation()
    {
        if (!IsFooterActive)
        {
            ActivateFooter();
        }
        else
        {
            ActivateContent();
        }
    }

    public void PopulateZombies()
    {
        foreach (IWorksheetPage page in Pages)
        {
            DebugTools.AssertNotNull(page.HeaderZombie, null);
            DebugTools.AssertNotNull(page.FooterZombie, null);
        }
    }
}
