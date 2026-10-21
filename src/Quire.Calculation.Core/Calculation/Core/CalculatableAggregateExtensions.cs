#nullable disable
using System.Collections.Generic;
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public static partial class CalculatableAggregateExtensions
{
    public static IList<string> GetAggregateRl(this ICalculatableAggregate aggregate)
    {
        IList<string> rlList = new List<string>(1 + aggregate.CalculatableChildren.Count());
        rlList.Add(aggregate.Rl);
        aggregate.CalculatableChildren.Each((ICalculatable calc) =>
        {
            rlList.Add(calc.Rl);
        });
        return rlList;
    }
}
