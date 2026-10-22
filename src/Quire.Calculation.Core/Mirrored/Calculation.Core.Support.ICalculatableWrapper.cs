#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core.Support;

partial interface ICalculatableWrapper
{
    global::Quire.Calculation.Core.ICalculatable WrappedCalculatable { get; }
}
