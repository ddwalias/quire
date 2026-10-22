#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IEagerMatrixResultNode
{
    void SetElementAt(int r, int c, global::Quire.Calculation.Core.IResultNode element);
}
