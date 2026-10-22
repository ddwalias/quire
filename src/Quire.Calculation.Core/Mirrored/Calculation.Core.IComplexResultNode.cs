#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IComplexResultNode
{
    double RValue { get; set; }
    double IValue { get; set; }
}
