using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Plot3D.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.Traces;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

public partial interface IPlotTraceViewModel3D : INotifyPropertyChanged, IPlotTraceStyle, ISymbolProperties, IHasNumberOfPoints
{
    PlotTraceVisualData3D VisualData { get; }

    Rect3D CommonRange { get; }

    ICalculatableInfo CalculatableInfo { get; }

    Color RenderColor { get; }

    bool IsStyleAutomatic { get; }

    Color? SurfaceFill { get; set; }

    TraceType3D VisualDataTypeOrDefault { get; }

    bool IsInPrinting { get; set; }

    bool IsOwnModel(IPlotTraceModel3D model);

    void UserChangedStyle();
}
