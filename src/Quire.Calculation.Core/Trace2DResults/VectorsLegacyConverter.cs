#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

internal partial class VectorsLegacyConverter : VectorsConverterBase, IMatrixToVectorsConverter
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private readonly IMatrixResultNode _node;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private readonly bool _readAsColumns;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private bool _hasCache;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private readonly int _vectorsNum;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private readonly int _singleVectorLength;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private Func<int, int, IResultNode> _readElement;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private List<IListResultNode> _vectorsRes;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private ResultValueType _commonValueType;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    public static VectorsLegacyConverter Create(IMatrixResultNode node, bool readAsColumns)
    {
        if (node != null)
        {
            return new VectorsLegacyConverter(node, readAsColumns);
        }
        return null;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private VectorsLegacyConverter(IMatrixResultNode node, bool readAsColumns)
    {
        _node = node;
        _readAsColumns = readAsColumns;
        _vectorsNum = (readAsColumns ? node.Columns : node.Rows);
        _singleVectorLength = (readAsColumns ? node.Rows : node.Columns);
        _hasCache = false;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    public IList<IListResultNode> ReadVectors(int maxCount)
    {
        if (!_hasCache)
        {
            ConvertAndCache();
        }
        if (_vectorsRes.Count > maxCount)
        {
            return _vectorsRes.GetRange(0, maxCount);
        }
        return _vectorsRes;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    public ResultValueType GetCommonType()
    {
        if (!_hasCache)
        {
            ConvertAndCache();
        }
        return _commonValueType;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    public bool ReadAsColumns => _readAsColumns;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private IResultNode ReadElement(int iVector, int iElement)
    {
        if (_readElement == null)
        {
            if (_readAsColumns)
            {
                _readElement = (int row, int col) => _node[col, row];
            }
            else
            {
                _readElement = (int row, int col) => _node[row, col];
            }
        }
        return _readElement(iVector, iElement);
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private void ConvertAndCache()
    {
        List<IListResultNode> list = new List<IListResultNode>(_vectorsNum);
        PlainDataTypes plainDataTypes = PlainDataTypes.Uninitialized;
        for (int i = 0; i < _vectorsNum && list != null; i++)
        {
            PlainDataTypes primaryDataType;
            IListResultNode listResultNode = TryConvertSingleVector(i, out primaryDataType);
            if (listResultNode == null)
            {
                list = null;
            }
            else
            {
                list.Add(listResultNode);
            }
            plainDataTypes |= primaryDataType;
        }
        _vectorsRes = list;
        _commonValueType = VectorsConverterBase.GetVectorsValueType(plainDataTypes);
        _hasCache = true;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private IListResultNode TryConvertSingleVector(int iVector, out PlainDataTypes primaryDataType)
    {
        primaryDataType = PlainDataTypes.NonPlainType;
        int firstMeaningfullValueIndex;
        ResultValueType? firstMeaningfulValueType = GetFirstMeaningfulValueType(iVector, out firstMeaningfullValueIndex);
        if (!firstMeaningfulValueType.HasValue)
        {
            return null;
        }
        IListResultNode result = null;
        IList<double> list2;
        IList<string> list;
        switch (firstMeaningfulValueType.Value)
        {
            case ResultValueType.Numeric:
                list2 = ReadDoubleValues(iVector, firstMeaningfullValueIndex);
                if (list2 != null)
                {
                    result = new DoubleListResultNode(list2);
                    primaryDataType = PlainDataTypes.Double;
                }
                break;
            case ResultValueType.String:
                list = ReadStringValues(iVector, firstMeaningfullValueIndex);
                if (list != null)
                {
                    result = new StringListResultNode(list);
                    primaryDataType = PlainDataTypes.String;
                }
                break;
            case ResultValueType.NaNs:
                result = new DoubleListResultNode(double.NaN, _singleVectorLength);
                primaryDataType = PlainDataTypes.NaN;
                break;
            default:
                DebugTools.FailAssertion(firstMeaningfulValueType.Value.ToString(), Array.Empty<object>());
                break;
        }
        return result;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private IList<string> ReadStringValues(int iVector, int startValueIndex)
    {
        IList<string> list = new List<string>(_singleVectorLength);
        for (int i = 0; i < startValueIndex; i++)
        {
            list.Add(null);
        }
        for (int j = startValueIndex; j < _singleVectorLength && list != null; j++)
        {
            if (ReadString(ReadElement(iVector, j), out var stringValue))
            {
                list.Add(stringValue);
            }
            else
            {
                list = null;
            }
        }
        return list;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private IList<double> ReadDoubleValues(int iVector, int startValueIndex)
    {
        IList<double> list = new List<double>(_singleVectorLength);
        for (int i = 0; i < startValueIndex; i++)
        {
            list.Add(double.NaN);
        }
        for (int j = startValueIndex; j < _singleVectorLength && list != null; j++)
        {
            if (ReadDoubleOrNone(ReadElement(iVector, j), out var doubleValue))
            {
                list.Add(doubleValue);
            }
            else
            {
                list = null;
            }
        }
        return list;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private ResultValueType? GetFirstMeaningfulValueType(int iVector, out int firstMeaningfullValueIndex)
    {
        ResultValueType value = ResultValueType.Empty;
        for (firstMeaningfullValueIndex = 0; firstMeaningfullValueIndex < _singleVectorLength; firstMeaningfullValueIndex++)
        {
            IResultNode element = ReadElement(iVector, firstMeaningfullValueIndex);
            if (ReadDoubleOrNone(element, out var doubleValue))
            {
                if (double.IsNaN(doubleValue))
                {
                    value = ResultValueType.NaNs;
                }
                else
                {
                    return ResultValueType.Numeric;
                }
            }
            else if (ReadString(element, out var stringValue))
            {
                return ResultValueType.String;
            }
            else
            {
                return null;
            }
        }
        return value;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private static bool ReadDoubleOrNone(IResultNode element, out double doubleValue)
    {
        doubleValue = double.NaN;
        if (element is IComplexResultNode || element is IImaginaryResultNode)
        {
            return true;
        }
        if (element is IRealResultNode realResultNode)
        {
            doubleValue = realResultNode.Value;
            return true;
        }
        return false;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsLegacyConverterMechanicalDifferentialTests")]
    private static bool ReadString(IResultNode element, out string stringValue)
    {
        stringValue = null;
        if (element is IStringResultNode stringResultNode)
        {
            stringValue = stringResultNode.Value;
            return true;
        }
        if (ReadDoubleOrNone(element, out var doubleValue))
        {
            return double.IsNaN(doubleValue);
        }
        return false;
    }
}
