#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IDisplayMatrixResultNode
{
    global::Quire.TableCoordinate ViewportOffset { get; }
    global::Quire.Calculation.Core.IEagerMatrixResultNode Viewport { get; }
}
