#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class TraceResultAnalyzer
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    public static string[] GetLabels(IList<IStringListResultNode> vectors)
    {
        int num = vectors.Sum((IStringListResultNode vec) => vec.MeaningfulValuesCount);
        if (DebugTools.DevAssert(num != 0, null).Failed)
        {
            return null;
        }
        string[] array = new string[num];
        int nextLabelIndex = 0;
        foreach (IStringListResultNode vector in vectors)
        {
            foreach (string item in vector.List)
            {
                if (vector.IsMeaningfulValue(item))
                {
                    array[nextLabelIndex++] = item;
                }
            }
        }
        return array;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    public static bool AddOneDataSegment(IList<Point> pointsList, IList<double> xData, IList<double> yData, bool ensureTrailingNaN)
    {
        if (xData == null || yData == null)
        {
            return false;
        }
        int num = -1;
        int pointCount = Math.Min(xData.Count, yData.Count);
        for (int i = 0; i < pointCount; i++)
        {
            Point val = new Point(xData[i], yData[i]);
            pointsList.Add(val);
            if (!val.IsNaN())
            {
                num = i;
            }
        }
        if (ensureTrailingNaN && num == pointCount - 1)
        {
            pointsList.Add(new Point(double.NaN, double.NaN));
        }
        return num != -1;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    private static bool AddOneDataSegment(IList<Point> pointsList, double xCoordinate, IList<double> yData, bool ensureTrailingNaN)
    {
        if (double.IsNaN(xCoordinate) || yData == null)
        {
            return false;
        }
        int num = -1;
        int lastNaNIndex = -1;
        int count = yData.Count;
        for (int i = 0; i < count; i++)
        {
            double yValue = yData[i];
            Point item = new Point(xCoordinate, yValue);
            if (!double.IsNaN(yValue))
            {
                num = i;
                pointsList.Add(item);
                continue;
            }
            if (lastNaNIndex != i - 1)
            {
                pointsList.Add(item);
            }
            lastNaNIndex = i;
        }
        if (ensureTrailingNaN && num == count - 1)
        {
            pointsList.Add(new Point(double.NaN, double.NaN));
        }
        return num != -1;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    private static IList<double> StringVectorToIndices(IStringListResultNode strData, int offset = 0)
    {
        IList<double> list = new List<double>(strData.Count);
        int num = 0;
        foreach (string item in strData.List)
        {
            list.Add(strData.IsMeaningfulValue(item) ? ((double)(offset + num++)) : double.NaN);
        }
        return list;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    public static IList<IList<double>> ToDoubleDataList(IVectorListResultNode result)
    {
        List<IList<double>> list = new List<IList<double>>(result.Count);
        int num = 0;
        foreach (IListResultNode vector in result.Vectors)
        {
            IDoubleListResultNode doubleListResultNode = vector as IDoubleListResultNode;
            IStringListResultNode stringListResultNode = vector as IStringListResultNode;
            if (doubleListResultNode != null)
            {
                list.Add(doubleListResultNode.List);
            }
            else if (stringListResultNode != null)
            {
                IList<double> indices = StringVectorToIndices(stringListResultNode, num);
                list.Add(indices);
                num += indices.Count;
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }
        return list;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    public static bool BuildPointsForWaterfall(IList<Point> pointsList, IList<IList<double>> xData, IList<IList<double>> yData)
    {
        int num = -1;
        int lastXVectorIndex = xData.Count - 1;
        int val = yData.Count - 1;
        int lastSegmentIndex = Math.Max(lastXVectorIndex, val);
        for (int i = 0; i <= lastSegmentIndex; i++)
        {
            IList<double> xData2 = xData[Math.Min(i, lastXVectorIndex)];
            IList<double> yData2 = yData[Math.Min(i, val)];
            if (AddOneDataSegment(pointsList, xData2, yData2, i < lastSegmentIndex))
            {
                num = i;
            }
        }
        return num != -1;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    public static bool BuildPointsForBoxes(IList<Point> pointsList, IList<double> xValues, IList<IList<double>> yData)
    {
        int num = -1;
        int lastValueIndex = Math.Min(xValues.Count, yData.Count) - 1;
        for (int i = 0; i <= lastValueIndex; i++)
        {
            double xValue = xValues[i];
            IList<double> list = yData[i];
            if (!double.IsNaN(xValue) && list != null && AddOneDataSegment(pointsList, xValue, list, i < lastValueIndex))
            {
                num = i;
            }
        }
        return num != -1;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsTraceResultAnalyzerMechanicalDifferentialTests")]
    public static bool BuildTuplesOfPoints(IList<Point> pointsList, IList<double> xVector, IList<double>[] yVectors, IList<double> xOffsets = null)
    {
        int count = pointsList.Count;
        int num = Math.Min(yVectors.Min((IList<double> vec) => vec?.Count ?? 0), xVector.Count);
        double[] array = null;
        if (xOffsets != null)
        {
            array = new double[yVectors.Length];
            int offsetCount = Math.Min(array.Length, xOffsets.Count);
            for (int offsetIndex = 0; offsetIndex < offsetCount; offsetIndex++)
            {
                array[offsetIndex] = xOffsets[offsetIndex];
            }
        }
        for (int num4 = 0; num4 < num; num4++)
        {
            double xCoordinate = xVector[num4];
            if (double.IsNaN(xCoordinate))
            {
                continue;
            }
            int lastValidVectorIndex = -1;
            for (int num7 = 0; num7 < yVectors.Length; num7++)
            {
                double yValue = yVectors[num7][num4];
                if (!double.IsNaN(yValue))
                {
                    Point item = new Point(xCoordinate, yValue);
                    if (array != null)
                    {
                        item.X += array[num7];
                    }
                    pointsList.Add(item);
                    lastValidVectorIndex = num7;
                }
            }
            if (lastValidVectorIndex != -1)
            {
                pointsList.Add(new Point(double.NaN, double.NaN));
            }
        }
        return count < pointsList.Count;
    }
}
