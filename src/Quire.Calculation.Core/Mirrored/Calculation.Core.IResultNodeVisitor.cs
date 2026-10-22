#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IResultNodeVisitor
{
    void Visit(global::Quire.Calculation.Core.IEmptyResultNode empty);
    void Visit(global::Quire.Calculation.Core.IRealResultNode real);
    void Visit(global::Quire.Calculation.Core.IImaginaryResultNode imag);
    void Visit(global::Quire.Calculation.Core.IComplexResultNode complex);
    void Visit(global::Quire.Calculation.Core.IStringResultNode str);
    void Visit(global::Quire.Calculation.Core.IFunctionResultNode str);
    void Visit(global::Quire.Calculation.Core.IUnitedValueResultNode unitedValue);
    void Visit(global::Quire.Calculation.Core.IDisplayMatrixResultNode displayMatrix);
    void Visit(global::Quire.Calculation.Core.IEagerMatrixResultNode eagerMatrix);
    void Visit(global::Quire.Calculation.Core.ILazyMatrixResultNode lazyMatrix);
    void Visit(global::Quire.Calculation.Core.ISymbolicResultNode symbolic);
    void Visit(global::Quire.Calculation.Core.IVectorListResultNode vectorList);
    void Visit(global::Quire.Calculation.Core.ITrace2DResultNode traceResult);
}
