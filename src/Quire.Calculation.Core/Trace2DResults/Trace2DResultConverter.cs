#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class Trace2DResultConverter
{
    private const int FirstColumnIndex = 0;

    private const int XResultIdx = 0;

    private const int YResultIdx = 1;

    private const int XMinValueIdx = 2;

    private const int XMaxValueIdx = 3;

    private const int YMinValueIdx = 4;

    private const int YMaxValueIdx = 5;

    private const int PlotTypeIdx = 6;

    [MirrorVerified("Release IL")]
    private readonly ITrace2DConverterHelper _helper;

    [MirrorVerified("Release IL")]
    public Trace2DResultConverter(ITrace2DConverterHelper helper)
    {
        _helper = helper;
    }

    [MirrorVerified("Release IL")]
    private static bool GetDataSourceType(IMatrixResultNode matrix, out Trace2DResultType type)
    {
        type = Trace2DResultType.Vector;
        bool result = false;
        try
        {
            type = (Trace2DResultType)Enum.Parse(typeof(Trace2DResultType), ((IStringResultNode)matrix[6, 0]).Value, ignoreCase: true);
            result = true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidCastException)
        {
        }
        return result;
    }

    [MirrorVerified("Release IL")]
    private TraceDataPoints BuildTraceData(IMatrixResultNode xResult, IMatrixResultNode yResult, TraceType traceTypeAuto, IMatrixToVectorsConverter xConverter, IMatrixToVectorsConverter yConverter)
    {
        int maxRangeColumns = TraceTypeDefinitions.GetMaxRangeColumns(traceTypeAuto);
        int singleVectorLength;
        int maxCount;
        VectorsType vectorsType;
        if (ReadAsColumns)
        {
            maxCount = TraceTypeDefinitions.GetMaxResultColumns(traceTypeAuto);
            singleVectorLength = yResult.Rows;
            vectorsType = VectorsType.Columns;
        }
        else
        {
            maxCount = int.MaxValue;
            singleVectorLength = yResult.Columns;
            vectorsType = VectorsType.Rows;
        }
        IList<IListResultNode> input = xConverter.ReadVectors(maxRangeColumns);
        IList<IListResultNode> input2 = yConverter.ReadVectors(maxCount);
        return new TraceDataPoints(new VectorListResultNode(input, xResult.Rows), new VectorListResultNode(input2, singleVectorLength, vectorsType));
    }

    [MirrorVerified("Release IL")]
    private static string TryParseResult(IMatrixResultNode matrix, out IMatrixResultNode xResult, out IMatrixResultNode yResult, out IList<Unit> rangeUnits, out IList<Unit> resultUnits, out Trace2DResultType type)
    {
        string result = null;
        if (!GetDataSourceType(matrix, out type))
        {
            result = "PlotControl.Error.UnsupportedType";
            rangeUnits = null;
            resultUnits = null;
            xResult = null;
            yResult = null;
        }
        else
        {
            rangeUnits = DissectUnits(matrix[0, 0], out xResult);
            resultUnits = DissectUnits(matrix[1, 0], out yResult);
            if (DebugTools.DevAssert(xResult != null && yResult != null, null).Failed)
            {
                result = "PlotControl.Error.QuickPlotElementsOutOfRange";
            }
        }
        return result;
    }

    [MirrorVerified("Release IL")]
    private bool ReadAsColumns => _helper.CurrentType != TraceType.Boxes;

    [MirrorVerified("Release IL")]
    private TraceType GetTraceTypePreset(int xResultColumns, int yResultColumns)
    {
        TraceType traceType = _helper.CurrentType;
        if (_helper.IsTypeDefault && DebugTools.DevAssert(traceType == TraceType.Lines || traceType == TraceType.Waterfall, null).Passed)
        {
            traceType = ((xResultColumns > 1 || yResultColumns > 1) ? TraceType.Waterfall : TraceType.Lines);
        }
        return traceType;
    }

    [MirrorVerified("Release IL")]
    private static IMatrixToVectorsConverter GetMatrixToVectorsConverter(out bool isDiscrete, IMatrixResultNode resultMatrix, bool readAsColumns = true)
    {
        isDiscrete = false;
        IMatrixToVectorsConverter matrixToVectorsConverter;
        if (resultMatrix is ILazyMatrixResultNode node)
        {
            matrixToVectorsConverter = VectorsLazyMatrixConverter.Create(node, readAsColumns);
        }
        else
        {
            DebugTools.DevAssert(resultMatrix is IEagerMatrixResultNode, null);
            matrixToVectorsConverter = VectorsLegacyConverter.Create(resultMatrix, readAsColumns);
        }
        if (DebugTools.DevAssert(matrixToVectorsConverter != null, null).Failed)
        {
            return null;
        }
        ResultValueType commonType = matrixToVectorsConverter.GetCommonType();
        if (commonType == ResultValueType.Mixed || commonType == ResultValueType.Empty)
        {
            return null;
        }
        isDiscrete = commonType == ResultValueType.String;
        return matrixToVectorsConverter;
    }
}
