using System;
using System.Windows;
using System.Windows.Documents;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet.Printing;

public partial class EngineeringPages
{
    public WorksheetPageLayout PageLayout
    {
        get => _pageLayout;
        set
        {
            _pageLayout = value;
            _pageSizeDips = new Size(PageLayout.PageWidth, PageLayout.PageHeight);
        }
    }

    public Size PageSizeDips => _pageSizeDips;

    public bool DisplayGrid { get; set; }

    public bool IsSolveBlockLabelVisible { get; set; }

    public bool DisplayHFGrid { get; set; }

    public bool UseMajorGridLines { get; set; }

    public bool UseHFMajorGridLines { get; set; }

    public bool DisplayFrame { get; set; }

    public bool DisplayHeaderFrame { get; set; }

    public bool DisplayFooterFrame { get; set; }

    public int TotalPages { get; set; }

    public int StartingPageNumber { get; set; }

    public int TotalPagesOffset { get; set; }

    public bool IsFirstPageHeaderFooterDifferent { get; set; }

    public IHeaderFooterControl FirstPageHeader { get; set; }

    public IHeaderFooterControl FirstPageFooter { get; set; }

    public DateTime? DisplaySavedDate { get; set; }

    public string DisplayFileName { get; set; }

    public string DisplayFilePath { get; set; }

    public string DisplayDocumentName { get; set; }

    public long MaxUpdateLayoutTimeMsec { get; set; }

    public PageLayoutMode WorksheetPageLayoutMode => PageLayoutMode.PageMode;

    public int PageCount => Items.Count;

    public void AddPage(IPageSnapshot page)
    {
        Items.Add(page);
    }

    public DocumentPaginator DocumentPaginator => _engPagesPaginator;

    private partial class EngPagesPaginator
    {
        public EngPagesPaginator(EngineeringPages engineeringPages)
        {
            _engineeringPages = engineeringPages;
        }

        public override bool IsPageCountValid => true;

        public override int PageCount => _engineeringPages.PageCount;

        public override Size PageSize
        {
            get => _engineeringPages.PageSizeDips;
            set => throw new NotSupportedException();
        }

        public override IDocumentPaginatorSource Source => _engineeringPages;
    }
}
