using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Wpf;

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
}
