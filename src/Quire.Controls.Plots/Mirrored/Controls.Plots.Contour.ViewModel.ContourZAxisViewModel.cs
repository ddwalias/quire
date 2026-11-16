using Quire.Controls.Plots.Base3D.ViewModel;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Contour.ViewModel;

public partial class ContourZAxisViewModel : Base3DAxisViewModel
{
    internal ContourZAxisViewModel()
        : base(AxesType.Z, PlotType.Contour)
    {
        PlotAxisModel.UseBetterTickMarks = false;
    }

    internal override void SetResultRange(double? resultStart, double? resultEnd)
    {
        ResultStart = resultStart;
        ResultEnd = resultEnd;
    }

    protected override void LimitsExpressionValueChanged()
    {
        bool flag = PlotAxisModel.StartExpression.IsValueValid || PlotAxisModel.EndExpression.IsValueValid || PlotAxisModel.SecondTickExpression.IsValueValid;
        if (flag)
        {
            PlotAxisModel.SetRange(PlotAxisModel.Start, PlotAxisModel.End);
        }
        else
        {
            base.LimitsExpressionValueChanged();
        }
    }
}
