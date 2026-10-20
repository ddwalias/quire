#nullable disable
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class CalculatableAggregateExtensions
{
    [MirrorVerified("PtcCalculationCoreCalculatableAggregateExtensionsMechanicalDifferentialTests")]
    public static IList<string> GetAggregateRl(this ICalculatableAggregate aggregate)
    {
        var rlList = new List<string>(1 + aggregate.CalculatableChildren.Count());
        rlList.Add(aggregate.Rl);
        foreach (ICalculatable calculatable in aggregate.CalculatableChildren)
        {
            rlList.Add(calculatable.Rl);
        }

        return rlList;
    }
}
