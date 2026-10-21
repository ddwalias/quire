#nullable disable
using System.Collections.Generic;
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class DoubleListResultNode
{
    private const double _neutralValue = double.NaN;

    public DoubleListResultNode(double paddingValue, int length)
        : base((IList<double>)new List<double>(length), paddingValue.IsValid() ? length : 0)
    {
        for (int i = 0; i < length; i++)
        {
            List.Add(paddingValue);
        }
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public DoubleListResultNode(IList<double> doubleValues) : base(doubleValues, -1) { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public DoubleListResultNode(IList<double> doubleValues, double paddingValue, int minLength) : base(doubleValues, paddingValue, minLength) { }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public override bool IsMeaningfulValue(double value) => DoubleExtension.IsValid(value);

    [MirrorVerified("PtcCalculationCoreTrace2DResultsDoubleListResultNodeMechanicalDifferentialTests")]
    public override double NeutralValue => double.NaN;
}
