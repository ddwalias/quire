#nullable disable
#pragma warning disable

namespace Quire.Controls.Plots;

internal partial class SecondTicktLabelVisibilityMultiConverter
{
    public new static readonly SecondTicktLabelVisibilityMultiConverter Instance = new SecondTicktLabelVisibilityMultiConverter();

    protected override bool Convert(object[] values)
    {
        Presenter.AxisScaling axisScaling = (Presenter.AxisScaling)values[3];
        int num = (int)values[4];
        if (base.Convert(values) && axisScaling == Presenter.AxisScaling.Linear)
        {
            return num > 2;
        }
        return false;
    }

    private SecondTicktLabelVisibilityMultiConverter()
    {
    }
}
