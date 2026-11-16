using System.Windows.Media;

namespace Quire.Controls.Plots.Model;

public partial interface IBaseMarker
{
    Color Color { get; set; }
    bool ShowCallout { get; set; }
    IMarkerCalculatable Calculatable { get; }
}
