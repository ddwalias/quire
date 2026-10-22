#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core.Support;

partial interface ISimpleCalculatable
{
    global::Quire.Calculation.Core.RealSimpleResultNode SystemOrigin { get; }
    bool IsErrorOnScalingUnits();
}
