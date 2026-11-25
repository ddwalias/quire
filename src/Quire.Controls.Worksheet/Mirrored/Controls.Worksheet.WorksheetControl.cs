using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Core.IORegions;
using Quire.Calculation.Core;
using Quire.Core;
using Quire.Services;
using Quire.Wpf;
using Microsoft.Win32;

namespace Quire.Controls.Worksheet;

public partial class WorksheetControl
{
    public bool HasScriptingControls { get; set; }

    public string HelpServerPath
    {
        get => _helpServerPath;
        set
        {
            if (_helpServerPath == value)
            {
                return;
            }
            _helpServerPath = value;
            FirePropertyChanged(nameof(HelpServerPath));
        }
    }

    public bool useSavedAutoCalcStateOnLoad
    {
        get => _useSavedAutoCalcStateOnLoad;
        set
        {
            if (value != _useSavedAutoCalcStateOnLoad)
            {
                _useSavedAutoCalcStateOnLoad = value;
                FirePropertyChanged("IsAutoCalcChecked");
            }
        }
    }

    public DependencyObject CurrentElement
    {
        get => _currentElement;
        set
        {
            if (_currentElement == value)
            {
                return;
            }
            _currentElement = value;
            FirePropertyChanged(nameof(CurrentElement));
        }
    }

    public bool ShowInputOutputDesignation
    {
        get => (bool)GetValue(ShowInputOutputDesignationProperty);
        set => SetValue(ShowInputOutputDesignationProperty, value);
    }

    public IIORegionManager InputOutputRegionManager
    {
        get => (IIORegionManager)GetValue(InputOutputRegionManagerProperty);
        set => SetValue(InputOutputRegionManagerProperty, value);
    }

    public global::Quire.Controls.Core.BookmarkTags.IBookmarksManager BookmarksManager
    {
        get => (global::Quire.Controls.Core.BookmarkTags.IBookmarksManager)GetValue(BookmarksManagerProperty);
        set => SetValue(BookmarksManagerProperty, value);
    }

    public bool ShowScriptableControlsWarningDialog
    {
        get => (bool)GetValue(ShowScriptableControlsWarningDialogProperty);
        set => SetValue(ShowScriptableControlsWarningDialogProperty, value);
    }

    public bool DisableScriptableControls
    {
        get => (bool)GetValue(DisableScriptableControlsProperty);
        set => SetValue(DisableScriptableControlsProperty, value);
    }

    public bool IsScriptingRunWorksheetOpen
    {
        get => (bool)GetValue(IsScriptingRunWorksheetOpenProperty);
        set => SetValue(IsScriptingRunWorksheetOpenProperty, value);
    }

    public bool IsScriptingRunWorksheetRecalculate
    {
        get => (bool)GetValue(IsScriptingRunWorksheetRecalculateProperty);
        set => SetValue(IsScriptingRunWorksheetRecalculateProperty, value);
    }

    public string CustomPathToPythonDir
    {
        get => (string)GetValue(CustomPathToPythonDirProperty);
        set => SetValue(CustomPathToPythonDirProperty, value);
    }

    public bool PreRecalculateAll
    {
        get => DocumentAttachedProperties.GetPreRecalculateAll(this);
        set
        {
            DebugTools.LogData("WorksheetControl.PreRecalculateAll: " + PreRecalculateAll + "=>" + value);
            DocumentAttachedProperties.SetPreRecalculateAll(this, value);
        }
    }

    public bool CacheWorksheet
    {
        get => DocumentAttachedProperties.GetCacheWorksheet(this);
        set => DocumentAttachedProperties.SetCacheWorksheet(this, value);
    }

    public global::Quire.Controls.CalculationOptions DocCalculationOptions
    {
        get => DocumentAttachedProperties.GetDocCalculationOptions(this);
        set => DocumentAttachedProperties.SetDocCalculationOptions(this, value);
    }

    public global::Quire.Controls.CalculationVariables DocCalculationVariables
    {
        get => DocumentAttachedProperties.GetDocCalculationVariables(this);
        set => DocumentAttachedProperties.SetDocCalculationVariables(this, value);
    }

    public UnitSystems DefaultUnitSystem
    {
        get => (UnitSystems)GetValue(DefaultUnitSystemProperty);
        set => SetValue(DefaultUnitSystemProperty, value);
    }

    public global::Quire.Controls.Core.NumberFormat DisplayNumberFormat
    {
        get => (global::Quire.Controls.Core.NumberFormat)GetValue(DisplayNumberFormatProperty);
        set => SetValue(DisplayNumberFormatProperty, value);
    }

    public global::Quire.Controls.Core.MatrixFormat ResultMatrixFormat
    {
        get => (global::Quire.Controls.Core.MatrixFormat)GetValue(ResultMatrixFormatProperty);
        set => SetValue(ResultMatrixFormatProperty, value);
    }

    public string FilePath
    {
        get => DocumentAttachedProperties.GetFilePath(this);
        set => DocumentAttachedProperties.SetFilePath(this, value);
    }

    public bool SimplifyUnitSystem
    {
        get => GetDispatchedProperty<bool>((global::Quire.DelegateFunction1<object, DependencyProperty>)GetValue, SimplifyUnitSystemProperty);
        set => SetValue(SimplifyUnitSystemProperty, value);
    }

    public UnitSystems CustomBaseUnitSystem
    {
        get => (UnitSystems)GetValue(CustomBaseUnitSystemProperty);
        set => SetValue(CustomBaseUnitSystemProperty, value);
    }

    public string WorksheetName
    {
        get => (string)GetValue(WorksheetNameProperty);
        set => SetValue(WorksheetNameProperty, value);
    }

    public global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont> LabeledIdFormat
    {
        get => (global::System.Collections.Generic.IDictionary<global::Quire.Controls.Core.IdLabels, global::Quire.Controls.Core.ExtendedFont>)GetValue(LabeledIdFormatProperty);
        set => SetValue(LabeledIdFormatProperty, value);
    }

    public TextFormat TextFormat
    {
        get => (TextFormat)GetValue(TextFormatProperty);
        set => SetValue(TextFormatProperty, value);
    }

    public NumberFormat CurrentNumberFormat
    {
        get => _currentNumberFormat;
        set
        {
            _currentNumberFormat = value;
            FirePropertyChanged(nameof(CurrentNumberFormat));
        }
    }

    public MatrixFormat CurrentMatrixFormat
    {
        get => _currentMatrixFormat;
        set
        {
            _currentMatrixFormat = value;
            FirePropertyChanged(nameof(CurrentMatrixFormat));
        }
    }


    public List<UnitDescriptor> CustomBaseUnitDescriptors { get; set; }

    public List<UnitDescriptor> CustomDerivedUnitDescriptors { get; set; }

    public ITextStylesManager TextStylesManager { get; set; }

    public SymbolicEngine SymbolicEngine => _model.SymbolicEngine;

    public string DisplayFilePath
    {
        get => _viewModel.DisplayFilePath;
        set => _viewModel.DisplayFilePath = value;
    }

    public bool OleObjectAutoResize
    {
        get => _viewModel.OleObjectAutoResize;
        set => _viewModel.OleObjectAutoResize = value;
    }

    public bool IsSavedWorksheet
    {
        set => _viewModel.IsSavedWorksheet = value;
    }

    public bool IsWorksheetContentScoped
    {
        get
        {
            if (IsContentActive)
            {
                return ActiveItem == null;
            }
            return false;
        }
    }

    public IEnumerable<IIORegionBase> IORegions => WorksheetItems.OfType<IIORegionBase>();

    public IIOItemCollection Inputs => InputOutputRegionManager.Inputs;

    public IIOItemCollection Outputs => InputOutputRegionManager.Outputs;

    public MathFormat MathFormat { get; private set; }

    public ExtendedFont Font => AppServiceRegistry.GetService<IFontManager>().GetFont(MathFormat);

    public StaticMathFormat InitialMathFormat => new(FontConstants.DefaultMathFormat);

    public MathFormat CurrentMathFormat
    {
        get => MathFormat;
        set => throw new NotImplementedException();
    }

    public bool ContainsSymbolicEvaluation => _model.ContainsSymbolicEvaluation;

    public PageLayoutMode WorksheetPageLayoutMode
    {
        get => _viewModel.WorksheetPageLayoutMode;
        set => _viewModel.WorksheetPageLayoutMode = value;
    }

    public bool EnableEditCurrentPageNumber
    {
        get => _viewModel.EnableEditCurrentPageNumber;
        set => _viewModel.EnableEditCurrentPageNumber = value;
    }

    public TextFormat ContentTextFormat
    {
        get => _viewModel.ContentTextFormat;
        set => _viewModel.ContentTextFormat = value;
    }

    public StaticMathFormat ContentMathFormat
    {
        set => _viewModel.ContentMathFormat = value;
    }

    public TextFormat HeaderTextFormat
    {
        get => _viewModel.HeaderTextFormat;
        set => _viewModel.HeaderTextFormat = value;
    }

    public TextFormat FooterTextFormat
    {
        get => _viewModel.FooterTextFormat;
        set => _viewModel.FooterTextFormat = value;
    }

    public TextFormat FirstPageHeaderTextFormat
    {
        get => _viewModel.FirstPageHeaderTextFormat;
        set => _viewModel.FirstPageHeaderTextFormat = value;
    }

    public TextFormat FirstPageFooterTextFormat
    {
        get => _viewModel.FirstPageFooterTextFormat;
        set => _viewModel.FirstPageFooterTextFormat = value;
    }

    public bool IsFirstPageHeaderFooterDifferent
    {
        get => _viewModel.IsFirstPageHeaderFooterDifferent;
        set => _viewModel.IsFirstPageHeaderFooterDifferent = value;
    }

    public bool DisplayGrid
    {
        get => _viewModel.DisplayGrid;
        set => _viewModel.DisplayGrid = value;
    }

    public bool IsSolveBlockLabelVisible
    {
        get => _viewModel.IsSolveBlockLabelVisible;
        set => _viewModel.IsSolveBlockLabelVisible = value;
    }

    public bool DisplayHFGrid
    {
        get => _viewModel.DisplayHFGrid;
        set => _viewModel.DisplayHFGrid = value;
    }

    public bool DisplayFrame
    {
        get => _viewModel.DisplayFrame;
        set => _viewModel.DisplayFrame = value;
    }

    public bool DisplayHeaderFrame
    {
        get => _viewModel.DisplayHeaderFrame;
        set => _viewModel.DisplayHeaderFrame = value;
    }

    public bool DisplayFooterFrame
    {
        get => _viewModel.DisplayFooterFrame;
        set => _viewModel.DisplayFooterFrame = value;
    }

    public bool UseMajorGridLines
    {
        get => _viewModel.UseMajorGridLines;
        set => _viewModel.UseMajorGridLines = value;
    }

    public bool UseHFMajorGridLines
    {
        get => _viewModel.UseHFMajorGridLines;
        set => _viewModel.UseHFMajorGridLines = value;
    }

    public bool HideFromCalculationButtonEnabled
    {
        get => _viewModel.HideFromCalculationButtonEnabled;
        set => _viewModel.HideFromCalculationButtonEnabled = value;
    }

    public bool HideFromCalculationButtonChecked
    {
        get => _viewModel.HideFromCalculationButtonChecked;
        set => _viewModel.HideFromCalculationButtonChecked = value;
    }

    public bool IsCalculationsEnabled
    {
        get => _viewModel.IsCalculationsEnabled;
        set => _viewModel.IsCalculationsEnabled = value;
    }

    public bool IsCalculating => _viewModel.IsCalculating;

    public System.Windows.Point InsertionPoint => _viewModel.InsertionPoint;

    public void Dispose()
    {
        Loaded -= DoActivate;
        _currentElement = null;
        UnhookFromViewModelEvents();
        if (_viewModel.PageLayout != null)
            _viewModel.PageLayout.PropertyChanged -= OnPageLayoutPropertyChanged;
        _viewModel.Dispose();
        Whiteboard.WhiteboardControl.DragSelectBeyondBorder -= OnDragSelectBeyondBorder;
        CurrentTextFormat = null;
        LabeledIdFormat = null;
    }

    public IWorksheetPageManager PageManager => _viewModel.PageManager;

    public bool HasScriptableControls => _viewModel.HasScriptableControls;

    public double WorksheetPageLayoutDisplayHeight => _viewModel.WorksheetPageLayoutDisplayHeight;

    public XmlLanguage CurrentProofingLanguage => _viewModel.CurrentProofingLanguage;

    internal IWorksheetPage ActivePage => _viewModel.ActivePage;

    public IEnumerable<IWorksheetPage> WorksheetPages => _viewModel.Pages;

    public IWorksheetSection ActiveSection => _viewModel.ActiveSection;

    public ObservableRangeCollection<IWhiteboardItem> ActiveSectionItems => _viewModel.ActiveSection.WhiteboardItems;

    public ObservableRangeCollection<IWhiteboardItem> WorksheetItems => _viewModel.WorksheetItems;

    public INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems => _viewModel.SelectedItems;

    public IWhiteboardItem ActiveItem => _viewModel.ActiveItem;

    public IWhiteboardItem ActiveDescendant => _viewModel.ActiveDescendant;

    public IList<IWhiteboardItem> SelectedDescendants => _viewModel.SelectedDescendants;

    public WorksheetPageLayout PageLayout => _viewModel.PageLayout;

    public bool IsContentActive => _viewModel.IsContentActive;

    public int CurrentPageNumber => _viewModel.CurrentPageNumber;

    public int PageCount => _viewModel.PageCount;

    public double NonprintableSpaceButtonHeight => _viewModel.NonprintableSpaceButtonHeight;

    public IHeaderFooterControl Header => _viewModel.Header;

    public IHeaderFooterControl Footer => _viewModel.Footer;

    public IHeaderFooterControl FirstPageHeader => _viewModel.FirstPageHeader;

    public IHeaderFooterControl FirstPageFooter => _viewModel.FirstPageFooter;

    public void WaitForCalculation()
    {
        _model.FireWaitForCalculationIdle();
    }

    public void WaitForCalculation(int seconds, System.Windows.Threading.DispatcherPriority priority)
    {
        _model.FireWaitForCalculationIdle(seconds, priority);
    }

    public void SetCurrentTextFormat()
    {
        if (_oldTextFormat != null)
        {
            if (_viewModel.ContentTextFormat.StyleKey == "Normal")
                _viewModel.SetStyleValues(_oldTextFormat, new TextStyle("Normal", _viewModel.ContentTextFormat, reset: true));
            CurrentTextFormat.UpdateTextFormat(_oldTextFormat);
            _oldTextFormat = null;
        }
    }

    private void ToggleOutputRegion(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (e.OriginalSource is IOutputRegion outputRegion)
        {
            IEnumerable<IIORegion> outputRegions = new IIORegion[1] { outputRegion };
            _model.InputOutputRegionManager.ToggleOutputRegions(outputRegions);
        }
    }

    private void ToggleInputRegion(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        if (e.OriginalSource is IInputRegion inputRegion)
        {
            IEnumerable<IIORegion> inputRegions = new IIORegion[1] { inputRegion };
            _model.InputOutputRegionManager.ToggleInputRegions(inputRegions);
        }
    }

    private void CanImportData(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _viewModel.CanImportData();
    }

    private void ImportData(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            Title = Resources["DataImport.FileOpenDialog.Title"] as string,
            Filter = Resources["DataImport.FileOpenDialog.FileFilter"] as string,
            FilterIndex = 0
        };
        if (dialog.ShowDialog() == true)
            _viewModel.ImportData(dialog.FileName);
    }

    public void Activate()
    {
        if (!IsLoaded)
        {
            Loaded -= DoActivate;
            Loaded += DoActivate;
        }
        else
        {
            DoActivate();
        }
    }

    public void ActivateHeader()
    {
        _viewModel.ActivateHeader();
    }

    public void ActivateFooter()
    {
        _viewModel.ActivateFooter();
    }

    public void ActivateContent()
    {
        _viewModel.ActivateContent();
    }

    public void MoveItem(IWhiteboardItem item, Point point)
    {
        _viewModel.MoveItem(item, point);
    }

    public bool CanToggleInputRegion()
    {
        return _viewModel.CanToggleRegionsAsInput();
    }

    public bool CanToggleOutputRegion()
    {
        return _viewModel.CanToggleRegionsAsOutput();
    }

    public void ToggleInputRegion()
    {
        _viewModel.ToggleInputRegions();
    }

    public void ToggleOutputRegion()
    {
        _viewModel.ToggleOutputRegions();
    }

    public void RefreshDictionary()
    {
        if (SpellCheckEnabled)
        {
            _viewModel.RefreshDictionary();
        }
    }

    public void UpdateWorksheetLayoutOnScrollChanged(double verticalOffset, double viewportHeight)
    {
        double paddingTopOffset = PageManager.UpdateVisiblePagesOnScrollChanged(verticalOffset, viewportHeight, _viewModel.WorksheetPageLayoutDisplayHeight);
        UpdateVisiblePagesControlPaddingTop(paddingTopOffset);
    }

    private void UpdateVisiblePagesControlPaddingTop(double paddingTopOffset)
    {
        if (VisiblePagesControl.Padding.Top != paddingTopOffset)
        {
            VisiblePagesControl.Padding = new Thickness(0.0, paddingTopOffset, 0.0, 0.0);
        }
    }

    private static void HandleShowInputOutputDesignation(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WorksheetControl)d).FireChanged();
    }

    public void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        FirePropertyChanged(e.Property.Name);
        ContentMathFormat = ((MathFormat != null) ? new StaticMathFormat(MathFormat) : null);
        FireChanged();
    }
}
