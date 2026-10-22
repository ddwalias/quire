#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IResultNodeFactory
{
    global::Quire.Calculation.Core.IEmptyResultNode CreateEmptyResultNode();
    global::Quire.Calculation.Core.IRealResultNode CreateRealResultNode();
    global::Quire.Calculation.Core.IRealResultNode CreateRealResultNode(double rvalue);
    global::Quire.Calculation.Core.IImaginaryResultNode CreateImaginaryResultNode();
    global::Quire.Calculation.Core.IImaginaryResultNode CreateImaginaryResultNode(double ivalue);
    global::Quire.Calculation.Core.IComplexResultNode CreateComplexResultNode();
    global::Quire.Calculation.Core.IComplexResultNode CreateComplexResultNode(double rvalue, double ivalue);
    global::Quire.Calculation.Core.IStringResultNode CreateStringResultNode(string value);
    global::Quire.Calculation.Core.IFunctionResultNode CreateFunctionResultNode(string descriptionOfFunction);
    global::Quire.Calculation.Core.IUnitedValueResultNode CreateUnitedValueResultNode();
    global::Quire.Calculation.Core.IDisplayMatrixResultNode CreateDisplayMatrixResultNode(int rows, int cols, global::Quire.TableCoordinate viewportOffset, global::Quire.TableSize viewportSize);
    global::Quire.Calculation.Core.IEagerMatrixResultNode CreateEagerMatrixResultNode(int rows, int cols);
    global::Quire.Calculation.Core.ILazyMatrixResultNode CreateLazyMatrixResultNode(global::Quire.Calculation.Core.IMatrixValueProxyWrapper matrix, global::System.Func<global::Quire.Calculation.Core.IValueProxyWrapper, global::Quire.Calculation.Core.IResultNode> transformValueToResult);
    global::Quire.Calculation.Core.ISymbolicResultNode CreateSymbolicResultNode(string xmcd);
}
