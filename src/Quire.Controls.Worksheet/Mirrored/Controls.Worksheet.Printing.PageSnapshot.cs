using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Worksheet.Printing;

public partial class PageSnapshot
{
    public PageSnapshot()
    {
    }

    public PageSnapshot(Size? gridSize)
        : base(gridSize, isPrinting: true)
    {
    }

    public new IHeaderFooterControl Header
    {
        get => (IHeaderFooterControl)base.Header;
        set => base.Header = value;
    }

    public new IHeaderFooterControl Footer
    {
        get => (IHeaderFooterControl)base.Footer;
        set => base.Footer = value;
    }

    public new void Dispose()
    {
        Header.Dispose();
        Footer.Dispose();
        PageBody.Dispose();
    }

    public Visual Visual => this;
}
