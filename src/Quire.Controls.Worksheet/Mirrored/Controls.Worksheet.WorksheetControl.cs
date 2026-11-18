using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Core.IORegions;
using Quire.Calculation.Core;
using Quire.Core;
using Quire.Wpf;
using Microsoft.Win32;

namespace Quire.Controls.Worksheet;

public partial class WorksheetControl
{
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
