#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class VectorsLazyMatrixConverter : VectorsConverterBase, IMatrixToVectorsConverter
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private VectorsLazyMatrixConverter(ILazyMatrixResultNode node, bool readAsColumns)
    {
        _node = node;
        _readAsColumns = readAsColumns;
        _vectorsNum = (readAsColumns ? node.Columns : node.Rows);
        _singleVectorLength = (readAsColumns ? node.Rows : node.Columns);
        _plainDataAccessor = _node.MatrixProxy.GetPlainData();
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private IList<IList> GetPlainDataValues(int startIndex, int endIndex)
    {
        if (_getPlainDataValues == null)
        {
            if (_readAsColumns)
            {
                _getPlainDataValues = (int start, int end) => _plainDataAccessor.GetColumnsPlainDataValues(start, end);
            }
            else
            {
                _getPlainDataValues = (int start, int end) => _plainDataAccessor.GetRowsPlainDataValues(start, end);
            }
        }
        return _getPlainDataValues(startIndex, endIndex);
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    public static VectorsLazyMatrixConverter Create(ILazyMatrixResultNode node, bool readAsColumns)
    {
        if (node != null)
        {
            return new VectorsLazyMatrixConverter(node, readAsColumns);
        }
        return null;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    public ResultValueType GetCommonType()
    {
        if (_commonValueType == ResultValueType.Empty)
        {
            PlainDataTypes entireMatrixDataTypes = _plainDataAccessor.GetEntireMatrixDataTypes();
            _commonValueType = VectorsConverterBase.GetVectorsValueType(entireMatrixDataTypes);
        }
        return _commonValueType;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    public bool ReadAsColumns => _readAsColumns;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    public IList<IListResultNode> ReadVectors(int maxCount)
    {
        IList<IListResultNode> result;
        switch (GetCommonType())
        {
            case ResultValueType.Numeric:
            case ResultValueType.String:
            case ResultValueType.NaNs:
                result = BuildResultVectors(maxCount);
                break;
            case ResultValueType.Empty:
            case ResultValueType.Mixed:
            default:
                result = null;
                break;
        }
        return result;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private IList<IListResultNode> BuildResultVectors(int maxCount)
    {
        int endIndex = Math.Min(maxCount, _vectorsNum) - 1;
        IList<IList> plainDataValues = GetPlainDataValues(0, endIndex);
        IList<IListResultNode> list = new List<IListResultNode>(plainDataValues.Count);
        foreach (IList item in plainDataValues)
        {
            if (item is IList<double> doubleValues)
            {
                list.Add(new DoubleListResultNode(doubleValues, double.NaN, _singleVectorLength));
                continue;
            }
            if (item is IList<string> data)
            {
                list.Add(new StringListResultNode(data, null, _singleVectorLength));
                continue;
            }
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return null;
        }
        return list;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private readonly ILazyMatrixResultNode _node;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private readonly bool _readAsColumns;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private readonly IMatrixValueDataAccessorWrapper _plainDataAccessor;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private readonly int _vectorsNum;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private readonly int _singleVectorLength;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private Func<int, int, IList<IList>> _getPlainDataValues;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLazyMatrixConverterMechanicalDifferentialTests")]
    private ResultValueType _commonValueType;
}
