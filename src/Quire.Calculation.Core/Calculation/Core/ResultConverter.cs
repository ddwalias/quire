#nullable disable
using System;
using System.Collections.Generic;

namespace Quire.Calculation.Core;

public partial class ResultConverter
{
    private readonly IResultNodeFactory _newResultNodeFactory;

    public ResultConverter(IResultNodeFactory newResultNodeFactory)
    {
        _newResultNodeFactory = newResultNodeFactory;
    }

    public IResultNode Convert(IResultNode resultNode)
    {
        if (resultNode is IEmptyResultNode)
        {
            return _newResultNodeFactory.CreateEmptyResultNode();
        }
        if (resultNode is IRealResultNode realResultNode)
        {
            return _newResultNodeFactory.CreateRealResultNode(realResultNode.Value);
        }
        if (resultNode is IImaginaryResultNode imaginaryResultNode)
        {
            return _newResultNodeFactory.CreateImaginaryResultNode(imaginaryResultNode.Value);
        }
        if (resultNode is IComplexResultNode complexResultNode)
        {
            return _newResultNodeFactory.CreateComplexResultNode(complexResultNode.RValue, complexResultNode.IValue);
        }
        if (resultNode is IStringResultNode stringResultNode)
        {
            return _newResultNodeFactory.CreateStringResultNode(stringResultNode.Value);
        }
        if (resultNode is IFunctionResultNode functionResultNode)
        {
            return _newResultNodeFactory.CreateFunctionResultNode(functionResultNode.Description);
        }
        if (resultNode is IUnitedValueResultNode unitedValueResultNode)
        {
            IUnitedValueResultNode newUnitedNode = _newResultNodeFactory.CreateUnitedValueResultNode();
            newUnitedNode.Value = Convert(unitedValueResultNode.Value);
            unitedValueResultNode.Units.Each(delegate(Unit unit)
            {
                newUnitedNode.Units.Add(unit);
            });
            return newUnitedNode;
        }
        if (resultNode is IDisplayMatrixResultNode displayMatrixResultNode)
        {
            IDisplayMatrixResultNode displayMatrixResultNode2 = _newResultNodeFactory.CreateDisplayMatrixResultNode(displayMatrixResultNode.Rows, displayMatrixResultNode.Columns, displayMatrixResultNode.ViewportOffset, displayMatrixResultNode.Viewport.Size);
            for (int num = 0; num < displayMatrixResultNode.Viewport.Rows; num++)
            {
                for (int num2 = 0; num2 < displayMatrixResultNode.Viewport.Columns; num2++)
                {
                    displayMatrixResultNode2.Viewport.SetElementAt(num, num2, Convert(displayMatrixResultNode.Viewport[num, num2]));
                }
            }
            return displayMatrixResultNode2;
        }
        if (resultNode is IEagerMatrixResultNode eagerMatrixResultNode)
        {
            IEagerMatrixResultNode eagerMatrixResultNode2 = _newResultNodeFactory.CreateEagerMatrixResultNode(eagerMatrixResultNode.Rows, eagerMatrixResultNode.Columns);
            for (int num3 = 0; num3 < eagerMatrixResultNode.Rows; num3++)
            {
                for (int num4 = 0; num4 < eagerMatrixResultNode.Columns; num4++)
                {
                    eagerMatrixResultNode2.SetElementAt(num3, num4, Convert(eagerMatrixResultNode[num3, num4]));
                }
            }
            return eagerMatrixResultNode2;
        }
        ILazyMatrixResultNode lazyMatrixNode = resultNode as ILazyMatrixResultNode;
        if (lazyMatrixNode != null)
        {
            Func<IValueProxyWrapper, IResultNode> transformValueToResult = (IValueProxyWrapper proxy) => Convert(lazyMatrixNode.TransformValueToResult(proxy));
            return _newResultNodeFactory.CreateLazyMatrixResultNode(lazyMatrixNode.MatrixProxy, transformValueToResult);
        }
        if (resultNode is ISymbolicResultNode symbolicResultNode)
        {
            return _newResultNodeFactory.CreateSymbolicResultNode(symbolicResultNode.Xml);
        }
        throw new ArgumentException("logic error: Cannot convert the given result node", "resultNode");
    }

    public static object ConvertFromIResultNode(SimpleResultNode value)
    {
        if (value == null)
        {
            return null;
        }
        if (value is RealSimpleResultNode realResultNode)
        {
            return (double)realResultNode.Value;
        }
        if (value is StringSimpleResultNode stringSimpleResultNode)
        {
            return stringSimpleResultNode.Value;
        }
        if (value is EagerMatrixSimpleResultNode eagerMatrixSimpleResultNode)
        {
            int rows = eagerMatrixSimpleResultNode.Rows;
            int columns = eagerMatrixSimpleResultNode.Columns;
            object[,] array = new object[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = ConvertFromIResultNode(eagerMatrixSimpleResultNode[i, j]);
                }
            }
            return array;
        }
        return null;
    }
}
