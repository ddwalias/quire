#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class TraceResultAnalyzer
{
    [MirrorVerified("PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests")]
    private static bool AddOneDataSegment(IList<Point> pointsList, double xCoordinate, IList<double> yData, bool ensureTrailingNaN)
    {
        if (double.IsNaN(xCoordinate) || yData == null)
        {
            return false;
        }

        int lastValidIndex = -1;
        int lastNaNIndex = -1;
        int count = yData.Count;
        for (int i = 0; i < count; i++)
        {
            double yCoordinate = yData[i];
            Point point = new(xCoordinate, yCoordinate);
            if (!double.IsNaN(yCoordinate))
            {
                lastValidIndex = i;
                pointsList.Add(point);
                continue;
            }

            if (lastNaNIndex != i - 1)
            {
                pointsList.Add(point);
            }

            lastNaNIndex = i;
        }

        if (ensureTrailingNaN && lastValidIndex == count - 1)
        {
            pointsList.Add(new Point(double.NaN, double.NaN));
        }

        return lastValidIndex != -1;
    }

    [MirrorVerified("PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests")]
    private static IList<double> StringVectorToIndices(IStringListResultNode strData, int offset = 0)
    {
        IList<double> values = new List<double>(strData.Count);
        int index = 0;
        foreach (string item in strData.List)
        {
            values.Add(strData.IsMeaningfulValue(item) ? (double)(offset + index++) : double.NaN);
        }

        return values;
    }

    [MirrorVerified("PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests")]
    public static IList<IList<double>> ToDoubleDataList(IVectorListResultNode result)
    {
        List<IList<double>> values = new(result.Count);
        int offset = 0;
        foreach (IListResultNode vector in result.Vectors)
        {
            IDoubleListResultNode doubleVector = vector as IDoubleListResultNode;
            IStringListResultNode stringVector = vector as IStringListResultNode;
            if (doubleVector != null)
            {
                values.Add(doubleVector.List);
            }
            else if (stringVector != null)
            {
                IList<double> indices = StringVectorToIndices(stringVector, offset);
                values.Add(indices);
                offset += indices.Count;
            }
            else
            {
                Quire.DebugTools.FailAssertion(null, System.Array.Empty<object>());
            }
        }

        return values;
    }

    [MirrorVerified("PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests")]
    public static bool BuildPointsForBoxes(IList<Point> pointsList, IList<double> xValues, IList<IList<double>> yData)
    {
        int lastValidIndex = -1;
        int lastIndex = System.Math.Min(xValues.Count, yData.Count) - 1;
        for (int i = 0; i <= lastIndex; i++)
        {
            double xValue = xValues[i];
            IList<double> values = yData[i];
            if (!double.IsNaN(xValue) && values != null && AddOneDataSegment(pointsList, xValue, values, i < lastIndex))
            {
                lastValidIndex = i;
            }
        }

        return lastValidIndex != -1;
    }

    [MirrorVerified("PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests")]
    public static bool BuildTuplesOfPoints(IList<Point> pointsList, IList<double> xVector, IList<double>[] yVectors, IList<double> xOffsets = null)
    {
        int initialCount = pointsList.Count;
        int tupleCount = System.Math.Min(yVectors.Min(vector => vector?.Count ?? 0), xVector.Count);
        double[] offsets = null;
        if (xOffsets != null)
        {
            offsets = new double[yVectors.Length];
            int count = System.Math.Min(offsets.Length, xOffsets.Count);
            for (int i = 0; i < count; i++)
            {
                offsets[i] = xOffsets[i];
            }
        }

        for (int i = 0; i < tupleCount; i++)
        {
            double xValue = xVector[i];
            if (double.IsNaN(xValue))
            {
                continue;
            }

            int lastValidVector = -1;
            for (int vectorIndex = 0; vectorIndex < yVectors.Length; vectorIndex++)
            {
                double yValue = yVectors[vectorIndex][i];
                if (!double.IsNaN(yValue))
                {
                    Point point = new(xValue, yValue);
                    if (offsets != null)
                    {
                        point.X += offsets[vectorIndex];
                    }

                    pointsList.Add(point);
                    lastValidVector = vectorIndex;
                }
            }

            if (lastValidVector != -1)
            {
                pointsList.Add(new Point(double.NaN, double.NaN));
            }
        }

        return initialCount < pointsList.Count;
    }
}
