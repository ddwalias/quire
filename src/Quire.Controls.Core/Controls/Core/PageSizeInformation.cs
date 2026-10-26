#nullable disable
using System.Collections.Generic;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial struct PageSizeInformation
{
    public Size Size;
    public Dictionary<PageMarginType, Thickness> PageMargins;
    public Thickness HeaderMargin;
    public Thickness FooterMargin;
    public Size GridCellSize;
    public Size FineGridCellSize;
    public Size HeaderFooterGridCellSize;
    public double MajorGridLines;
    public Point InitialInsertionPoint;

    public PageSizeInformation(Size size, Dictionary<PageMarginType, Thickness> pageMargins, Thickness headerMargin, Thickness footerMargin, Size gridCellSize, Size fineGridCellSize, Size headerFooterGridCellSize, double majorGridLines, Point initialInsertionPoint)
    {
        Size = size;
        HeaderMargin = headerMargin;
        FooterMargin = footerMargin;
        GridCellSize = gridCellSize;
        FineGridCellSize = fineGridCellSize;
        HeaderFooterGridCellSize = headerFooterGridCellSize;
        MajorGridLines = majorGridLines;
        InitialInsertionPoint = initialInsertionPoint;
        PageMargins = pageMargins;
    }
}
