#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface ILazyMatrixResultNode
{
    global::Quire.Calculation.Core.IMatrixValueProxyWrapper MatrixProxy { get; }
    global::System.Func<global::Quire.Calculation.Core.IValueProxyWrapper, global::Quire.Calculation.Core.IResultNode> TransformValueToResult { get; }
}
