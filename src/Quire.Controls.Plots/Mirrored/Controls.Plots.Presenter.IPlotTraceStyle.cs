using System.Windows.Media;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Traces;

namespace Quire.Controls.Plots.Presenter;

public partial interface IPlotTraceStyle : ISymbolProperties, IHasNumberOfPoints
{
    Color Color { get; set; }

    TraceSymbol Symbol { get; set; }

    LineStyle LineStyle { get; set; }

    bool ChangeSymbol(TraceSymbol value);

    bool ChangeLineStyle(LineStyle value);

    TraceStyleState? DoStyleChangeCommand();

    void UndoStyleChangeCommand(TraceStyleState? doData);
}
