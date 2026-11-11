using System;
using Quire.Calculation.Core;

namespace Quire.Native.PtcEquationEditor;

internal partial class ae
{
    public IEmptyResultNode CreateEmptyResultNode()
    {
        return new u();
    }

    public IRealResultNode CreateRealResultNode()
    {
        return new w();
    }

    public IRealResultNode CreateRealResultNode(double rvalue)
    {
        w node = new w();
        node.Value = rvalue;
        return node;
    }

    public IImaginaryResultNode CreateImaginaryResultNode()
    {
        return new x();
    }

    public IImaginaryResultNode CreateImaginaryResultNode(double ivalue)
    {
        x node = new x();
        node.Value = ivalue;
        return node;
    }

    public IComplexResultNode CreateComplexResultNode()
    {
        return new y();
    }

    public IComplexResultNode CreateComplexResultNode(double rvalue, double ivalue)
    {
        y node = new y();
        node.RValue = rvalue;
        node.IValue = ivalue;
        return node;
    }

    public IStringResultNode a()
    {
        return new z();
    }

    public IStringResultNode CreateStringResultNode(string value)
    {
        z node = new z();
        node.Value = value;
        return node;
    }

    public IFunctionResultNode CreateFunctionResultNode(string descriptionOfFunction)
    {
        aa node = new aa();
        node.Description = descriptionOfFunction;
        return node;
    }

    public IDisplayMatrixResultNode CreateDisplayMatrixResultNode(int rows, int cols, TableCoordinate viewportOffset, TableSize viewportSize)
    {
        return new ab(rows, cols, viewportOffset, viewportSize);
    }

    public IEagerMatrixResultNode CreateEagerMatrixResultNode(int rows, int cols)
    {
        return new ac(rows, cols);
    }

    public ILazyMatrixResultNode CreateLazyMatrixResultNode(IMatrixValueProxyWrapper matrix, Func<IValueProxyWrapper, IResultNode> transformValueToResult)
    {
        return new ad(matrix, transformValueToResult);
    }

    public IUnitedValueResultNode CreateUnitedValueResultNode()
    {
        return new v();
    }

    public ISymbolicResultNode CreateSymbolicResultNode(string xmcd)
    {
        throw new NotImplementedException();
    }
}
