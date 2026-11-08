#nullable disable
#pragma warning disable
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.ViewModel;

internal partial class OleComponentResizeData : IResizeData
{
    public double InputSectionHeight { get; private set; }

    public double OutputSectionHeight { get; private set; }

    public Size OldSize { get; private set; }

    public Size OldMinSize { get; private set; }

    public double OleContainerItemTop { get; private set; }

    public Size OleContainerItemSize { get; private set; }

    public Dictionary<IWhiteboardItem, Point> LocationMap { get; private set; }

    public OleComponentResizeData(Size oldSize, Size oldMinSize, double oleContainerItemTop, double oleContainerItemWidth, double oleContainerItemHeight, double inputHeight, double outputHeight, Dictionary<IWhiteboardItem, Point> locationMap)
    {
        OldSize = oldSize;
        OldMinSize = oldMinSize;
        OleContainerItemTop = oleContainerItemTop;
        OleContainerItemSize = new Size(oleContainerItemWidth, oleContainerItemHeight);
        LocationMap = locationMap;
        InputSectionHeight = inputHeight;
        OutputSectionHeight = outputHeight;
    }
}
