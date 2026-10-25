#nullable disable
#pragma warning disable
using System.Collections.Generic;

namespace Quire.Calculation.Core;

partial interface IUnitedValueResultNode
{
    IResultNode Value { get; set; }
    IList<Unit> Units { get; }
    void AddUnit(string canonicalName, int powerNumerator, int powerDenominator);
}
