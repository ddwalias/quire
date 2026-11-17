#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Contour.Model;

internal partial class ContourCalculationResult
{
    private IMatrixResultNode _matrixResultNode;

    private IMatrixResultNode MatrixResultNode
    {
        get
        {
            return _matrixResultNode;
        }
        set
        {
            _matrixResultNode = value;
            GetZAndUnits(out var z, out var units);
            ZNode = z;
            Units = units;
        }
    }

    public IMatrixResultNode ZNode { get; private set; }

    public IList<Unit> Units { get; private set; }

    public bool IsEmpty => MatrixResultNode == null;

    internal int Contours => MatrixResultNode.Count - 3;

    internal IMatrixResultNode Bounds => MatrixResultNode[MatrixResultNode.Rows - 1, 0] as IMatrixResultNode;

    internal IMatrixResultNode GradientSigns => MatrixResultNode[MatrixResultNode.Rows - 2, 0] as IMatrixResultNode;

    internal ContourCalculationResult(IMatrixResultNode lazyMatrixSimpleResultNode)
    {
        MatrixResultNode = lazyMatrixSimpleResultNode;
        DebugTools.DevAssert(MatrixResultNode == null || 1 == MatrixResultNode.Columns, null);
    }

    private void GetZAndUnits(out IMatrixResultNode z, out IList<Unit> units)
    {
        IResultNode resultNode = MatrixResultNode[MatrixResultNode.Rows - 3, 0];
        if (resultNode is IMatrixResultNode)
        {
            z = resultNode as IMatrixResultNode;
            units = null;
        }
        else if (resultNode is IUnitedValueResultNode)
        {
            IUnitedValueResultNode unitedValueResultNode = resultNode as IUnitedValueResultNode;
            z = unitedValueResultNode.Value as IMatrixResultNode;
            units = unitedValueResultNode.Units;
        }
        else
        {
            z = null;
            units = null;
        }
    }

    private IMatrixResultNode GetContour(int index)
    {
        return MatrixResultNode[index, 0] as IMatrixResultNode;
    }

    internal ContourLogicalDataCollection Convert()
    {
        ContourLogicalDataCollection contourLogicalDataCollection = new ContourLogicalDataCollection();
        if (!IsEmpty)
        {
            int contours = Contours;
            contourLogicalDataCollection.Contours = new ContourLogicalData[contours];
            IMatrixResultNode bounds = Bounds;
            DebugTools.DevAssert(4 == bounds.Rows, null);
            DebugTools.DevAssert(1 == bounds.Columns, null);
            IMatrixResultNode zNode = ZNode;
            IMatrixResultNode gradientSigns = GradientSigns;
            DebugTools.DevAssert(1 == zNode.Columns, null);
            DebugTools.DevAssert(1 == gradientSigns.Columns, null);
            DebugTools.DevAssert(contours == zNode.Rows, null);
            DebugTools.DevAssert(contours == gradientSigns.Rows, null);
            for (int i = 0; i < contours; i++)
            {
                IMatrixResultNode contour = GetContour(i);
                ContourLogicalData contourLogicalData = default;
                int rows = contour.Rows;
                DebugTools.DevAssert(2 == contour.Columns, null);
                contourLogicalData.Points = new Point[rows];
                for (int j = 0; j < rows; j++)
                {
                    IResultNode node = contour[j, 0];
                    IResultNode node2 = contour[j, 1];
                    contourLogicalData.Points[j] = new Point(ResultNodeToDouble(node), ResultNodeToDouble(node2));
                }
                contourLogicalData.ZLevel = ResultNodeToDouble(zNode[i, 0]);
                contourLogicalData.GradientSign = ResultNodeToDouble(gradientSigns[i, 0]);
                contourLogicalDataCollection.Compute(contourLogicalData, i);
            }
            Rect boundingRectangleXy = default;
            double num = ResultNodeToDouble(bounds[0, 0]);
            double num2 = ResultNodeToDouble(bounds[1, 0]);
            boundingRectangleXy.X = Math.Min(num, num2);
            boundingRectangleXy.Width = Math.Abs(num2 - num);
            double num3 = ResultNodeToDouble(bounds[2, 0]);
            double num4 = ResultNodeToDouble(bounds[3, 0]);
            boundingRectangleXy.Y = Math.Min(num3, num4);
            boundingRectangleXy.Height = Math.Abs(num4 - num3);
            contourLogicalDataCollection.BoundingRectangleXy = boundingRectangleXy;
        }
        return contourLogicalDataCollection;
    }

    private static double ResultNodeToDouble(IResultNode node)
    {
        if (!DebugTools.DevAssert(node is IRealResultNode, null).Passed)
        {
            return double.NaN;
        }
        return ((IRealResultNode)node).Value;
    }
}
