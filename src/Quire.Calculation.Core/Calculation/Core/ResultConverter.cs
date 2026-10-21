#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;

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
        ILazyMatrixResultNode lazyMatrixNode;
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
            unitedValueResultNode.Units.Each((Unit unit) =>
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
        lazyMatrixNode = resultNode as ILazyMatrixResultNode;
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

    public static SimpleResultNode ConvertToIResultNode(object value, bool isFlatMatrix = false)
    {
        if (value == null)
        {
            return new EmptySimpleResultNode();
        }
        if (value is object[] array && array.Length == 1 && !isFlatMatrix)
        {
            value = array[0];
        }
        if (value is double)
        {
            return new RealSimpleResultNode
            {
                Value = (double)value
            };
        }
        if (value is short)
        {
            return new RealSimpleResultNode
            {
                Value = (short)value
            };
        }
        if (value is int)
        {
            return new RealSimpleResultNode
            {
                Value = (int)value
            };
        }
        if (value is string)
        {
            return new StringSimpleResultNode
            {
                Value = (string)value
            };
        }
        object[] array2 = value as object[];
        if ((array2 != null) & isFlatMatrix)
        {
            int length = array2.GetLength(0);
            EagerMatrixSimpleResultNode eagerMatrixSimpleResultNode = new EagerMatrixSimpleResultNode(1, length);
            for (int i = 0; i < length; i++)
            {
                eagerMatrixSimpleResultNode.SetElementAt(0, i, ConvertToIResultNode(array2[i], isFlatMatrix));
            }
            return eagerMatrixSimpleResultNode;
        }
        if (value is List<object> list)
        {
            value = list.ToArray();
        }
        if (value is object[] array3 && array3.Length != 0)
        {
            int colCount = 1;
            int length2 = array3.GetLength(0);
            EagerMatrixSimpleResultNode eagerMatrixSimpleResultNode2 = new EagerMatrixSimpleResultNode(length2, colCount);
            for (int j = 0; j < length2; j++)
            {
                eagerMatrixSimpleResultNode2.SetElementAt(j, 0, ConvertToIResultNode(array3[j]));
            }
            return eagerMatrixSimpleResultNode2;
        }
        if (value is object[,] array4)
        {
            int length3 = array4.GetLength(0);
            int length4 = array4.GetLength(1);
            EagerMatrixSimpleResultNode eagerMatrixSimpleResultNode3 = new EagerMatrixSimpleResultNode(length3, length4);
            for (int k = 0; k < length3; k++)
            {
                for (int l = 0; l < length4; l++)
                {
                    eagerMatrixSimpleResultNode3.SetElementAt(k, l, ConvertToIResultNode(array4[k, l]));
                }
            }
            return eagerMatrixSimpleResultNode3;
        }
        return new StringSimpleResultNode
        {
            Value = string.Empty
        };
    }

    public static object ConvertFromIResultNode(SimpleResultNode value)
    {
        if (value == null)
        {
            return null;
        }
        if (value is RealSimpleResultNode realSimpleResultNode)
        {
            return realSimpleResultNode.Value;
        }
        if (value is StringSimpleResultNode stringSimpleResultNode)
        {
            return stringSimpleResultNode.Value;
        }
        if (value is EagerMatrixSimpleResultNode { Rows: var rows, Columns: var columns } eagerMatrixSimpleResultNode)
        {
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
