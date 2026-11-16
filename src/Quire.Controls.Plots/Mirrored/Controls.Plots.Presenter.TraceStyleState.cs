namespace Quire.Controls.Plots.Presenter;

public partial struct TraceStyleState
{
    public TraceSymbol TraceSymbol;
    public LineStyle LineStyle;
    public double LineWeight;

    public TraceStyleState(TraceSymbol symbol, LineStyle line, double weight)
    {
        TraceSymbol = symbol;
        LineStyle = line;
        LineWeight = weight;
    }
}
