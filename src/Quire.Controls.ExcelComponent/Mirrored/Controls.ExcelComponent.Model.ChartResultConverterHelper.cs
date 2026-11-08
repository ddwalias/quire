#nullable disable
#pragma warning disable
using Quire.Calculation.Core.Trace2DResults;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ChartResultConverterHelper : Trace2DResultConverterHelperBase
{
    public ChartResultConverterHelper(ChartContainerModel resultsOwner)
        : base(resultsOwner)
    {
    }

    public override int MaxNumberOf2DPoints => ControlSettings.Instance.MaxNumberOf2dPoints;

    public override bool DiscreteValuesAllowed => false;

    public override bool RangeUnitsAllowed => true;
}
