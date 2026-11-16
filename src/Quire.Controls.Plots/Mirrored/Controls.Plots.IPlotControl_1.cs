namespace Quire.Controls.Plots;

public partial interface IPlotControl<TEquation>
{
    double Width { get; set; }
    double Height { get; set; }
    void AddEquations(TEquation xyEquation);
}
