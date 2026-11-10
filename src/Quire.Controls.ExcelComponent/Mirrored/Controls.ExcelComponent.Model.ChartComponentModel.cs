using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ChartComponentModel
{
    internal void CommitPost()
    {
        _chartMinMaxCalc.Commit();
    }

    private void HandleChildWrapperChartTypeChanged(IChartComponentCalculatableWrapper wrapper)
    {
        HandleChildWrapperRemovedOrChartTypeChanged(wrapper, null);
    }
}
