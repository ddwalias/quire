#nullable disable
#pragma warning disable

using System.Windows.Media;

namespace Quire.Controls.Plots.Presenter;

public partial class TraceStyleSelector
{
    private int _currentColorIndex;

    public void ApplyNextStyle(ITraceModel trace)
    {
        trace.Color = ((SolidColorBrush)TraceColorTable.Default[_currentColorIndex]).Color;
        trace.Symbol = TraceSymbol.none;
        trace.LineStyle = LineStyle.Solid;
        trace.LineWeight = 1.0;
        trace.SymbolFrequency = 1.0;
        trace.SymbolWeight = 1.5;
        _currentColorIndex++;
        if (_currentColorIndex >= TraceColorTable.Default.Count)
        {
            _currentColorIndex = 0;
        }
    }

    public TraceStyleSelector()
    {
    }
}
