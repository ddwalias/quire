#nullable disable
#pragma warning disable

using System.Windows.Media;

namespace Quire.Controls.Plots.Contour.View;

public partial struct ColorSample
{
    public Brush Color { readonly get; internal set; }

    public double Width { readonly get; internal set; }
}
