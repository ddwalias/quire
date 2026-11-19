using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quire.Controls;

public partial class ScrollViewerGrid
{
    private readonly DocumentScrollViewer _scrollViewer;
    private string _scrollViewerToolTipHeader;
    private readonly global::System.Windows.Shapes.Rectangle _clippingRectangle;

    public ScrollViewerGrid(EngineeringDocument engineeringDocument)
    {
        _scrollViewer = new DocumentScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Focusable = false
        };
        _clippingRectangle = new global::System.Windows.Shapes.Rectangle
        {
            Name = "ClippingRectangleControl",
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Stretch,
            Fill = TryFindResource("WorksheetBackgroundBrush") as LinearGradientBrush,
            IsHitTestVisible = false
        };
        UpdateEngineeringDocument(engineeringDocument);
        Children.Add(_scrollViewer);
        Children.Add(_clippingRectangle);
        _scrollViewer.ScrollChanged += OnScrollChanged;
        _scrollViewer.Loaded += SetTooltipAndMinScrollbarThumbSize;
    }

    public EngineeringDocument EngineeringDocumentInstance { get; private set; }

    private string ScrollViewerToolTipHeader
    {
        get => _scrollViewerToolTipHeader ??= EngineeringDocumentInstance.Resources[ScrollViewerToolTipHeaderKey] as string;
    }

    private global::Quire.Controls.Worksheet.IWorksheetControl Worksheet => EngineeringDocumentInstance.Worksheet;
}
