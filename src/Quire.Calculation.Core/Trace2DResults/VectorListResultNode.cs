#nullable disable
using System.Collections.Generic;
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class VectorListResultNode : SimpleResultNode, IVectorListResultNode, IResultNode, IVisitable<IResultNodeVisitor>
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    private readonly IList<IListResultNode> _vectors;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public int SingleVectorLength { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public VectorsType VectorsType { get; private set; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public VectorListResultNode(IList<IListResultNode> input, int singleVectorLength, VectorsType vectorsType = VectorsType.Columns)
    {
        SingleVectorLength = singleVectorLength;
        VectorsType = vectorsType;
        _vectors = input;
        DebugTools.DevAssert(_vectors.All((IListResultNode vec) => vec.Count == singleVectorLength), null);
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public int ElementsCount => Count * SingleVectorLength;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public static ResultValueType GetResultType(IListResultNode resultNode)
    {
        if (resultNode == null || resultNode.Count == 0)
        {
            return ResultValueType.Empty;
        }
        if (resultNode is IStringListResultNode)
        {
            return ResultValueType.String;
        }
        if (resultNode is IDoubleListResultNode doubleListResultNode)
        {
            if (doubleListResultNode.MeaningfulValuesCount <= 0)
            {
                return ResultValueType.NaNs;
            }
            return ResultValueType.Numeric;
        }
        DebugTools.FailAssertion("unsupported IListResultNode found: {0}", resultNode.GetType().Name);
        return ResultValueType.Mixed;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public IList<IListResultNode> Vectors => _vectors;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public ResultValueType CommonValuesType
    {
        get
        {
            if (Vectors.IsNullOrEmpty())
            {
                return ResultValueType.Empty;
            }
            ResultValueType resultValueType = GetResultType(Vectors.First());
            for (int i = 1; i < Vectors.Count && resultValueType != ResultValueType.Mixed; i++)
            {
                ResultValueType resultType = GetResultType(Vectors[i]);
                if (resultValueType == ResultValueType.NaNs)
                {
                    resultValueType = resultType;
                }
                else if (resultType != ResultValueType.NaNs && resultValueType != resultType)
                {
                    resultValueType = ResultValueType.Mixed;
                }
            }
            return resultValueType;
        }
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public ResultValueType GetVectorValuesType(int index) => GetResultType(Vectors[index]);

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public int Count => _vectors.Count;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public IListResultNode this[int index] { get => _vectors[index]; set => _vectors[index] = value; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorListResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);
}
