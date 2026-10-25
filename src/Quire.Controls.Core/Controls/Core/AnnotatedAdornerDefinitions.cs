#nullable disable
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Core;

internal static partial class AnnotatedAdornerDefinitions
{
    public static readonly Geometry MarginLine;

    public static readonly Geometry ArrowBase;

    public static readonly Geometry ArrowHeadTop;

    public static readonly Geometry ArrowHeadBottom;

    public static readonly Brush AdornerBrush;

    public static readonly Pen AdornerPen;

    static AnnotatedAdornerDefinitions()
    {
        MarginLine = Geometry.Parse("M-10,0 L-45,0");
        ArrowBase = Geometry.Parse("M2,0 L25,0");
        ArrowHeadTop = Geometry.Parse("M2,0 L10,-5");
        ArrowHeadBottom = Geometry.Parse("M2,0 L10,5");
        AdornerBrush = Brushes.Red;
        AdornerPen = new Pen(AdornerBrush, 1.5);
        AdornerPen.Freeze();
    }
}
