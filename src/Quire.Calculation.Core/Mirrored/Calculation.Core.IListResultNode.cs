#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IListResultNode
{
    int Count { get; }
    int MeaningfulValuesCount { get; }
}
