using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;
using Quire.Services;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Quire.Controls.Core.Protection;
using Quire.Controls.Core.IORegions;
using Quire.Controls.ExcelComponent;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Text;
using Quire.Controls.Worksheet.Printing;
using Quire.Undo;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Include;
using Quire.Controls.CollapsibleArea;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Mathboard;
using Quire.Controls.Plots;
using Quire.Controls.Plots.View;
using Quire.Core;
using Quire.IO;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Wpf;
using Quire.Wpf.Printing;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetViewModel
{

private const string SpaceKeyCode = " ";

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

public WorksheetViewModel(global::Quire.Controls.Worksheet.IWorksheetModel model, global::Quire.Controls.Core.IRestrictedAreaAlignmentGrid grid, global::Quire.Controls.Worksheet.IWorksheetContent content, global::Quire.Controls.Worksheet.IWorksheetPageManager pageManager, global::Quire.Controls.Worksheet.IWorksheetPasteHandler pasteHandler) { }

public bool UNITTEST_CanCreateControlOfType<T>()
    {
        return CanCreateControlOfType<T>();
    }

public bool UNITTEST_CanCreatePageNumberControl()
    {
        return CanCreatePageNumberControl();
    }

public bool UNITTEST_CanCreateParagraphControl()
    {
        return CanCreateParagraphControl();
    }

public bool UNITTEST_CanCreateFileSpecificRegion<T>()
    {
        return CanCreateFileSpecificRegion<T>();
    }

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

internal static global::Quire.Controls.Worksheet.IWorksheetItemManager ItemManager { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

internal static global::Quire.Controls.Worksheet.IControlFactory ControlCreator { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public bool HasScriptableControls { get; set; }

internal static global::Quire.Services.IMouseFacade MouseFacadeService { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

static WorksheetViewModel() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public global::Quire.Controls.Worksheet.IWorksheetPage ActivePage { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); private set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

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

private void RestoreVisualBrushForAllPages()
    {
        ItemManager.ExecuteOnLoad(Header, () => Pages.Each(page =>
        {
            if (page.HeaderZombie != null)
            {
                page.HeaderZombie.RestoreVisualBrush();
            }
        }));
        ItemManager.ExecuteOnLoad(Footer, () => Pages.Each(page =>
        {
            if (page.FooterZombie != null)
            {
                page.FooterZombie.RestoreVisualBrush();
            }
        }));
        ItemManager.ExecuteOnLoad(FirstPageHeader, () => Pages.Each(page =>
        {
            if (page.FirstPageHeaderZombie != null)
            {
                page.FirstPageHeaderZombie.RestoreVisualBrush();
            }
        }));
        ItemManager.ExecuteOnLoad(FirstPageFooter, () => Pages.Each(page =>
        {
            if (page.FirstPageFooterZombie != null)
            {
                page.FirstPageFooterZombie.RestoreVisualBrush();
            }
        }));
    }

private int LinesPerPage { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

private global::Quire.Calculation.Core.EngineError CurrentError { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

private EngineError CurrentErrorChain { get; set; }

public WorksheetViewModel(global::Quire.Controls.Worksheet.IWorksheetModel model, global::System.Windows.ResourceDictionary resourceDictionary, global::Quire.Controls.Core.ITextStylesManager textStylesManager) { }

public int DisplayMessageId(worksheetMessageIdType messageId)
    {
        // Button locals come first so Release local slots match Mathcad.
        string[] buttonsKey;
        string defaultButtonKey;
        string cancelButtonKey;
        CheckBoxData[] checkBoxesData = null;
        string empty = string.Empty;
        if (messageId == worksheetMessageIdType.ScriptableWarning)
        {
            empty = (string)_pasteHandler.WorksheetMessages["WorksheetMessages.ContainsScriptableObjects"];
            empty = empty.Replace("\\n", "\n");
            buttonsKey = new string[2] { "Yes", "No" };
            defaultButtonKey = "Yes";
            cancelButtonKey = "No";
        }
        else
        {
            return -1;
        }
        return VisualHelper.DisplayMessage(empty, MessageBoxImage.Exclamation, checkBoxesData, buttonsKey, defaultButtonKey, cancelButtonKey);
    }

private string GetMessageResource(worksheetMessageIdType id)
    {
        string key = string.Empty;
        if (id == worksheetMessageIdType.ScriptableWarning)
        {
            key = "WorksheetMessages.ContainsScriptableObjects";
        }
        return ((string)_pasteHandler.WorksheetMessages[key]).Replace("\\n", "\n");
    }

private void TextStyleChanged(ICollection<TextStyleData> arg)
    {
        foreach (TextStyleData item in arg)
        {
            if (item.StyleKey == "Normal")
            {
                TextStyle style = new TextStyle(item);
                SetStyleValues(ContentTextFormat, style);
                break;
            }
        }
    }

public void SetStyleValues(TextFormat textFormat, ITextStyle style)
    {
        textFormat.FontSize = (double)style.FontSize.Value;
        textFormat.FontFamily = style.FontFamily;
        textFormat.Foreground = style.Foreground;
        textFormat.Background = style.Background;
        textFormat.FontStyle = style.FontStyle.Value;
        textFormat.FontWeight = style.FontWeight.Value;
        textFormat.TextDecorations = style.TextDecorations;
    }

private void OnStyleCollectionChanged()
    {
        FireChanged();
    }

public global::Quire.Controls.Worksheet.IWorksheetVisualHelper VisualHelper { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

private void Initialize()
    {
        _model.ItemMoved += HandleItemMoved;
        _model.PropertyChanged += HandleModelPropertyChanged;
        WorksheetContent.InsertionPointSet += () => SetInsertionPointOnAppropriatePage(WorksheetContent.InsertionPoint);
        WorksheetContent.ActiveItemChanged += HandleActiveItemChanged;
        WorksheetContent.ActiveDescendantChanged += HandleActiveDescendantChanged;
        WorksheetContent.SelectedDescendantsChanged += HandleSelectedDescendantsChanged;
        WorksheetContent.Activated += () => ActiveSection = WorksheetContent;
        _pageManager.ContentActivated += () => ActiveSection = WorksheetContent;
        WorksheetContent.NonprintableSpaceWidthIncrease += newWidth =>
        {
            this.WidthWithNonprintableSpace = newWidth + 2.0 * _worksheetGrid.GridCellSize.Width;
        };
        HookupCollectionChangedHandlers();
        OleObjectAutoResize = true;
        SetupHeaderFooter();
        SetupDefaultPageLayout();
        SetupCommandBindings();
        SetupDisplayFormats();
        HookupDraggerHandlers();
    }

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

private void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

public event global::System.EventHandler Changed;

public IHeaderFooterControl Header { get; private set; }

public IHeaderFooterControl Footer { get; private set; }

public IHeaderFooterControl FirstPageHeader { get; private set; }

public IHeaderFooterControl FirstPageFooter { get; private set; }

public bool IsSavedWorksheet { private get; set; }

public IWorksheetContent WorksheetContent { get; private set; }

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

public bool IsFirstPageHeaderActive { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public bool IsFirstPageFooterActive { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

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

public global::Quire.Controls.Core.IRestrictedAreaAlignmentGrid WorksheetGrid { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public global::System.Windows.Markup.XmlLanguage CurrentProofingLanguage { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); private set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

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

public ObservableRangeCollection<IWorksheetPage> VisiblePages => _pageManager.VisiblePages;

public ObservableRangeCollection<IWorksheetPage> Pages => _pageManager.Pages;

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

public int TrailingEmptyPageCount => _pageManager.GetTrailingEmptyPageCount();

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

public event global::System.Action StartingPageNumberChanged;

private void FireStartingPageNumberChanged()
    {
        StartingPageNumberChanged?.Invoke();
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

public bool IsFirstPageHeaderFooterDifferent
    {
        get
        {
            return _isDifferentFirstPage;
        }
        set
        {
            if (_isDifferentFirstPage == value)
            {
                return;
            }
            _isDifferentFirstPage = value;
            OnPropertyChanged(nameof(IsFirstPageHeaderFooterDifferent));
            UpdateFirstPageHeaderFooter();
            UpdateActiveSectionForDifferentFirstPage();
            FireChanged();
        }
    }

private void UpdateFirstPageHeaderFooter()
    {
        if (Pages.IsEmpty())
        {
            return;
        }
        IWorksheetPage worksheetPage = Pages[0];
        if (IsFirstPageHeaderFooterDifferent)
        {
            createFirstPageZombiesIfNeeded(worksheetPage);
            if (ActivePage == worksheetPage)
            {
                worksheetPage.Header = FirstPageHeader;
                worksheetPage.Footer = FirstPageFooter;
            }
            else
            {
                ApplyFirstPageHeaderFooterToInactive(worksheetPage);
            }
        }
        else if (ActivePage == worksheetPage)
        {
            worksheetPage.Header = Header;
            worksheetPage.Footer = Footer;
        }
        else
        {
            ApplyRegularHeaderFooterToInactive(worksheetPage);
        }
    }

private void createFirstPageZombiesIfNeeded(IWorksheetPage page)
    {
        if (page.FirstPageHeaderZombie == null)
        {
            page.FirstPageHeaderZombie = ControlCreator.CreateHeaderFooterZombieControl(FirstPageHeader);
        }
        if (page.FirstPageFooterZombie == null)
        {
            page.FirstPageFooterZombie = ControlCreator.CreateHeaderFooterZombieControl(FirstPageFooter);
        }
    }

private void ApplyFirstPageHeaderFooterToInactive(IWorksheetPage page)
    {
        page.Header = page.FirstPageHeaderZombie;
        page.Footer = page.FirstPageFooterZombie;
        ShowIfPresent(page.FirstPageHeaderZombie);
        ShowIfPresent(page.FirstPageFooterZombie);
        CollapseIfPresent(page.HeaderZombie);
        CollapseIfPresent(page.FooterZombie);
    }

private void ApplyRegularHeaderFooterToInactive(IWorksheetPage page)
    {
        page.Header = page.HeaderZombie;
        page.Footer = page.FooterZombie;
        CollapseIfPresent(page.FirstPageHeaderZombie);
        CollapseIfPresent(page.FirstPageFooterZombie);
        ShowIfPresent(page.HeaderZombie);
        ShowIfPresent(page.FooterZombie);
    }

private void UpdateActiveSectionForDifferentFirstPage()
    {
        IWorksheetPage worksheetPage = Pages[0];
        if (ActivePage == worksheetPage)
        {
            if (IsHeaderActive)
            {
                ActivateHeader(InsertionPoint);
            }
            else if (IsFooterActive)
            {
                ActivateFooter(InsertionPoint);
            }
        }
    }

public void UpdateTotalPagesOffsetOnPageChange(int updatedPageNumberDifference)
    {
        if (TotalPagesOffset != 0)
        {
            TotalPagesOffset = Math.Max(0, TotalPagesOffset - updatedPageNumberDifference);
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

private void UpdateWorksheetHeight()
    {
        WorksheetHeight = WorksheetPageLayoutDisplayHeight * (double)Pages.Count;
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

private void UpdateHeaderFooterTops()
    {
        double num = (CurrentPageNumber - 1) * PageLayout.DisplayHeight;
        HeaderTop = num;
        FooterTop = num + PageLayout.PageMargin.Top + PageLayout.PageBodyHeight;
    }

public global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Core.IWorksheetPageBreak> PageBreaks { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems)
    {
        WorksheetContent.SetCopyWhiteboardItemsDelegate(copyWhiteboardItems);
    }

public void HandleTabOutOfText(CustomRoutedEventArgs<FocusNavigationDirection> eventArgs)
    {
        eventArgs.Handled = true;
        HandleTab(eventArgs.Parameter);
    }

public void TraverseWhiteboardItems(Func<IWhiteboardItem, bool, bool> function)
    {
        TraverseWhiteboardItems(_model.WhiteboardItems, function, isWorksheetContentItems: true);
        TraverseWhiteboardItems(Footer.WhiteboardItems, function, isWorksheetContentItems: false);
        TraverseWhiteboardItems(Header.WhiteboardItems, function, isWorksheetContentItems: false);
        TraverseWhiteboardItems(FirstPageFooter.WhiteboardItems, function, isWorksheetContentItems: false);
        TraverseWhiteboardItems(FirstPageHeader.WhiteboardItems, function, isWorksheetContentItems: false);
    }

public bool IsValidInsertionPoint(Point point)
    {
        return WorksheetContent.IsValidInsertionPoint(point);
    }

public void ApplyWorksheetData(IWorksheetPersistentData worksheetData)
    {
        PageLayout.WorksheetRecreateSuggested -= RecreateDocumentAfterPageSizeChange;
        HasScriptableControls = worksheetData.WorksheetContent.MsgId == worksheetMessageIdType.ScriptableWarning;
        try
        {
            if (worksheetData.LayoutSize != null && Enum.IsDefined(typeof(PageSize), worksheetData.LayoutSize))
            {
                PageLayout.SetPageSize((PageSize)Enum.Parse(typeof(PageSize), worksheetData.LayoutSize, ignoreCase: false), updateCustomMargins: false);
            }
            if (worksheetData.GridSize != null && Enum.IsDefined(typeof(GridSize), worksheetData.GridSize))
            {
                PageLayout.GridSize = (GridSize)Enum.Parse(typeof(GridSize), worksheetData.GridSize, ignoreCase: false);
            }
            if (worksheetData.PageOrientation != null && Enum.IsDefined(typeof(PageOrientation), worksheetData.PageOrientation))
            {
                PageLayout.SetPageOrientation((PageOrientation)Enum.Parse(typeof(PageOrientation), worksheetData.PageOrientation, ignoreCase: false), updateCustomMargins: false);
            }
            _ = worksheetData.PageBackgroundColor;
            PageLayout.PageBackgroundColor = worksheetData.PageBackgroundColor;
            Thickness? pageMargin = worksheetData.PageMargin;
            PageMarginType? pageMarginType = null;
            if (worksheetData.MarginType != null && Enum.IsDefined(typeof(PageMarginType), worksheetData.MarginType))
            {
                pageMarginType = (PageMarginType)Enum.Parse(typeof(PageMarginType), worksheetData.MarginType, ignoreCase: false);
            }
            if (pageMarginType.HasValue)
            {
                bool flag = pageMarginType.Value == PageMarginType.Custom;
                if (flag && pageMargin.HasValue)
                {
                    PageLayout.SetCustomMargin(pageMargin.Value);
                }
                else
                {
                    if (!flag)
                    {
                        PageLayout.SetMarginType(pageMarginType.Value);
                    }
                    DebugTools.DevAssert(pageMargin.HasValue == flag, null);
                }
            }
            DisplayGrid = worksheetData.DisplayGrid;
            DisplayHFGrid = worksheetData.DisplayHFGrid;
            DisplayFrame = worksheetData.DisplayFrame;
            DisplayHeaderFrame = worksheetData.DisplayHeaderFrame;
            DisplayFooterFrame = worksheetData.DisplayFooterFrame;
            UseMajorGridLines = worksheetData.UseMajorGridLines;
            UseHFMajorGridLines = worksheetData.UseHFMajorGridLines;
            OleObjectAutoResize = worksheetData.OleObjectAutoResize;
            IsSolveBlockLabelVisible = worksheetData.IsSolveBlockLabelVisible;
            StartingPageNumber = worksheetData.StartingPageNumber;
            TotalPagesOffset = worksheetData.TotalPagesOffset;
            IsFirstPageHeaderFooterDifferent = worksheetData.IsFirstPageHeaderFooterDifferent;
            CurrentProofingLanguage = worksheetData.ProofingLanguage ?? XmlLanguageHelper.FallbackXmlLanguage;
        }
        finally
        {
            PageLayout.WorksheetRecreateSuggested += RecreateDocumentAfterPageSizeChange;
        }
        foreach (IWorksheetPage page in _pageManager.Pages)
        {
            page.PageBody.GridCellSize = PageLayout.GridCellSize;
        }
    }

public void TraverseWhiteboardItems(IEnumerable<IWhiteboardItem> wbiCollection, Func<IWhiteboardItem, bool, bool> function, bool isWorksheetContentItems)
    {
        foreach (IWhiteboardItem item in wbiCollection)
        {
            if (function(item, isWorksheetContentItems) && item is IWhiteboardItemContainer)
            {
                IWhiteboardItemContainer whiteboardItemContainer = (IWhiteboardItemContainer)item;
                TraverseWhiteboardItems(whiteboardItemContainer.WhiteboardItems, function, isWorksheetContentItems);
            }
        }
    }

public global::Quire.Controls.Core.PageLayoutMode WorksheetPageLayoutMode { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public global::Quire.Controls.Core.WorksheetPageLayout PageLayout { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

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

public global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Core.IWhiteboardItem> WorksheetItems { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public global::Quire.Wpf.INotifyCollectionRangeChanged<global::Quire.Controls.Core.IWhiteboardItem> SelectedItems { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> ActiveOrSelectedItems { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public bool CanDeleteVerticalSpace(Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void DeleteActiveItem() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void CreateTextRegionFromEquationControl(IEquationControl sender)
    => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

public global::Quire.Core.TextFormat ContentTextFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

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

public NumberFormat DisplayNumberFormat { get; set; }

public global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont> LabeledIdFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public MatrixFormat ResultMatrixFormat { get; set; }

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

public global::System.Windows.Thickness NonprintableSpaceButtonPosition { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

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

public bool IsAutoCalcChecked => IsAutoCalcOn;

public bool IsCalculationsEnabled
    {
        get
        {
            return _model.IsCalculationsEnabled;
        }
        set
        {
            if (_model.IsCalculationsEnabled != value)
            {
                _model.IsCalculationsEnabled = value;
                OnPropertyChanged(nameof(IsCalculationsEnabled));
            }
        }
    }

public bool IsAutoCalcOn
    {
        get
        {
            return _model.IsAutoCalcOn;
        }
        set
        {
            if (_model.IsAutoCalcOn != value)
            {
                _model.IsAutoCalcOn = value;
                OnPropertyChanged(nameof(IsAutoCalcOn));
                OnPropertyChanged("IsAutoCalcChecked");
            }
        }
    }

public bool IsCalculating => _model.IsCalculating;

public global::Quire.Calculation.Core.ICalculatable GetLastVisibleCalculatable() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public ICalculatable GetCalculatableFromVisibleWhiteBoardItem(IWhiteboardItem item, IWorksheetControl wc)
    {
        if (item is ICalculatable result)
        {
            return result;
        }
        if (item is ICalculatableContainer calculatableContainer && !(calculatableContainer is ICalculatableAggregate))
        {
            if (calculatableContainer is IPlotControl || calculatableContainer is IPlotControl<XyzEquation>)
            {
                ICalculatable calculatable = calculatableContainer.CalculatableChildren.LastOrDefault();
                if (calculatable != null)
                {
                    return calculatable;
                }
            }
            if (calculatableContainer is ICollapsibleAreaControl collapsibleAreaControl)
            {
                if (collapsibleAreaControl.IsCollapsed)
                {
                    return calculatableContainer.CalculatableChildren.LastOrDefault();
                }
                for (int index = collapsibleAreaControl.WhiteboardItems.Count - 1; index >= 0; index--)
                {
                    if (VisualHelper.IsItemInView(collapsibleAreaControl.WhiteboardItems[index], wc))
                    {
                        ICalculatable calculatableFromVisibleWhiteBoardItem = GetCalculatableFromVisibleWhiteBoardItem(collapsibleAreaControl.WhiteboardItems[index], wc);
                        if (calculatableFromVisibleWhiteBoardItem != null && !IsCalculableHiddenFromCalculationContext(calculatableFromVisibleWhiteBoardItem))
                        {
                            return calculatableFromVisibleWhiteBoardItem;
                        }
                    }
                }
            }
        }
        if (item is IRichTextWithEmbeddedItem richTextWithEmbeddedItem && richTextWithEmbeddedItem.EmbeddedObjects.Any())
        {
            foreach (object embeddedItem in richTextWithEmbeddedItem.EmbeddedObjects.Reverse())
            {
                if (embeddedItem is ICalculatable embeddedCalculatable && VisualHelper.IsItemInView(item, wc))
                {
                    return embeddedCalculatable;
                }
            }
        }
        return null;
    }

public bool IsCalculableHiddenFromCalculationContext(ICalculatable calc)
    {
        if (calc is ICalculationHideableChild calculationHideableChild)
        {
            return calculationHideableChild.HideFromCalculationContext;
        }
        return false;
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

public event global::System.Action CurrentElementChanged;

public global::Quire.Controls.Worksheet.IWorksheetPageManager PageManager { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

public bool OleObjectAutoResize
    {
        get
        {
            return _oleObjectAutoResize;
        }
        set
        {
            if (_oleObjectAutoResize == value)
            {
                return;
            }
            _oleObjectAutoResize = value;
            OnPropertyChanged(nameof(OleObjectAutoResize));
            FireChanged();
        }
    }

public string DisplayFilePath
    {
        get
        {
            return _displayFilePath;
        }
        set
        {
            if (_displayFilePath == value)
            {
                return;
            }
            _displayFilePath = value;
            OnPropertyChanged(nameof(DisplayFilePath));
        }
    }

public bool IsSolveBlockLabelVisible
    {
        get
        {
            return _isSolveBlockLabelVisible;
        }
        set
        {
            if (_isSolveBlockLabelVisible == value)
            {
                return;
            }
            _isSolveBlockLabelVisible = value;
            OnPropertyChanged(nameof(IsSolveBlockLabelVisible));
            FireChanged();
        }
    }

public void AddItemAtLocation(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point location) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public IEquationControl CreateNewEquation(string sExpression)
    {
        IEquationControl equationControl = ControlCreator.CreateEquationControl(sExpression);
        equationControl.ChangeToTextBox += CreateTextRegionFromEquationControl;
        AddItemAtInsertionPointAndActivate(equationControl, "Add EquationControl");
        return equationControl;
    }

private IEquationControl CreateNewEquation(ICommand command, object parameter)
    {
        IEquationControl equationControl = ControlCreator.CreateEquationControl();
        if (command is EquationInsertCommand { Name: "InsertValue" })
        {
            equationControl.ChangeToTextBox += CreateTextRegionFromEquationControl;
        }
        if (!VisualHelper.ExecuteCommandOnItem(equationControl, command, parameter))
        {
            return null;
        }
        AddItemAtInsertionPointAndActivate(equationControl, "Add EquationControl and execute command");
        if (equationControl.IsCommitted)
        {
            equationControl.Commit();
        }
        return equationControl;
    }

private void CreateNewEquationWithInputControl(ICommand command, InputControlTypes? parameter)
    {
        CreateNewEquation(command, parameter).InputControlOnEquationActivated(parameter);
    }

public IImageControl CreateImageControl()
    {
        IImageControl imageControl = ControlCreator.CreateImageControl();
        if (imageControl != null)
        {
            AddItemAtInsertionPointAndActivate(imageControl, "Add Image");
        }
        return imageControl;
    }

public IImageControl CreateImageControl(string fileName)
    {
        IImageControl imageControl = new ImageControl();
        imageControl.OriginalImageUri = new Uri(fileName);
        if (imageControl.OriginalImageUri != null)
        {
            AddItemAtInsertionPointAndActivate(imageControl, "Add Image");
            return imageControl;
        }
        return null;
    }

private TextFormat GetCurrentTextFormat()
    {
        TextFormat result = ContentTextFormat;
        if (ActiveSection == FirstPageHeader)
        {
            result = FirstPageHeaderTextFormat;
        }
        else if (ActiveSection == FirstPageFooter)
        {
            result = FirstPageFooterTextFormat;
        }
        else if (IsHeaderActive)
        {
            result = HeaderTextFormat;
        }
        else if (IsFooterActive)
        {
            result = FooterTextFormat;
        }
        return result;
    }

public TextFormat GetCurrentTextFormatClone(TextFormat inputTextFormat)
    {
        TextFormat textFormat = inputTextFormat.Clone();
        if (textFormat.StyleKey == "areaLayoutStyle" || textFormat.StyleKey == "headerLayoutStyle" || textFormat.StyleKey == "footerLayoutStyle")
        {
            textFormat.StyleKey = "Normal";
        }
        return textFormat;
    }

public IRichTextBase CreateTextboxControl(string text = default)
    {
        IRichTextBase textbox = ControlCreator.CreateTextboxControl(GetCurrentTextFormatClone(GetCurrentTextFormat()), CurrentProofingLanguage, text, DataFormats.Text, false, _textStylesManager.TextStyleChangedHandler, false);
        AddItemAtInsertionPointAndActivate(textbox, "Add Textbox");
        return textbox;
    }

public IFileNameControl CreateFileNameControl()
    {
        IFileNameControl fileNameControl = ControlCreator.CreateFileNameControl(GetCurrentTextFormat());
        AddItemAtInsertionPointAndActivate(fileNameControl, "Add FileName Region");
        return fileNameControl;
    }

public IFilePathControl CreateFilePathControl()
    {
        IFilePathControl filePathControl = ControlCreator.CreateFilePathControl(GetCurrentTextFormat());
        AddItemAtInsertionPointAndActivate(filePathControl, "Add FilePath Region");
        return filePathControl;
    }

public IPageNumberControl CreatePageNumberControl(string template)
    {
        IPageNumberControl pageNumberControl = ActiveItem as IPageNumberControl;
        if (pageNumberControl == null)
        {
            IEnumerable<IPageNumberControl> source = SelectedItems.OfType<IPageNumberControl>();
            if (source.Count() > 0)
            {
                pageNumberControl = source.First();
            }
        }
        if (pageNumberControl != null)
        {
            pageNumberControl.TemplateFormat = template;
            return pageNumberControl;
        }
        pageNumberControl = ControlCreator.CreatePageNumberControl(template, GetCurrentTextFormat());
        AddItemAtInsertionPointAndActivate(pageNumberControl, "Add PageNumber Region");
        return pageNumberControl;
    }

public ISavedDateControl CreateSavedDateControl(string template)
    {
        ISavedDateControl savedDateControl = ActiveItem as ISavedDateControl;
        if (savedDateControl == null)
        {
            IEnumerable<ISavedDateControl> source = SelectedItems.OfType<ISavedDateControl>();
            if (source.Count() > 0)
            {
                savedDateControl = source.First();
            }
        }
        if (savedDateControl != null)
        {
            savedDateControl.TemplateFormat = template;
            return savedDateControl;
        }
        savedDateControl = ControlCreator.CreateSavedDateControl(template, GetCurrentTextFormat());
        AddItemAtInsertionPointAndActivate(savedDateControl, "Add Saved Date Region");
        return savedDateControl;
    }

private void RemoveRegionOfType<T>()
    {
        ActiveSection.WhiteboardItems.OfType<T>().ToList().Each(item =>
        {
            ActiveSection.RemoveItem(item as IWhiteboardItem);
        });
    }

private bool _headerFooterZombiesReady;

public IPlotControl CreateXYPlotControl(string xSExpression, string ySExpression)
    {
        IPlotControl plotControl = ControlCreator.CreateXYPlotControl(GetCurrentTextFormatClone(GetCurrentTextFormat()));
        if (plotControl is PlotControlBase plotControlBase)
        {
            plotControlBase.TextStyleChangedHandler = _textStylesManager.TextStyleChangedHandler;
        }
        using (new CommandScope(new CompoundCommand()))
        {
            InitializePlotControl(plotControl, xSExpression, ySExpression);
        }
        AddItemAtInsertionPointAndActivate(plotControl, "Add XYPlot");
        return plotControl;
    }

public IPlotControl CreateXYPlotControl()
    {
        return CreateXYPlotControl(string.Empty, string.Empty);
    }

public IPlotControl CreatePolarPlotControl(string xSExpression, string ySExpression)
    {
        IPlotControl plotControl = ControlCreator.CreatePolarPlotControl(GetCurrentTextFormatClone(GetCurrentTextFormat()));
        if (plotControl is PlotControlBase plotControlBase)
        {
            plotControlBase.TextStyleChangedHandler = _textStylesManager.TextStyleChangedHandler;
        }
        using (new CommandScope(new CompoundCommand()))
        {
            InitializePlotControl(plotControl, xSExpression, ySExpression);
        }
        AddItemAtInsertionPointAndActivate(plotControl, "Add PolarPlot");
        return plotControl;
    }

public IPlotControl CreatePolarPlotControl()
    {
        return CreatePolarPlotControl(string.Empty, string.Empty);
    }

public IPlotControl<XyzEquation> CreateContourPlotControl()
    {
        return CreateContourPlotControl(null, null, null);
    }

public IPlotControl<XyzEquation> Create3DPlotControl()
    {
        return Create3DPlotControl(null, null, null);
    }

public IPlotControl<XyzEquation> CreateContourPlotControl(string xSExpression, string ySExpression, string zSExpression)
    {
        IPlotControl<XyzEquation> plotControl = ControlCreator.CreateContourPlotControl();
        InitializeXYZPlotControl(plotControl, xSExpression, ySExpression, zSExpression);
        AddItemAtInsertionPointAndActivate(plotControl, "Add ContourPlot.");
        return plotControl;
    }

public IPlotControl<XyzEquation> Create3DPlotControl(string xSExpression, string ySExpression, string zSExpression)
    {
        IPlotControl<XyzEquation> plotControl;
        using (new CommandScope(new CompoundCommand()))
        {
            plotControl = ControlCreator.Create3DPlotControl();
            InitializeXYZPlotControl(plotControl, xSExpression, ySExpression, zSExpression);
        }
        AddItemAtInsertionPointAndActivate(plotControl, "Add 3D plot.");
        return plotControl;
    }

public ISpecTableControl CreateSpecTableControl(object dimensions)
    {
        return CreateSpecTableControl((dimensions as TableCoordinate?) ?? (TypeDescriptor.GetConverter(typeof(TableCoordinate)).ConvertFrom(dimensions) as TableCoordinate?));
    }

public ISpecTableControl CreateSpecTableControl(TableCoordinate? dimensions)
    {
        ISpecTableControl specTableControl;
        using (new CommandScope("CreateSpecTableControl"))
        {
            specTableControl = ControlCreator.CreateSpecTableControl(dimensions);
            AddItemAtInsertionPointAndActivate(specTableControl, "Add SpecTable");
        }
        return specTableControl;
    }

public IAggregateMathboardControl CreateSolveBlockControl()
    {
        IAggregateMathboardControl aggregateMathboardControl = ControlCreator.CreateSolveBlockControl(_worksheetGrid.GridCellSize);
        AddItemAtInsertionPointAndActivate(aggregateMathboardControl, "Add SolveBlock");
        return aggregateMathboardControl;
    }

public IAggregateMathboardControl CreateExcelComponentControl()
    {
        try
        {
            IAggregateMathboardControl control = ControlCreator.CreateExcelComponentControl(_worksheetGrid.GridCellSize);
            AddItemAtInsertionPointAndActivate(control, "Add ExcelComponent");
            control.Commit();
            return control;
        }
        catch (ApplicationException ex)
        {
            VisualHelper.DisplayWarningMessage(ex.Message);
            return null;
        }
    }

public IOleObjectControl CreateOleObjectControl(OleObjectControlCreationMethod creationMethod)
    {
        if (creationMethod == null)
        {
            creationMethod = new OleObjectControl.CreationMethodWithDialog(Application.Current.MainWindow);
        }

        try
        {
            IOleObjectControl oleObjectControl = ControlCreator.CreateOleObjectControl(new OleObjectControlCreationOptions(creationMethod, OleObjectAutoResize), DisplayFilePath);
            if (oleObjectControl != null)
            {
                AddItemAtInsertionPointAndActivate(oleObjectControl, "Insert Object");
            }

            return oleObjectControl;
        }
        catch (ApplicationException ex)
        {
            VisualHelper.DisplayWarningMessage(ex.Message);
            return null;
        }
    }

public IAggregateMathboardControl CreateChartComponentControl()
    {
        try
        {
            IAggregateMathboardControl control = ControlCreator.CreateChartComponentControl(_worksheetGrid.GridCellSize);
            AddItemAtInsertionPointAndActivate(control, "Add ChartComponent");
            control.Commit();
            return control;
        }
        catch (ApplicationException ex)
        {
            VisualHelper.DisplayWarningMessage(ex.Message);
            return null;
        }
    }

private ICollapsibleAreaControl CreateCollapsibleAreaControl()
    {
        ICollapsibleAreaControl collapsibleAreaControl = ControlCreator.CreateCollapsibleAreaControl(_worksheetGrid.GridCellSize);
        double majorGridLines = PageLayout.MajorGridLines;
        ItemManager.SetHeight(collapsibleAreaControl, majorGridLines);
        AddItemAtInsertionPointAndActivate(collapsibleAreaControl, "Add CollapsibleArea");
        return collapsibleAreaControl;
    }

private bool CanCreateExcelComponent()
    {
        if (CanCreateControlOfType<IExcelComponentControl>())
        {
            return OleUtils.CanCreateExcelContainer();
        }
        return false;
    }

private bool CanCreateOleObjectControl()
    {
        return CanCreateControlOfType<IOleObjectControl>();
    }

private bool CanCreateChartComponent()
    {
        return CanCreateControlOfType<IChartComponentControl>();
    }

public IParagraphControl CreateTextBlockControl(string text = default)
    {
        IParagraphControl textBlock = ControlCreator.CreateTextBlockControl(GetCurrentTextFormatClone(ContentTextFormat), CurrentProofingLanguage, text, DataFormats.Text, _textStylesManager.TextStyleChangedHandler);
        AddItemAtInsertionPointAndActivate(textBlock, "Add TextBlock");
        return textBlock;
    }

public IIncludeWorksheetControl CreateIncludeControl(string filePath)
    {
        IIncludeWorksheetControl includeControl;
        using (new CommandScope("CreateIncludeControl"))
        {
            includeControl = ControlCreator.CreateIncludeControl();
            AddItemAtInsertionPointAndActivate(includeControl, "Add IncludeControl");
            includeControl.IncludedFilePath = filePath;
        }

        return includeControl;
    }

public IIncludeWorksheetControl CreateIncludeControl()
    {
        return CreateIncludeControl("");
    }

public ISpecTableControl CreateSpecTableControl()
    {
        return CreateSpecTableControl(null);
    }

public IWorksheetPageBreak CreatePageBreakControl()
    {
        return ControlCreator.CreatePageBreakControl();
    }

private bool CanMoveInDirection(Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void DirectionalMoveHandler(global::Quire.Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void DirectionalMoveHandler(global::Quire.Direction direction, global::System.Windows.Vector offset) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public bool HandleTextInput(string text)
    {
        if (WpfUtils.ShouldSuspendInputMethod())
        {
            _textCache += text;
            ItemManager.BlockUntilInput();
            if (string.IsNullOrEmpty(_textCache))
            {
                return true;
            }
            text = _textCache;
            _textCache = string.Empty;
        }
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }
        if (!CanCreateControlOfType<IEquationControl>())
        {
            return false;
        }
        IEquationControl equationControl;
        using (new CommandScope("Create Equation Control Via Keypress"))
        {
            equationControl = ControlCreator.CreateEquationControlWithTextInput(text);
            if (equationControl != null)
            {
                equationControl = CreateNewEquation(null);
                ItemManager.BlockUntilLoaded();
                equationControl.Keypress(text[0].ToString());
            }
        }
        if (equationControl != null && text.Length > 1)
        {
            equationControl.Keypress(text.Substring(1));
        }
        return equationControl != null;
    }

public bool HandleKeyDown(System.Windows.Input.KeyEventArgs args)
    {
        if (!CanCreateControlOfType<IEquationControl>())
        {
            return false;
        }
        if (ActiveDescendant is IRichTextWithEmbeddedItem)
        {
            return false;
        }
        IEquationControl equationControl = null;
        SymbolDescriptor symbolDescriptor = SearchSymbolDescriptorsForKeygesture(args, VisualOperatorDescriptorTable.Default.Values.OfType<SymbolDescriptor>()) ?? SearchSymbolDescriptorsForKeygesture(args, VisualConstantDescriptorTable.Default.Values.OfType<SymbolDescriptor>());
        if (symbolDescriptor != null && FunctionalityServiceWrapper.Instance.IsFunctionalityEnabled(symbolDescriptor.FunctionalityKey))
        {
            if (symbolDescriptor is VisualOperatorDescriptor visualOperatorDescriptor && !CanInsertOperator(visualOperatorDescriptor.CanonicalName))
            {
                return false;
            }
            equationControl = CreateNewEquation(symbolDescriptor.Command, symbolDescriptor.CommandParameter);
        }
        return equationControl != null;
    }

private static global::Quire.Controls.SymbolDescriptor SearchSymbolDescriptorsForKeygesture(global::System.Windows.Input.KeyEventArgs e, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.SymbolDescriptor> symbols) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void HandleDelete() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void HandleBackspace() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void HandleEnter() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void HandleTab(FocusNavigationDirection direction)
    {
        IWhiteboardItem whiteboardItem = FindNextItemForTabNavigation(direction);
        if (whiteboardItem == null)
        {
            return;
        }
        ItemManager.TabInto(whiteboardItem, direction);
    }

public void HandleControlEnter()
    {
        if (ActiveOrSelectedItems.Count() == 0)
        {
            InsertPageBreak();
        }
        else if (ActiveItem != null)
        {
            ToggleItemSelection(ActiveItem, includeActiveItem: true);
        }
        else
        {
            ToggleItemSelection(SelectedItems.Last(), includeActiveItem: true);
        }
    }

public void HandleSelectAll()
    {
        ActiveSection.SelectAllItems();
    }

public void HandleGoToPage(object pageNumber)
    {
        int pageNum = 1;
        if (int.TryParse(pageNumber.ToString(), out pageNum) && pageNum > 0 && pageNum <= PageManager.Pages.Count)
        {
            PageManager.GoToPage(PageManager.Pages[pageNum - 1]);
        }
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

public void SetWorksheetSectionItemLocation(IWhiteboardItem item, Point worksheetLocation)
    {
        ActiveSection.SetItemLocation(item, worksheetLocation);
    }

public Point GetWorksheetSectionItemLocation(IWhiteboardItem item)
    {
        return ActiveSection.GetItemLocation(item);
    }

public DragMode CurrentDragMode { get; set; }

public global::Quire.Controls.Worksheet.IWorksheetSection ActiveSection { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); private set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

protected override void OnPropertyChanged(string propertyName) => base.OnPropertyChanged(propertyName);

public int CurrentPageNumber
    {
        get
        {
            return _currentPageNumber;
        }
        private set
        {
            if (_currentPageNumber == value)
            {
                return;
            }
            _currentPageNumber = value;
            UpdateHeaderFooterTops();
            OnPropertyChanged(nameof(CurrentPageNumber));
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

public bool HandlePreviewMouseDown(object originalSource, Point position, bool mouseDoubleClicked, bool rightButton)
    {
        bool handled;
        _worksheetMouseDownHandler.HandlePreviewMouseDown(originalSource, position, mouseDoubleClicked, rightButton, out handled);
        return handled;
    }

public bool HandleMouseDown(object originalSource, bool alreadyHandled)
    {
        return _worksheetMouseDownHandler.HandleMouseDown(originalSource, alreadyHandled);
    }

public void HandleBlockedPreviewMouseButton(object originalSource)
    {
        _worksheetMouseDownHandler.HandleBlockedPreviewMouseButton(originalSource);
    }

public bool HandlePreviewMouseUp(object originalSource)
    {
        return _worksheetMouseDownHandler.HandlePreviewMouseUp(originalSource);
    }

public bool HandleMouseUp(object originalSource, Point position, bool rightButton)
    {
        return _worksheetMouseDownHandler.HandleMouseUp(originalSource, rightButton);
    }

public void OnRequestMoveAndResize(global::Quire.Controls.Worksheet.IWorksheetControl worksheetControl, global::Quire.Controls.Core.ResizeEventMouseArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void HandleCaretLeavesTextArea(IWorksheetControl worksheetControl, CommonRoutedEvents.CaretLeavesTextRegionEventArgs args)
    {
        Direction direction = Direction.Down;
        switch (args.LeavingDirection)
        {
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Down:
                direction = Direction.Down;
                break;
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Up:
                direction = Direction.Up;
                break;
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Left:
                direction = Direction.Left;
                break;
            case CommonRoutedEvents.CaretLeavesTextRegionEventArgs.TextAreaLeavingDirection.Right:
                direction = Direction.Right;
                break;
        }
        DirectionalMoveHandler(direction);
    }

public bool GetItemIgnoreForHorizontalMovementAndWidth(IWhiteboardItem item)
    {
        return ItemManager.GetIgnoreForHorizontalMovementAndWidth(item);
    }

public void ActivateHeader()
    {
        WorksheetPageLayoutMode = PageLayoutMode.PageMode;
        IHeaderFooterControl headerControl = IsFirstPageHeaderFooterDifferent && ActivePage?.PageNumber == 1 ? FirstPageHeader : Header;
        ActivateSection(headerControl, ActivePage, null);
        headerControl.RestoreVisualState();
    }

public void ActivateFooter()
    {
        WorksheetPageLayoutMode = PageLayoutMode.PageMode;
        IHeaderFooterControl footerControl = IsFirstPageHeaderFooterDifferent && ActivePage?.PageNumber == 1 ? FirstPageFooter : Footer;
        ActivateSection(footerControl, ActivePage, null);
        footerControl.RestoreVisualState();
    }

public void ActivateContent()
    {
        MakeContentActive();
        WorksheetContent.RestoreVisualState();
    }

public void ActivateHeader(Point pt)
    {
        IHeaderFooterControl headerControl = IsFirstPageHeaderFooterDifferent && ActivePage?.PageNumber == 1 ? FirstPageHeader : Header;
        ActivateSection(headerControl, ActivePage, pt);
        headerControl.RestoreVisualState();
    }

public void ActivateFooter(Point pt)
    {
        IHeaderFooterControl footerControl = IsFirstPageHeaderFooterDifferent && ActivePage?.PageNumber == 1 ? FirstPageFooter : Footer;
        ActivateSection(footerControl, ActivePage, pt);
        footerControl.RestoreVisualState();
    }

public void ActivateContent(Point insertionPoint)
    {
        MakeContentActive();
        WorksheetContent.InsertionPoint = insertionPoint;
        SetInsertionPointOnAppropriatePage(WorksheetContent.InsertionPoint);
    }

public void InsertPageBreak()
    {
        InsertPageBreak(InsertionPoint);
    }

public void ExpandToIncludePoint(global::System.Windows.Point pointToInclude)
    {
        global::System.Windows.Point point = VisualHelper.TranslateWorksheetItemsCanvasPointToWorksheet(pointToInclude, this, global::Quire.Direction.Down);
        global::System.Windows.Rect gridArea = _worksheetGrid.GridArea;
        if (gridArea.Contains(point))
        {
            return;
        }
        if (point.Y > 0.0)
        {
            int pageIndex = GetPageIndex(point.Y);
            Pages.Count.UpTo(pageIndex, (int num) =>
            {
                _pageManager.AppendNewPage();
            });
        }
        WorksheetContent.SetWidthWithNonprintableSpace(point.X);
        ItemManager.BlockUntilLoaded();
    }

public void PopulateZombies()
    {
        foreach (IWorksheetPage page in Pages)
        {
            DebugTools.AssertNotNull(page.HeaderZombie, null);
            DebugTools.AssertNotNull(page.FooterZombie, null);
        }
    }

public void ScrollToActiveArea()
    {
        if (ActiveSection.IsCrosshairVisible)
        {
            IWorksheetPageSection pageSection;
            IWhiteboardControl section;
            if (ActiveSection is IWorksheetPageSection)
            {
                pageSection = ActiveSection as IWorksheetPageSection;
                section = ActiveSection;
            }
            else
            {
                DebugTools.DevAssert(IsContentActive);
                pageSection = ActivePage.PageBody;
                section = ActivePage.PageBody;
            }

            VisualHelper.BringIntoView(pageSection, section.GetInsertionPointRect());
        }
        else
        {
            VisualHelper.BringIntoView(ActiveOrSelectedItems.First());
        }
    }

public void MoveItem(IWhiteboardItem item, Point point)
    {
        Point worksheetSectionItemLocation = GetWorksheetSectionItemLocation(item);
        Vector offset = point - worksheetSectionItemLocation;
        MoveItemsBy(new[] { item }, offset);
    }

public void MoveItemsBy(IEnumerable<IWhiteboardItem> items, Vector offset)
    {
        if (IsContentActive)
        {
            // Release IL calls Execute through a local (callvirt); a new(...).Execute() call compiles to call.
            MoveItemsCommand command = new MoveItemsCommand(this, offset, items, bringItemsIntoView: true);
            command.Execute();
        }
        else
        {
            ActiveSection.MoveItemsBy(items, offset);
        }
    }

public bool CanInsertSpace()
    {
        if (!IsContentActive)
        {
            return false;
        }
        return ActiveOrSelectedItems.Count() == 0;
    }

public bool CanGotopage(object pageNumber)
    {
        int pageNum = 1;
        if (int.TryParse(pageNumber.ToString(), out pageNum) && pageNum > 0 && pageNum <= PageManager.Pages.Count)
        {
            return true;
        }

        return false;
    }

public void InsertSpace(int? numberOfLines)
    {
        int numberOfLines2 = numberOfLines ?? 1;
        // Release IL calls Execute through a local (callvirt); a new(...).Execute() call compiles to call.
        InsertSpaceCommand command = new InsertSpaceCommand(this, numberOfLines2, pushInsertionPoint: false);
        command.Execute();
    }

public bool CanRemoveSpace(Direction direction)
    {
        return CanDeleteVerticalSpace(direction);
    }

public void RemoveSpace(int? numberOfLines, Direction direction)
    {
        int numberOfLines2 = numberOfLines ?? WhiteboardControl.DeleteEmptySpaceDefaultLinesCount;
        // Release IL calls Execute through a local (callvirt); a new(...).Execute() call compiles to call.
        RemoveSpaceCommand command = new RemoveSpaceCommand(this, numberOfLines2, direction);
        command.Execute();
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

public void ToggleIsSolveBlockLabelVisible() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool CanToggleIsSolveBlockLabelVisible()
    {
        return !SelectedDescendants.Any();
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

public void ToggleHideFromCalculationContext() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

public bool CanReCalculateAll()
    {
        return IsCalculationsEnabled;
    }

public void FireRecalculateAll()
    {
        PreRecalculateAll = true;
        CommitActive(ActiveItem);
        WpfUtils.DoWork(() => _model.FireRecalculateAll());
        PreRecalculateAll = false;
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

public void HandleToggleAutoCalculation(bool newState)
    {
        IsAutoCalcOn = newState;
        FireChanged();
    }

private static void CommitActive(IWhiteboardItem activeItem)
    {
        if (activeItem == null)
        {
            return;
        }

        if (activeItem is ICalculatable)
        {
            // The IL re-tests the type for the call; a pattern variable would add a local.
            (activeItem as ICalculatable).Commit();
            return;
        }

        if (activeItem is IWhiteboardControl)
        {
            CommitActive((activeItem as IWhiteboardControl).ActiveDescendant);
            return;
        }

        if (activeItem is ICalculatableContainer)
        {
            foreach (ICalculatable calculatable in (activeItem as ICalculatableContainer).CalculatableChildren)
            {
                calculatable.Commit();
            }
        }
    }

protected void FireChanged()
    {
        Changed?.Invoke(this, EventArgs.Empty);
    }

public void GoToLastError()
    {
        if (ErrorCanTraceForward() && CurrentErrorChain != null)
        {
            _model.ShowError(CurrentErrorChain);
        }
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

public void HandleRemoveAnnotations() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

public int GetItemWorksheetPageIndex(IWhiteboardItem item)
    {
        return _pageManager.GetPageIndex(item);
    }

public bool FindOnce(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

public void PageDown()
    {
        MoveByPage(Direction.Down);
    }

public void PageUp()
    {
        MoveByPage(Direction.Up);
    }

public void ToggleInputRegions()
    {
        _model.InputOutputRegionManager.ToggleInputRegions(GetInputRegions());
    }

public void ToggleOutputRegions()
    {
        _model.InputOutputRegionManager.ToggleOutputRegions(GetOutputRegions());
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

protected override void OnDispose() => base.OnDispose();

private bool CanToggleHideFromCalculationContext()
    {
        return HideFromCalculationButtonEnabled;
    }

private bool CanDeleteSelectedItems()
    {
        return !SelectedItems.IsNullOrEmpty();
    }

private bool DeleteSelectedItems()
    {
        IList<IWhiteboardItem> items = new List<IWhiteboardItem>(SelectedItems);
        bool deleted = ActiveSection.HandleDelete();
        if (deleted)
        {
            HandlePageWideSizableDeletion(items);
        }
        return deleted;
    }

private bool CanDeleteActiveOrSelectedItems()
    {
        return !ActiveOrSelectedItems.IsNullOrEmpty();
    }

private bool CanTogglePageLayout(object param)
    => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void DeleteActiveOrSelectedItems()
    {
        if (CanDeleteActiveItem())
        {
            DeleteActiveItem();
        }
        else if (CanDeleteSelectedItems())
        {
            DeleteSelectedItems();
        }
    }

private bool CanCreateInputControlOfType<T>(InputControlTypes? inputControlTypes)
    {
        if ((inputControlTypes != InputControlTypes.ComboBoxControl && inputControlTypes != InputControlTypes.ListBoxScriptableControl && inputControlTypes != InputControlTypes.CheckBoxScriptableControl && inputControlTypes != InputControlTypes.ButtonScriptableControl && inputControlTypes != InputControlTypes.RadioButtonScriptableControl && inputControlTypes != InputControlTypes.SliderScriptableControl && inputControlTypes != InputControlTypes.TextBoxScriptableControl) || (inputControlTypes != InputControlTypes.ComboBoxControl && GetAddableContainerControl(isExcludeWorksheetControlContent: true) != null) || (inputControlTypes != InputControlTypes.ComboBoxControl && ActiveSection is HeaderFooterControl))
        {
            return false;
        }
        return CanCreateControlOfType<T>();
    }

private bool CanCreateControlOfType<T>(global::Quire.Controls.Core.Whiteboard.IWhiteboardItemContainer containerControl = default) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool ForbidsCreatingControls(IWhiteboardItemContainer containerControl)
    {
        if (!(containerControl is IWhiteboardControl whiteboardControl))
        {
            return false;
        }
        return !whiteboardControl.CanAddItems;
    }

private IWhiteboardItemContainer GetAddableContainerControl(bool isExcludeWorksheetControlContent = false)
    {
        if (ActiveItem == null && SelectedItems.Count() == 0)
        {
            if (IsContentActive && OverlapsVerticallyWithRegionOfType<IParagraphControl>(InsertionPoint))
            {
                return null;
            }
            if (isExcludeWorksheetControlContent && ActiveSection is WorksheetControlContent)
            {
                return null;
            }
            return ActiveSection;
        }
        if (!(ActiveDescendant is IWhiteboardItemContainer { CanAddItems: not false } whiteboardItemContainer))
        {
            return null;
        }
        if (isExcludeWorksheetControlContent && whiteboardItemContainer is CollapsibleAreaControl)
        {
            return null;
        }
        return whiteboardItemContainer;
    }

private static bool CanCreateControlOfType(global::System.Type containerType, global::System.Type controlType) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private static bool CanCreateInputControl(Type containerType, IWhiteboardItem control)
    {
        EquationControl equationControl = control as EquationControl;
        if (equationControl != null && equationControl.IsScriptableControl && containerType != typeof(WorksheetControlContent) && containerType != typeof(CollapsibleAreaControl))
        {
            return false;
        }

        return true;
    }

private bool CanCreateFileSpecificRegion<T>()
    {
        if (!CanCreateControlOfType<T>())
        {
            return false;
        }
        if (ActiveSection.WhiteboardItems.Any((IWhiteboardItem item) => item is T))
        {
            return false;
        }
        return true;
    }

private bool CanCreateSavedDateControl()
    {
        if (ActiveItem is ISavedDateControl)
        {
            return true;
        }
        if (SelectedItems.Any((IWhiteboardItem item) => item is ISavedDateControl))
        {
            return true;
        }
        return CanCreateFileSpecificRegion<ISavedDateControl>();
    }

private bool CanCreatePageNumberControl()
    {
        if (ActiveItem is IPageNumberControl)
        {
            return true;
        }
        if (SelectedItems.Any((IWhiteboardItem item) => item is IPageNumberControl))
        {
            return true;
        }
        if (!CanCreateControlOfType<IPageNumberControl>())
        {
            return false;
        }
        return !ActiveSection.WhiteboardItems.Any((IWhiteboardItem item) => item is IPageNumberControl);
    }

private bool CanRemoveRegionOfType<T>()
    {
        return ActiveSection.WhiteboardItems.Any((IWhiteboardItem item) => item is T);
    }

private bool CanCreateParagraphControl()
    {
        if (!CanCreateControlOfType<IParagraphControl>())
        {
            return false;
        }
        IWhiteboardControl whiteboardControl = ActiveItem as IWhiteboardControl;
        Point insertionPoint;
        if (whiteboardControl != null)
        {
            insertionPoint = whiteboardControl.InsertionPoint;
            return insertionPoint.X <= PageLayout.PageBodyWidth;
        }
        insertionPoint = InsertionPoint;
        return insertionPoint.X <= PageLayout.PageBodyWidth;
    }

private void InitializePlotControl(IPlotControl control, string xSExpression, string ySExpression)
    {
        control.AddEquations(new XyEquation(xSExpression, ySExpression));
        control.Width = PageLayout.PageBodyWidth / 2.0;
        control.Height = control.Width;
    }

private void InitializeXYZPlotControl(IPlotControl<XyzEquation> control, string xSExpression, string ySExpression, string zSExpression)
    {
        if (zSExpression != null)
        {
            control.AddEquations(new XyzEquation(xSExpression, ySExpression, zSExpression));
        }
    }

private IPlotControl CreateXYPlotControl(XyEquation xyEquation)
    {
        if (xyEquation != null)
        {
            return CreateXYPlotControl(xyEquation.XsExpression, xyEquation.YsExpression);
        }
        return CreateXYPlotControl();
    }

private IPlotControl CreatePolarPlotControl(XyEquation xyEquation)
    {
        if (xyEquation != null)
        {
            return CreatePolarPlotControl(xyEquation.XsExpression, xyEquation.YsExpression);
        }
        return CreatePolarPlotControl();
    }

private IPlotControl<XyzEquation> CreateContourPlotControl(XyzEquation xyzEquation)
    {
        if (xyzEquation != null)
        {
            return CreateContourPlotControl(xyzEquation.XsExpression, xyzEquation.YsExpression, xyzEquation.ZsExpression);
        }
        return CreateContourPlotControl();
    }

private IPlotControl<XyzEquation> Create3DPlotControl(XyzEquation xyzEquation)
    {
        if (xyzEquation != null)
        {
            return Create3DPlotControl(xyzEquation.XsExpression, xyzEquation.YsExpression, xyzEquation.ZsExpression);
        }
        return Create3DPlotControl();
    }

private bool OverlapsVerticallyWithRegionOfType<T>(global::System.Windows.Point point) where T : global::Quire.Controls.Core.IWhiteboardItem => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private Rect GetItemRect(IWhiteboardItem item)
    {
        return ItemManager.GetItemRect(item);
    }

private bool ItemsOverlapped(IEnumerable<IWhiteboardItem> items)
    {
        return ItemManager.ItemsOverlapped(items);
    }

private bool CanCopySelection()
    {
        return SelectedItems.Count() > 0;
    }

private void CopySelection()
    {
        if (!CanCopySelection())
        {
            return;
        }
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ActiveSection.CopySelection();
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

private bool CanCutSelection()
    {
        return SelectedItems.Count() > 0;
    }

private void CutSelection()
    {
        if (!CanCutSelection())
        {
            return;
        }
        try
        {
            IList<IWhiteboardItem> items = new List<IWhiteboardItem>(SelectedItems);
            Mouse.OverrideCursor = Cursors.Wait;
            ActiveSection.CutSelection();
            HandlePageWideSizableDeletion(items);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

private bool CanPaste() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void Paste() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool CanDeleteActiveItem()
    {
        if (IsContentActive)
        {
            return ActiveItem != null;
        }
        return false;
    }

private static void ShowWaitCursor(Action action)
    {
        Cursor overrideCursor = MouseFacadeService.ShowWaitCursor();
        try
        {
            action();
        }
        finally
        {
            MouseFacadeService.SetOverrideCursor(overrideCursor);
        }
    }

private void EvaluateItem(IWhiteboardItem item)
    {
        if (item is IHasPreCalculationAction hasPreCalculationAction)
        {
            hasPreCalculationAction.PreCalculationAction();
        }
        _model.CommitItem(item);
    }

private void Evaluate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void AddItemAtInsertionPointAndActivate(IWhiteboardItem item, string name)
    {
        IWhiteboardItemContainer whiteboardItemContainer = ActiveItem as IWhiteboardItemContainer;
        if (item is IEquationControl equationControl && ((whiteboardItemContainer != null && !(whiteboardItemContainer is CollapsibleAreaControl { ActiveItem: null })) || ActiveSection is HeaderFooterControl))
        {
            equationControl.ProhibitInputControl = true;
        }
        if (whiteboardItemContainer != null)
        {
            whiteboardItemContainer.AddItem(item);
        }
        else
        {
            // Release IL calls Execute through a local (callvirt); a new(...).Execute() call compiles to call.
            AddItemAtInsertionPointAndActivateCommand command = new AddItemAtInsertionPointAndActivateCommand(this, item, name);
            command.Execute();
        }
    }

private void HandlePageWideSizableDeletion(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items)
    {
        IEnumerable<IWhiteboardItem> sizableItems = items.Where(item => item is IPageWideSizable && !(item is IIncludeWorksheetControl));
        if (!sizableItems.Any())
        {
            return;
        }
        sizableItems.Each(item =>
        {
            Point location = GetWorksheetSectionItemLocation(item);
            UpdateItemPositionsAfterPageWideSizableRemoved(item, location, null);
        });
    }

private global::Quire.Core.TextFormat _contentTextFormat;

private void UpdateItemPositionsAfterPageWideSizableRemoved(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point location, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> companionItems) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void SetInsertionPointOnAppropriatePage(Point insertionPoint)
    {
        ActivePage = _pageManager.SetInsertionPointOnAppropriatePage(insertionPoint);
        if (ActivePage.ActiveItem == null)
        {
            WorksheetContent.ActiveItem = null;
        }

        ClearSelection();
    }

private void DeleteItemIfNeeded(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void UpdateWorksheetGridSize()
    {
        double num = (double)Pages.Count * PageLayout.PageBodyHeight;
        _worksheetGrid.GridArea = new Rect(new Size(WidthWithNonprintableSpace, num));
    }

private void PageRequestScrollHandler(IWorksheetPageSection section, Rect target)
    {
        VisualHelper.BringIntoView(section, target);
    }

private void ShowZombiesOnPage(IWorksheetPage page, bool createIfNeeded = true)
    {
        if (createIfNeeded && (page.HeaderZombie == null || page.FooterZombie == null))
        {
            DebugTools.DevAssert(page.HeaderZombie == page.FooterZombie, null);
            page.HeaderZombie = ControlCreator.CreateHeaderFooterZombieControl(Header);
            page.FooterZombie = ControlCreator.CreateHeaderFooterZombieControl(Footer);
            page.HeaderZombie?.SubscribeToZombieIsInitialized(OnZombieInitializationComplete);
            page.FooterZombie?.SubscribeToZombieIsInitialized(OnZombieInitializationComplete);
            bool isPageVisible = VisiblePages.Contains(page);
            ItemManager.SetElementIsOnVisiblePage(page.HeaderZombie, isPageVisible);
            ItemManager.SetElementIsOnVisiblePage(page.FooterZombie, isPageVisible);
        }
        if (createIfNeeded && page.PageNumber == 1 && (page.FirstPageHeaderZombie == null || page.FirstPageFooterZombie == null))
        {
            page.FirstPageHeaderZombie = ControlCreator.CreateHeaderFooterZombieControl(FirstPageHeader);
            page.FirstPageFooterZombie = ControlCreator.CreateHeaderFooterZombieControl(FirstPageFooter);
            bool isPageVisible2 = VisiblePages.Contains(page);
            ItemManager.SetElementIsOnVisiblePage(page.FirstPageHeaderZombie, isPageVisible2);
            ItemManager.SetElementIsOnVisiblePage(page.FirstPageFooterZombie, isPageVisible2);
        }
        if (page.PageNumber == 1 && IsFirstPageHeaderFooterDifferent)
        {
            page.Header = page.FirstPageHeaderZombie;
            page.Footer = page.FirstPageFooterZombie;
        }
        else
        {
            page.Header = page.HeaderZombie;
            page.Footer = page.FooterZombie;
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

public void OnZombieInitializationComplete()
    {
        HeaderFooterZombiesReady = true;
        Pages.Each(page =>
        {
            page.HeaderZombie?.UnsubscribeFromZombieIsInitialized(OnZombieInitializationComplete);
            page.FooterZombie?.UnsubscribeFromZombieIsInitialized(OnZombieInitializationComplete);
        });
    }

private static void RemoveZombiesFromPage(IWorksheetPage page)
    {
        if (page.HeaderZombie != null)
        {
            page.HeaderZombie.Dispose();
            page.HeaderZombie = null;
        }

        if (page.FooterZombie != null)
        {
            page.FooterZombie.Dispose();
            page.FooterZombie = null;
        }

        if (page.FirstPageHeaderZombie != null)
        {
            page.FirstPageHeaderZombie.Dispose();
            page.FirstPageHeaderZombie = null;
        }

        if (page.FirstPageFooterZombie != null)
        {
            page.FirstPageFooterZombie.Dispose();
            page.FirstPageFooterZombie = null;
        }

        page.Header = null;
        page.Footer = null;
    }

private void HandleItemsCollectionChanging(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
        case NotifyCollectionChangedAction.Remove:
        case NotifyCollectionChangedAction.Move:
            args.OldItems.OfType<IWhiteboardItem>().Each((IWhiteboardItem item) =>
            {
                ItemManager.SetHandleChangesEnabled(item, value: false);
            });
            break;
        default:
            DebugTools.FailAssertion("Unsupported action: {0}", args.Action);
            break;
        case NotifyCollectionChangedAction.Add:
            break;
        }
    }

private void HandleItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                HandleItemsAdded(args.NewItems.OfType<IWhiteboardItem>());
                break;
            case NotifyCollectionChangedAction.Remove:
                HandleItemsRemoved(args.OldItems.OfType<IWhiteboardItem>());
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", args.Action);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
        }
    }

private void HandleItemMoved(IWhiteboardItem item, Point oldLocation)
    {
        _pageManager.HandleItemMoved(item, oldLocation);
        FireChanged();
    }

private void HandleItemsAdded(IEnumerable<IWhiteboardItem> items)
    {
        items.Each(HandleItemAddedWithoutNotification);
        if (DebugTools.DevAssert(_disposeStatus == DisposeStatus.Alive, null).Passed)
        {
            FireChanged();
        }
    }

private void HandleIsOnVisiblePagePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        CheckFlagsAndSplitItemIfNeeded(sender as IWhiteboardItem, (bool)args.NewValue);
    }

private void CheckFlagsAndSplitItemIfNeeded(IWhiteboardItem item, bool isOnVisiblePage)
    {
        if (isOnVisiblePage && ItemManager.GetItemSplitCheckIsRequired(item))
        {
            PageManager.ExecuteOrScheduleSplitAttempt(item, null, false);
        }
    }

private void HandleItemAddedWithoutNotification(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleProtectedStateChanged(IProtectableControl control)
    {
        ICollapsibleAreaControl collapsibleAreaControl = control as ICollapsibleAreaControl;
        if (DebugTools.AssertNotNull(collapsibleAreaControl, null).Failed || DebugTools.DevAssert(IsContentActive, null).Failed)
        {
            return;
        }
        if (!collapsibleAreaControl.ProtectionLevel.IsProtected())
        {
            if (!collapsibleAreaControl.IsCollapsed)
            {
                RemoveGridRestrictedAreaForItem(collapsibleAreaControl);
                ActivateAndNavigateIntoArea(collapsibleAreaControl);
                return;
            }
            // Release IL branches collapsed areas to the shared method exit.
            goto Done;
        }
        UpdateGridRestrictedAreaForItem(collapsibleAreaControl);
        bool cannotActivate = !_itemManager.GetCanActivate(collapsibleAreaControl) && ActiveItem == collapsibleAreaControl;
        bool cannotSelect = !collapsibleAreaControl.GetCanSelect() && SelectedItems.Contains(collapsibleAreaControl);
        if (cannotActivate | cannotSelect)
        {
            if (cannotSelect)
            {
                ActiveSection.UnSelectItem(collapsibleAreaControl);
            }
            NavigateOutsideMovingAreaDownIfNeeded(collapsibleAreaControl, Direction.Up, moveDown: true);
        }
        else
        {
            DebugTools.DevAssert(_itemManager.GetCanActivate(collapsibleAreaControl) || !GetItemRect(collapsibleAreaControl).ContainsAccountingForMeasurementError(InsertionPoint), null);
        }
    Done: // lint-ok: Release IL requires the collapsed path to branch to the shared exit.
        ;
    }

private void HandleActivatePartContainingChildRequested(IWhiteboardItemContainer wbContainer, IWhiteboardItem item)
    {
        IWhiteboardItem whiteboardItem = wbContainer as IWhiteboardItem;
        if (whiteboardItem != null && ItemManager.IsItemLoaded(item) && ItemManager.GetIsBringToView(item))
        {
            _pageManager.ActivateDescendantParentIfNeeded(whiteboardItem, item);
        }
    }

private void HandleItemSplitRequest(ICustomSplitItem customSplitItem)
    {
        customSplitItem.ValidateLayout();
        _pageManager.SplitItemIfNeeded(customSplitItem, null, false);
    }

private void HandleCollapsedStateChanged(ICollapsibleAreaControl item)
    {
        if (!item.IsCollapsed && !SelectedItems.Contains(item) && _itemManager.GetCanActivate(item))
        {
            ActivateAndNavigateIntoArea(item);
        }

        UpdateNonprintableSpaceIndicatorWhenAreaHeightChanged(item);
    }

private void NavigateOutsideMovingAreaDownIfNeeded(global::Quire.Controls.Core.ICollapsibleAreaControl area, global::Quire.Direction direction, bool moveDown) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void ActivateAndNavigateIntoArea(global::Quire.Controls.Core.ICollapsibleAreaControl item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void UpdateNonprintableSpaceIndicatorWhenAreaHeightChanged(global::Quire.Controls.Core.ICollapsibleAreaControl area) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void AddSizeChangedHandlerToItemMap(IWhiteboardItem item, WhiteboardItemSizeChangedEventHandler eventHandler)
    {
        IList<WhiteboardItemSizeChangedEventHandler> handlers;
        if (!_itemSizeChangedHandlerMap.TryGetValue(item, out handlers))
        {
            handlers = new List<WhiteboardItemSizeChangedEventHandler>();
            _itemSizeChangedHandlerMap.Add(item, handlers);
        }

        handlers.Add(eventHandler);
    }

private void HandleItemsRemoved(IEnumerable<IWhiteboardItem> items)
    {
        items.Each(HandleItemRemovedWithoutNotification);
        if (_disposeStatus == DisposeStatus.Alive)
        {
            FireChanged();
        }
    }

private void HandleItemRemovedWithoutNotification(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleItemSizeChanged(IWhiteboardItem item, Size oldSize, Size newSize)
    {
        if (!ItemManager.IsResizingItem(item))
        {
            _pageManager.HandleItemSizeChanged(item, oldSize, newSize);
        }

        if (item == ActiveItem || ItemManager.IsResizingItem(item))
        {
            ChangePageLayoutAndGridAreaIfNeeded(item);
        }

        SetEqResizable(item);
    }

private void SetEqResizable(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void RemoveItemAndSetInsertionPoint(IWhiteboardItem item, Direction deleteDirection)
    {
        if (item == ActiveItem)
        {
            DeleteActiveItem();
        }
    }

private void HandleSelectedItemsChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                IWorksheetPage page = null;
                e.NewItems.OfType<IWhiteboardItem>().Each(item =>
                {
                    page = _pageManager.GetItemPage(item);
                    page.PageBody.SelectItem(item);
                    ItemManager.SelectItem(item);
                });
                _model.InputOutputRegionManager.HandleElementsListSelectedInParentContainer(e.NewItems.OfType<IIORegionBase>());
                ActivePage = page;
                break;
            }
            case NotifyCollectionChangedAction.Remove:
                if (_disposeStatus == DisposeStatus.Alive)
                {
                    e.OldItems.OfType<IWhiteboardItem>().Each(item =>
                    {
                        _pageManager.GetItemPage(item).PageBody.UnSelectItem(item);
                        ItemManager.DeselectItem(item);
                    });
                }
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
        if (_disposeStatus == DisposeStatus.Alive && IsContentActive)
        {
            UpdateIsRegionBorderShownState();
            UpdateCalculationButtonStatus();
            OnPropertyChanged("SelectedItems");
        }
    }

private void HandleActiveDescendantChanged()
    {
        if (IsContentActive)
        {
            IWhiteboardItem activeItem = ActiveItem;
            IWhiteboardItem activeDescendant = ActiveDescendant;
            if (activeItem != activeDescendant && ItemManager.IsItemLoaded(activeDescendant) && ItemManager.GetIsBringToView(activeItem))
            {
                _pageManager.ActivateDescendantParentIfNeeded(activeItem, activeDescendant);
            }

            UpdateIsRegionBorderShownState();
            OnPropertyChanged("ActiveDescendant");
        }
    }

private void HandleSelectedDescendantsChanged()
    {
        if (IsContentActive)
        {
            UpdateIsRegionBorderShownState();
            OnPropertyChanged("SelectedDescendants");
        }
    }

private void HandleActiveItemChanged(IWhiteboardItem oldItem, IWhiteboardItem newItem)
    {
        DeleteItemIfNeeded(oldItem);
        if (IsContentActive)
        {
            UpdateIsRegionBorderShownState();
            UpdateCalculationButtonStatus();
            OnPropertyChanged("ActiveItem");
        }
        if (oldItem is IWhiteboardItemContainer)
        {
            oldItem.PropertyChanged -= HandleActiveContainerPropertyChanged;
        }
        if (newItem is IWhiteboardItemContainer)
        {
            newItem.PropertyChanged += HandleActiveContainerPropertyChanged;
        }
    }

private void HandleActiveContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (!IsContentActive)
        {
            return;
        }

        if (sender != ActiveItem)
        {
            return;
        }

        IHideFromCalculationButtonStatus status = sender as IHideFromCalculationButtonStatus;
        if (status == null)
        {
            return;
        }

        if (e.PropertyName == "HideFromCalculationButtonEnabled")
        {
            HideFromCalculationButtonEnabled = status.HideFromCalculationButtonEnabled;
            return;
        }

        if (e.PropertyName == "HideFromCalculationButtonChecked")
        {
            HideFromCalculationButtonChecked = status.HideFromCalculationButtonChecked;
        }
    }

private void HandleFooterPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (!IsFooterActive)
        {
            return;
        }

        if (e.PropertyName != "ActiveDescendant" && e.PropertyName != "SelectedDescendants")
        {
            return;
        }

        UpdateIsRegionBorderShownState();
        OnPropertyChanged(e.PropertyName);
    }

private void HandleHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (!IsHeaderActive)
        {
            return;
        }

        if (e.PropertyName != "ActiveDescendant" && e.PropertyName != "SelectedDescendants")
        {
            return;
        }

        UpdateIsRegionBorderShownState();
        OnPropertyChanged(e.PropertyName);
    }

private void UpdateHasSelectionProperty()
    {
        HasSelection = SelectedDescendants.Any();
    }

private void UpdateIsInLimboProperty()
    {
        IsInLimbo = ActiveDescendant == null || ActiveDescendant is IWhiteboardControl;
    }

private void HandlePagesCollectionChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems.OfType<IWorksheetPage>().Each(page =>
                {
                    page.RequestScrollTo += PageRequestScrollHandler;
                    ShowZombiesOnPage(page);
                });
                break;
            case NotifyCollectionChangedAction.Remove:
                e.OldItems.OfType<IWorksheetPage>().Each(page =>
                {
                    page.RequestScrollTo -= PageRequestScrollHandler;
                    RemoveZombiesFromPage(page);
                });
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
        UpdateWorksheetHeight();
        UpdateTotalPagesOffsetOnPageChange(Pages.Count - PageCount);
        PageCount = Pages.Count;
        if (_disposeStatus == DisposeStatus.Alive)
        {
            UpdateWorksheetGridSize();
        }
    }

private void RemoveItemFromPage(IWhiteboardItem item, bool removeEmptyPages)
    {
        _pageManager.RemoveItemFromAppropriatePage(item, removeEmptyPages);
        RemoveGridRestrictedAreaForItem(item);
    }

private int GetPageIndex(double yOffset)
    {
        if (PageLayout.PageBodyHeight != 0.0)
        {
            return (int)((yOffset + PageLayout.GridCellSize.Height / 2.0) / PageLayout.PageBodyHeight);
        }
        return 0;
    }

private void AddItemToAppropriatePage(IWhiteboardItem item)
    {
        _pageManager.AddItemToAppropriatePage(item);
        UpdateGridRestrictedAreaForItem(item);
    }

private void HandleItemGotFocus(IWhiteboardItem item)
    {
        WorksheetContent.HandleItemGotFocus(item);
        ChangePageLayoutAndGridAreaIfNeeded(item);
        IWhiteboardItem activeDescendant = WorksheetContent.ActiveDescendant;
        if (ItemManager.SmartDescendantsSatisfyingCondition(item, ItemManager.IsSuppressBringIntoView).IsNullOrEmpty())
        {
            // Release IL tests activeDescendant != item first, so the BringIntoView(item) case is the else branch.
            if (activeDescendant != item)
            {
                if (ItemManager.IsItemLoaded(activeDescendant))
                {
                    VisualHelper.BringIntoView(item, ItemManager.TransformBoundsToAncestor(activeDescendant, item, new Rect(ItemManager.GetRenderSize(activeDescendant))));
                }
            }
            else
            {
                VisualHelper.BringIntoView(item);
            }
        }

        SetActivePage(item);
    }

private void SetActivePage(IWhiteboardItem activeItem)
    {
        ActivePage = _pageManager.GetItemPage(activeItem);
        if (DebugTools.DevAssert(ActivePage.PageBody.WhiteboardItems.Contains(activeItem), null).Passed)
        {
            ActivePage.PageBody.ActiveItem = activeItem;
        }
    }

private void ChangePageLayoutAndGridAreaIfNeeded(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void RemoveActiveSectionItemAndRestoreInsertionPoint(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Point insertionPoint) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void SetupDefaultPageLayout() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void SetupCommandBindings()
    {
        CreateCommandBinding(EditingCommands.MoveDownByLine, (object param) =>
        {
            DirectionalMoveHandler(Direction.Down);
        }, (object param) => CanMoveInDirection(Direction.Down));
        CreateCommandBinding(EditingCommands.MoveUpByLine, (object param) =>
        {
            DirectionalMoveHandler(Direction.Up);
        }, (object param) => CanMoveInDirection(Direction.Up));
        CreateCommandBinding(EditingCommands.MoveRightByCharacter, (object param) =>
        {
            DirectionalMoveHandler(Direction.Right);
        });
        CreateCommandBinding(EditingCommands.MoveLeftByCharacter, (object param) =>
        {
            DirectionalMoveHandler(Direction.Left);
        });
        CreateCommandBinding(EditingCommands.Delete, (object param) =>
        {
            HandleDelete();
        });
        CreateCommandBinding(EditingCommands.Backspace, (object param) =>
        {
            HandleBackspace();
        });
        CreateCommandBinding(EditingCommands.EnterParagraphBreak, (object param) =>
        {
            HandleEnter();
        });
        CreateCommandBinding(EditingCommands.TabForward, (object param) =>
        {
            HandleTab((FocusNavigationDirection)0);
        });
        CreateCommandBinding(EditingCommands.TabBackward, (object param) =>
        {
            HandleTab((FocusNavigationDirection)1);
        });
        CreateCommandBinding(EquationCommands.InsertOperator, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, CanInsertOperator);
        CreateCommandBinding(EquationCommands.InsertEvaluation, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertSymbolicEvaluation, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertValue, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertId, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertMatrix, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertInputControl, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquationWithInputControl(cmd, param as InputControlTypes?);
        }, (object param) => CanCreateInputControlOfType<IEquationControl>(param as InputControlTypes?));
        CreateCommandBinding(EquationCommands.InsertFunction, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertString, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertKeyword, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(EquationCommands.InsertModifier, (RoutedCommand cmd, object param) =>
        {
            CreateNewEquation(cmd, param);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(OleBaseComponentCommands.NewOleComponentExpression, (object param) =>
        {
            CreateNewEquation(param as string);
        });
        CreateCommandBinding(WorksheetCommands.NewMathRegion, (object param) =>
        {
            CreateNewEquation(param as string);
        }, (object param) => CanCreateControlOfType<IEquationControl>());
        CreateCommandBinding(WorksheetCommands.NewImageRegion, (object param) =>
        {
            CreateImageControl();
        }, (object param) => CanCreateControlOfType<IImageControl>());
        CreateCommandBinding(WorksheetCommands.NewImageRegionFromPath, (object param) =>
        {
            CreateImageControl(param as string);
        }, (object param) => CanCreateControlOfType<IImageControl>());
        CreateCommandBinding(WorksheetCommands.NewTextRegion, (object param) =>
        {
            CreateTextboxControl(param as string);
        }, (object param) => CanCreateControlOfType<IRichTextBase>());
        CreateCommandBinding(WorksheetCommands.NewPlot, (object param) =>
        {
            CreateXYPlotControl(param as XyEquation);
        }, (object param) => CanCreateControlOfType<IPlotControl>());
        CreateCommandBinding(WorksheetCommands.NewPolarPlot, (object param) =>
        {
            CreatePolarPlotControl(param as XyEquation);
        }, (object param) => CanCreateControlOfType<IPlotControl>());
        CreateCommandBinding(WorksheetCommands.NewContourPlot, (object param) =>
        {
            CreateContourPlotControl(param as XyzEquation);
        }, (object param) => CanCreateControlOfType<IPlotControl>());
        CreateCommandBinding(WorksheetCommands.New3DPlot, (object param) =>
        {
            Create3DPlotControl(param as XyzEquation);
        }, (object param) => CanCreateControlOfType<IPlotControl>());
        CreateCommandBinding(WorksheetCommands.NewSpecTable, (object param) =>
        {
            CreateSpecTableControl(param);
        }, (object param) => CanCreateControlOfType<ISpecTableControl>());
        CreateCommandBinding(WorksheetCommands.NewSolveBlock, (object param) =>
        {
            CreateSolveBlockControl();
        }, (object param) => CanCreateControlOfType<ISolveBlockControl>());
        CreateCommandBinding(WorksheetCommands.NewExcelComponent, (object param) =>
        {
            CreateExcelComponentControl();
        }, (object param) => CanCreateExcelComponent());
        CreateCommandBinding(WorksheetCommands.InsertOleObject, (object param) =>
        {
            CreateOleObjectControl(param as OleObjectControlCreationMethod);
        }, (object param) => CanCreateOleObjectControl());
        CreateCommandBinding(WorksheetCommands.NewChartComponent, (object param) =>
        {
            CreateChartComponentControl();
        }, (object param) => CanCreateChartComponent());
        CreateCommandBinding(WorksheetCommands.NewCollapsibleArea, (object param) =>
        {
            CreateCollapsibleAreaControl();
        }, (object param) => CanCreateControlOfType<ICollapsibleAreaControl>());
        CreateCommandBinding(WorksheetCommands.NewParagraph, (object param) =>
        {
            CreateTextBlockControl(param as string);
        }, (object param) => CanCreateParagraphControl());
        CreateCommandBinding(WorksheetCommands.NewIncludeRegion, (object param) =>
        {
            CreateIncludeControl(param as string);
        }, (object param) => CanCreateControlOfType<IIncludeWorksheetControl>());
        CreateCommandBinding(WorksheetCommands.NewFileNameRegion, (object param) =>
        {
            CreateFileNameControl();
        }, (object param) => CanCreateFileSpecificRegion<IFileNameControl>());
        CreateCommandBinding(WorksheetCommands.NewFilePathRegion, (object param) =>
        {
            CreateFilePathControl();
        }, (object param) => CanCreateFileSpecificRegion<IFilePathControl>());
        CreateCommandBinding(WorksheetCommands.NewSavedDateRegion, (object param) =>
        {
            CreateSavedDateControl(param as string);
        }, (object param) => CanCreateSavedDateControl());
        CreateCommandBinding(WorksheetCommands.NewPageNumberRegion, (object param) =>
        {
            CreatePageNumberControl(param as string);
        }, (object param) => CanCreatePageNumberControl());
        CreateCommandBinding(WorksheetCommands.NewPageBreak, (object param) =>
        {
            InsertPageBreak();
        }, (object param) => CanCreateControlOfType<IWorksheetPageBreak>());
        CreateCommandBinding(WorksheetCommands.RemoveFileNameRegion, (object param) =>
        {
            RemoveRegionOfType<IFileNameControl>();
        }, (object param) => CanRemoveRegionOfType<IFileNameControl>());
        CreateCommandBinding(WorksheetCommands.RemoveFilePathRegion, (object param) =>
        {
            RemoveRegionOfType<IFilePathControl>();
        }, (object param) => CanRemoveRegionOfType<IFilePathControl>());
        CreateCommandBinding(WorksheetCommands.RemoveSavedDateRegion, (object param) =>
        {
            RemoveRegionOfType<ISavedDateControl>();
        }, (object param) => CanRemoveRegionOfType<ISavedDateControl>());
        CreateCommandBinding(WorksheetCommands.RemovePageNumberRegion, (object param) =>
        {
            RemoveRegionOfType<IPageNumberControl>();
        }, (object param) => CanRemoveRegionOfType<IPageNumberControl>());
        CreateCommandBinding(WhiteboardCommands.DeleteActiveItem, (object param) =>
        {
            DeleteActiveItem();
        }, (object param) => CanDeleteActiveItem());
        CreateCommandBinding(WhiteboardCommands.DeleteSelectedItems, (object param) =>
        {
            DeleteSelectedItems();
        }, (object param) => CanDeleteSelectedItems());
        CreateCommandBinding(WhiteboardCommands.DeleteActiveOrSelectedItems, (object param) =>
        {
            DeleteActiveOrSelectedItems();
        }, (object param) => CanDeleteActiveOrSelectedItems());
        CreateCommandBinding(WorksheetCommands.TogglePageLayout, (object param) =>
        {
            WorksheetPageLayoutMode = (PageLayoutMode)param;
        }, (object param) => CanTogglePageLayout(param));
        CreateCommandBinding(WorksheetCommands.ControlEnterContextCommand, (object param) =>
        {
            HandleControlEnter();
        });
        CreateCommandBinding(WhiteboardCommands.SelectAllRegions, (object param) =>
        {
            HandleSelectAll();
        });
        CreateCommandBinding(WorksheetCommands.GotoPage, (object param) =>
        {
            HandleGoToPage(param);
        }, (object param) => CanGotopage(param));
        CreateCommandBinding(ApplicationCommands.Copy, (object param) =>
        {
            CopySelection();
        }, (object param) => CanCopySelection());
        CreateCommandBinding(ApplicationCommands.Cut, (object param) =>
        {
            CutSelection();
        }, (object param) => CanCutSelection());
        CreatePreviewCommandBinding(ApplicationCommands.Paste, (object param) =>
        {
            Paste();
        }, (object param) => CanPaste());
        CreateCommandBinding(WorksheetCommands.InsertSpace, (object param) =>
        {
            InsertSpace(param as int?);
        }, (object param) => CanInsertSpace());
        CreateCommandBinding(WorksheetCommands.RemoveSpaceBelow, (object param) =>
        {
            RemoveSpace(param as int?, Direction.Down);
        }, (object param) => CanRemoveSpace(Direction.Down));
        CreateCommandBinding(WorksheetCommands.RemoveSpaceAbove, (object param) =>
        {
            RemoveSpace(param as int?, Direction.Up);
        }, (object param) => CanRemoveSpace(Direction.Up));
        CreateCommandBinding(WorksheetCommands.InsertPage, (object param) =>
        {
            InsertSpace(LinesPerPage);
        }, (object param) => CanInsertSpace());
        CreateCommandBinding(WorksheetCommands.ToggleShowGrid, (object param) =>
        {
            ToggleShowGrid();
        });
        CreateCommandBinding(WorksheetCommands.ToggleSolveBlockLabelsVisibilityCommand, (object param) =>
        {
            ToggleIsSolveBlockLabelVisible();
        }, (object param) => CanToggleIsSolveBlockLabelVisible());
        CreateCommandBinding(WorksheetCommands.ShowMajorGridlinesCommand, (object param) =>
        {
            ToggleShowMajorGridlines();
        });
        CreateCommandBinding(WorksheetCommands.ToggleShowFrame, (object param) =>
        {
            ToggleShowFrame();
        });
        CreateCommandBinding(EquationCommands.Commit, (object param) =>
        {
            Evaluate();
        });
        CreateCommandBinding(EquationCommands.ToggleHideFromCalculationContext, (object param) =>
        {
            ToggleHideFromCalculationContext();
        }, (object param) => CanToggleHideFromCalculationContext());
        CreateCommandBinding(WorksheetCommands.GoToFirstError, (object param) =>
        {
            GoToFirstError();
        }, (object param) => ErrorCanTraceBackward());
        CreateCommandBinding(WorksheetCommands.GoToPreviousError, (object param) =>
        {
            GoToPreviousError();
        }, (object param) => ErrorCanTraceBackward());
        CreateCommandBinding(WorksheetCommands.GoToLastError, (object param) =>
        {
            GoToLastError();
        }, (object param) => ErrorCanTraceForward());
        CreateCommandBinding(WorksheetCommands.GoToNextError, (object param) =>
        {
            GoToNextError();
        }, (object param) => ErrorCanTraceForward());
        CreateCommandBinding(WorksheetCommands.StopRunningCalculations, (object param) =>
        {
            HandleStopRunningCalculations();
        });
        CreateCommandBinding(WorksheetCommands.RecalculateWorksheet, (object param) =>
        {
            FireRecalculateAll();
        });
        CreateCommandBinding(WorksheetCommands.CalculateNow, (object param) =>
        {
            FireCalculateNow();
        });
        CreateCommandBinding(WorksheetCommands.ToggleAutoCalculation, (object param) =>
        {
            HandleToggleAutoCalculation((bool)param);
        });
        CreateCommandBinding(WorksheetCommands.RemoveAnnotations, (object param) =>
        {
            HandleRemoveAnnotations();
        }, (object param) => RemoveAnnotationsCanExecute());
        CreateCommandBinding(WhiteboardCommands.SeparateOverlappedItemsVertically, (object param) =>
        {
            SeparateOverlappedItems(Dimension.Vertical);
        }, (object param) => CanSeparateOverlappedItems(Dimension.Vertical));
        CreateCommandBinding(WhiteboardCommands.SeparateOverlappedItemsHorizontally, (object param) =>
        {
            SeparateOverlappedItems(Dimension.Horizontal);
        }, (object param) => CanSeparateOverlappedItems(Dimension.Horizontal));
        CreateCommandBinding(IORegionCommands.ToggleInputRegion, (object param) =>
        {
            ToggleInputRegions();
        }, (object param) => CanToggleRegionsAsInput());
        CreateCommandBinding(IORegionCommands.ToggleOutputRegion, (object param) =>
        {
            ToggleOutputRegions();
        }, (object param) => CanToggleRegionsAsOutput());
        CreateCommandBinding(EditingCommands.MoveUpByPage, (object param) =>
        {
            PageUp();
        });
        CreateCommandBinding(EditingCommands.MoveDownByPage, (object param) =>
        {
            PageDown();
        });
        CreateCommandBinding(RichTextBase.AddToDictionaryCommand, (object param) =>
        {
            HandleAddToDictionary();
        }, (object param) => CanAddToDictionary());
        CreateCommandBinding(RichTextBase.TextStyleCreateFromCommand, (object param) =>
        {
            HandleCreateStyleFrom();
        }, (object param) => CanCreateStyleFrom());
        CreateCommandBinding(RegionCommands.TagCommand, (object param) =>
        {
            HandleBookmarkTag();
        }, (object param) => CanBookmarkTag());
        CreateCommandBinding(RegionCommands.RemoveTagCommand, (object param) =>
        {
            HandleRemoveBookmarkTag();
        }, (object param) => CanRemoveBookmarkTag());
        CreateCommandBinding(WorksheetCommands.EditHeader, (object param) =>
        {
            ToggleHeaderActivation();
        });
        CreateCommandBinding(WorksheetCommands.EditFooter, (object param) =>
        {
            ToggleFooterActivation();
        });
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

public void UpdateIsRegionBorderShownState() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void HandleSetCurrentProofingLanguage(CultureInfo culture)
    {
        CurrentProofingLanguage = XmlLanguageHelper.ConvertCultureInfoToXmlLanguage(culture);
    }

public void OnRequestRestoreInsertionPoint(IWorksheetControl worksheetControl, RoutedEventArgs eventArgs)
    {
        Activate(worksheetControl);
        eventArgs.Handled = true;
    }

    public void Activate(global::Quire.Controls.Worksheet.IWorksheetControl worksheetControl) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool CanAddToDictionary()
    {
        if (ActiveDescendant is ISpellCheckRegion spellCheckRegion)
        {
            return spellCheckRegion.CanAddToDictionary();
        }
        return false;
    }

private void HandleAddToDictionary() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleCreateStyleFrom() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool CanCreateStyleFrom()
    {
        if (ActiveDescendant is RichTextBase richTextBase)
        {
            List<Run> list = richTextBase.Selection.EnumerateRunsInRange().ToList();
            if (list.Count() > 1)
            {
                for (int i = 1; i < list.Count; i++)
                {
                    if (!RichTextBase.compareRuns(list[0], list[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }

public void RefreshDictionary() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

public event global::System.Action DictionaryUpdated;

private void FireDictionaryUpdated()
    {
        DictionaryUpdated?.Invoke();
    }

private bool CanInsertOperator(object operatorName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void MoveByPage(Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool CanSeparateOverlappedItems(global::Quire.Dimension dimension) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void SeparateOverlappedItems(global::Quire.Dimension dimension) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void SetupHeaderFooter()
    {
        Header = ControlCreator.CreateHeaderFooterControl(PageLayout.HeaderFooterGridCellSize, HeaderFooterRole.Header);
        Footer = ControlCreator.CreateHeaderFooterControl(PageLayout.HeaderFooterGridCellSize, HeaderFooterRole.Footer);
        FirstPageHeader = ControlCreator.CreateHeaderFooterControl(PageLayout.HeaderFooterGridCellSize, HeaderFooterRole.Header);
        FirstPageFooter = ControlCreator.CreateHeaderFooterControl(PageLayout.HeaderFooterGridCellSize, HeaderFooterRole.Footer);
        PageLayout.HeaderFooterGridCellSizeChanged += Header.GridCellSizeChangedHandler;
        PageLayout.HeaderFooterGridCellSizeChanged += Footer.GridCellSizeChangedHandler;
        PageLayout.HeaderFooterGridCellSizeChanged += FirstPageHeader.GridCellSizeChangedHandler;
        PageLayout.HeaderFooterGridCellSizeChanged += FirstPageFooter.GridCellSizeChangedHandler;
        Header.Activated += () => ActiveSection = Header;
        Footer.Activated += () => ActiveSection = Footer;
        FirstPageHeader.Activated += () => ActiveSection = FirstPageHeader;
        FirstPageFooter.Activated += () => ActiveSection = FirstPageFooter;
        Action<Direction, bool> navigateOut = (direction, navigateByPage) =>
        {
            if (!navigateByPage)
            {
                HandleHeaderFooterNavigation(direction);
            }
            else if (direction == Direction.Up)
            {
                HandleHeaderFooterPageUp();
            }
            else
            {
                HandleHeaderFooterPageDown();
            }
        };
        Header.NavigateOutOfHeaderFooter += navigateOut;
        Footer.NavigateOutOfHeaderFooter += navigateOut;
        FirstPageHeader.NavigateOutOfHeaderFooter += navigateOut;
        FirstPageFooter.NavigateOutOfHeaderFooter += navigateOut;
        Header.PropertyChanged += HandleHeaderPropertyChanged;
        Footer.PropertyChanged += HandleFooterPropertyChanged;
        FirstPageHeader.PropertyChanged += HandleHeaderPropertyChanged;
        FirstPageFooter.PropertyChanged += HandleFooterPropertyChanged;
    }

private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> ItemsInRelativeBounds(global::System.Windows.Rect bounds, object boundsRelativeTo) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private global::Quire.Controls.Core.IWhiteboardItem FindNextItemForTabNavigation(global::System.Windows.Input.FocusNavigationDirection direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GetTabNavigatableItems(bool includingActiveItem = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void ActivateSection(global::Quire.Controls.Worksheet.IWorksheetSection section, global::Quire.Controls.Worksheet.IWorksheetPage page, global::System.Nullable<global::System.Windows.Point> position) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void MakeContentActive()
    {
        if (!IsContentActive)
        {
            ActiveSection.SaveVisualState();
            ActiveSection.ClearVisualState();
            ActiveSection = WorksheetContent;
        }
    }

public void SelectItemsInBounds(IWorksheetControl worksheet, Point start, Point end) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleDragSelection(global::System.Windows.UIElement originalSource, global::System.Windows.Point position) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void UpdateNonprintableSpaceIndicator(IWorksheetPage page)
    {
        page.UpdateNonPrintableSpaceIndicator(PageLayout.PageBodyWidth);
    }

private void RecreateDocumentAfterPageSizeChange() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void InsertPageBreak(Point location)
    {
        // Release IL calls Execute through a local (callvirt); a new(...).Execute() call compiles to call.
        PageBreakCommand command = new PageBreakCommand(this, location);
        command.Execute();
    }

private void RemovePageBreakForPageResize(global::Quire.Controls.Core.IWorksheetPageBreak pageBreak) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandlePagebreaksChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            e.NewItems.OfType<IWorksheetPageBreak>().Each(SetPagebreakVisualOffset);
            return;
        }
        DebugTools.DevAssert(e.NewItems == null, "Got new items on unsupported action: {0}", e.Action);
    }

private void SetPagebreakVisualOffset(IWorksheetPageBreak pagebreak)
    {
        Point location = WorksheetContent.GetItemLocation(pagebreak);
        int pageIndex = GetPageIndex(location.Y);
        double margin = (PageLayout.DisplayHeight - PageLayout.PageHeight) / 2.0;
        pagebreak.VerticalPosition = PageLayout.DisplayHeight * pageIndex - margin - 2.0 * PageBreaks.IndexOf(pagebreak);
    }

private void RequestMaximumSizeHandler(IWhiteboardControl whiteboard)
    {
        IWhiteboardItem item = whiteboard as IWhiteboardItem;
        if (item == null)
        {
            return;
        }

        Size sizeLimit = FindSizeLimit(item);
        whiteboard.UpdateSizeLimits(new Size(sizeLimit.Width, double.PositiveInfinity));
        if (!(whiteboard is IPageWideSizable))
        {
            whiteboard.ForbidGrowingMovementRight = OverlapsRightMargin(item);
        }
    }

private void UpdateSizeLimit(IWhiteboardControl whiteBoard)
    {
        Size sizeLimit = FindSizeLimit((IWhiteboardItem)whiteBoard);
        whiteBoard.UpdateSizeLimits(new Size(sizeLimit.Width, sizeLimit.Height));
    }

private global::System.Windows.Size FindSizeLimit(global::Quire.Controls.Core.IWhiteboardItem whiteboardItem) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private global::System.Windows.Size FindSizeLimit(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> itemsToIgnore, bool considerMargin) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GetOverlappingItems(global::Quire.Controls.Core.IWhiteboardItem item, bool includingNonPrintableArea = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool OverlapsRightMargin(IWhiteboardItem item)
    {
        Size renderSize = ItemManager.GetRenderSize(item);
        Point worksheetSectionItemLocation = GetWorksheetSectionItemLocation(item);
        if (WorksheetPageLayoutMode == PageLayoutMode.PageMode)
        {
            return renderSize.Width + worksheetSectionItemLocation.X >= PageLayout.PageBodyWidth;
        }
        return false;
    }

private void ChildTriggeredResizeHandler(global::Quire.Controls.Core.Whiteboard.IWhiteboardControl whiteboard, global::System.Windows.Size oldSize, global::System.Windows.Size newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void UpdateGridRestrictedAreaForItem(global::Quire.Controls.Core.IWhiteboardItem item) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void RemoveGridRestrictedAreaForItem(IWhiteboardItem item)
    {
        _worksheetGrid.RestrictedAreas.Remove(item);
        _worksheetGrid.ProtectedAreas.Remove(item);
    }

private void HandlePageWideSizableHeightChanged(global::Quire.Controls.Core.IWhiteboardItem item, global::System.Windows.Size oldSize, global::System.Windows.Size newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandlePageWideSizableSizeChanged(IWhiteboardItem item, Size oldSize, Size newSize)
    {
        HandlePageWideSizableHeightChanged(item, oldSize, newSize);
    }

private void HandleHeaderFooterNavigation(Direction direction) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleHeaderFooterPageUp()
    {
        bool isFirstPage = ActivePage?.PageNumber == 1;
        if (IsHeaderActive)
        {
            if (ActivePage != Pages.First())
            {
                IWorksheetPage page = Pages[Pages.IndexOf(ActivePage) - 1];
                ActivateSection(Footer, page, InsertionPoint);
            }
        }
        else if (IsFooterActive)
        {
            IHeaderFooterControl section = ((IsFirstPageHeaderFooterDifferent & isFirstPage) ? FirstPageHeader : Header);
            ActivateSection(section, ActivePage, InsertionPoint);
        }
    }

private void HandleHeaderFooterPageDown()
    {
        bool isFirstPage = ActivePage?.PageNumber == 1;
        if (IsHeaderActive)
        {
            IHeaderFooterControl section = ((IsFirstPageHeaderFooterDifferent & isFirstPage) ? FirstPageFooter : Footer);
            ActivateSection(section, ActivePage, InsertionPoint);
        }
        else if (IsFooterActive && ActivePage != Pages.Last())
        {
            IWorksheetPage page = Pages[Pages.IndexOf(ActivePage) + 1];
            ActivateSection(Header, page, InsertionPoint);
        }
    }

private void HookupCollectionChangedHandlers()
    {
        Pages.RangeChanged += HandlePagesCollectionChanged;
        PageBreaks.RangeChanged += HandlePagebreaksChanged;
        WorksheetItems.RangeChanging += HandleItemsCollectionChanging;
        WorksheetItems.RangeChanged += HandleItemsCollectionChanged;
        WorksheetContent.SelectedItems.RangeChanged += HandleSelectedItemsChanged;
    }

private void SetupDisplayFormats()
    {
        ContentTextFormat = new TextFormat("areaLayoutStyle");
        DisplayNumberFormat = new NumberFormat();
        HeaderTextFormat = new TextFormat("headerLayoutStyle");
        FooterTextFormat = new TextFormat("footerLayoutStyle");
        FirstPageHeaderTextFormat = new TextFormat("headerLayoutStyle");
        FirstPageFooterTextFormat = new TextFormat("footerLayoutStyle");
        LabeledIdFormat = VisualHelper.GetDefaultLabeledIdFormat();
    }

private void HookupDraggerHandlers() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private readonly global::Quire.Controls.Worksheet.IWorksheetModel _model;

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

private void HandleItemsOverlapWithPageWideSizables(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandlePageWideSizableMoved(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items, double verticalOffset) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandlePageWideSizablePasted(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private double PushDownPageWideSizableToAvoidOverlap(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> foregroundItems, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> backgroundItems) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void PushDownOverlappedBackgroundItems(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> foregroundPageWideSizables, global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> backgroundItems) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void OffsetItemsVertically(double offset, IEnumerable<IWhiteboardItem> itemsToOffset)
    {
        CommandBuffer.ExecuteAndAttach(new MoveItemsCommandWithSideEffects(this, new Vector(0.0, offset), itemsToOffset.ToList()));
    }

private static bool IsPageWideSizablePredicate(IWhiteboardItem item)
    {
        return item is IPageWideSizable;
    }

private void UpdateCalculationButtonStatus() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

private bool RemoveAnnotationsCanExecute() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public void ImportData(string fileName)
    {
        InsertFileName(fileName);
    }

public bool CanImportData()
    {
        if (ActiveOrSelectedItems.Count() == 0)
        {
            return true;
        }
        if (ActiveDescendant is EquationControl)
        {
            return true;
        }
        if (!(ActiveDescendant is IWhiteboardControl whiteboardControl))
        {
            return false;
        }
        if (whiteboardControl.SelectedItems.Count() == 0)
        {
            return true;
        }
        return false;
    }

public bool HandleRequestBringIntoView(DependencyObject intoViewObject, Rect bringIntoViewRect) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

public IDictionary<IdLabels, ExtendedFont> GetDefaultLabeledIdFormat()
    {
        return VisualHelper.GetDefaultLabeledIdFormat();
    }

private IEnumerable<IFindRestrictor> GetSearchableRegions(IWhiteboardItem fromElement, IWhiteboardItem toElement) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private bool CanToggleIORegions<T>() where T : IIORegion
    {
        if (!IsContentActive)
        {
            return false;
        }

        if (_model.InputOutputRegionManager == null)
        {
            return false;
        }

        if (GetIORegions<T>().Count() == 0)
        {
            return false;
        }

        return true;
    }

private IEnumerable<IIORegion> GetInputRegions()
    {
        return GetIORegions<IInputRegion>();
    }

private IEnumerable<IIORegion> GetOutputRegions()
    {
        return GetIORegions<IOutputRegion>();
    }

private global::System.Collections.Generic.IEnumerable<T> GetIORegions<T>(global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> items = default) where T : global::Quire.Controls.Core.IORegions.IIORegion => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleRequestResizeToFitOrClearRestriction(IResizeToFitWhiteboardItem item)
    {
        item.RequestResizeToFitOrClearRestriction -= HandleRequestResizeToFitOrClearRestriction;
        ResizeItemToFit(item, assertIsNotReady: true);
    }

private void ResizeItemToFit(IResizeToFitWhiteboardItem item, bool assertIsNotReady)
    {
        if (item.ResizeState == ResizeToFitState.ResizingNotInitialized)
        {
            if (assertIsNotReady)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
            return;
        }
        if (item.ResizeState != ResizeToFitState.ResizingEnabled)
        {
            ItemManager.ClearMaxSize(item);
            return;
        }
        Size size = ItemManager.GetSize(item);
        if (DebugTools.DevAssert(size.Width.IsValid() && size.Height.IsValid(), null).Failed)
        {
            return;
        }
            double index = PageLayout.PageBodyWidth;
            double lastIndex = PageLayout.PageBodyHeight;
            Size size2 = size;
            double offsetIndex = size2.Width / size2.Height;
            if ((index / lastIndex).IsGreaterThan(offsetIndex))
            {
                index = lastIndex * offsetIndex;
            }
            else
            {
                lastIndex = index / offsetIndex;
            }
            if (size2.Height.IsGreaterThan(lastIndex))
            {
                size2.Height = lastIndex;
                size2.Width = size2.Height * offsetIndex;
            }
            if (size2.Width.IsGreaterThan(index))
            {
                size2.Width = index;
                size2.Height = size2.Width / offsetIndex;
            }
            ItemManager.SetSize(item, size2);
    }

private bool CanBookmarkTag()
    {
        if (!IsHeaderActive && !IsFooterActive)
        {
            return ActiveOrSelectedItems.Count() == 1;
        }
        return false;
    }

private void HandleBookmarkTag()
    {
        IWhiteboardItem item = ActiveItem ?? SelectedItems.First();
        if (DebugTools.AssertNotNull(item).Failed || DebugTools.AssertNotNull(_model.BookmarksManager).Failed)
        {
            return;
        }

        string bookmarkTag = BookmarkTag.GetBookmarkTag(item as DependencyObject);
        BookmarkTagViewModel viewModel = new BookmarkTagViewModel(bookmarkTag);
        if (BookmarkTagDialog.ShowBookmarkTagsDialog(viewModel, Application.Current.MainWindow))
        {
            _model.BookmarksManager.SetBookmarkTagCommand(item, bookmarkTag, viewModel.Tag);
        }
    }

private bool CanRemoveBookmarkTag() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

private void HandleRemoveBookmarkTag()
    {
        if (DebugTools.AssertNotNull(ActiveOrSelectedItems).Failed || DebugTools.AssertNotNull(_model.BookmarksManager).Failed)
        {
            return;
        }

        foreach (IWhiteboardItem item in ActiveOrSelectedItems)
        {
            string bookmarkTag = BookmarkTag.GetBookmarkTag(item as DependencyObject);
            _model.BookmarksManager.RemoveBookmarkTagCommand(item, bookmarkTag);
        }
    }

public void RequestNavigateHandler(IWorksheetControl worksheetControl, global::System.Windows.Navigation.RequestNavigateEventArgs eventArgs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

private partial class WorksheetMouseDownHandler
    {
        public bool HandleMouseDown(object originalSource, bool alreadyHandled)
        {
            ResetBringIntoViewOfParentItem(originalSource);
            return true;
        }

        public void HandleBlockedPreviewMouseButton(object originalSource)
        {
            ResetBringIntoViewOfParentItem(originalSource);
        }

        public void ResetBringIntoViewOfParentItem(object originalSource)
        {
            IWhiteboardItem whiteboardItem = GetWhiteboardItem(originalSource);
            if (whiteboardItem != null)
            {
                ItemManager.SetIsBringToView(whiteboardItem, value: true);
            }
        }

        private IWhiteboardItem GetWhiteboardItem(object originalSource)
        {
            return _worksheetViewModel.VisualHelper.FindWhiteboardItemAncestorIncludingSelf(originalSource);
        }

        private IWhiteboardItem GetWhiteboardItemTopAncestor(IWhiteboardItem item)
        {
            return _worksheetViewModel.VisualHelper.FindTopWhiteboardItemAncestorIncludingSelf(item);
        }

        private Point GetTranslatedPoint(object originalSource, object target, Point position)
        {
            return ItemManager.TranslatePoint(originalSource, target, position);
        }

        private Point GetInsertionPoint(Point position, IWorksheetPage page)
        {
            return new Point(position.X, position.Y + _worksheetViewModel.PageLayout.PageBodyHeight * (double)_worksheetViewModel.Pages.IndexOf(page));
        }
    }

internal partial class RemoveItemCommand
    {
        private void RemoveItem()
        {
            ViewModel._model.RemoveItem(_item);
        }
    }
}
