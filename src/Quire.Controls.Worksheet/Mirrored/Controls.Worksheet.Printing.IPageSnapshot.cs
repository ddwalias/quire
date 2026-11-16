namespace Quire.Controls.Worksheet.Printing;

public partial interface IPageSnapshot
{
    IWorksheetPageBody PageBody { get; }

    IHeaderFooterControl Header { get; set; }

    IHeaderFooterControl Footer { get; set; }
}
