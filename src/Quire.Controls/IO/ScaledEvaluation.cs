#nullable disable
using System.Collections.Generic;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class ScaledEvaluation
{
    private readonly UnitMonomialBuilder _builder;

    private const string EvalScaledExpressionRlFormat = "(eval (numeric (op_scale_inverse {0} {1})))";

    internal const string EvalExpressionRlFormat = "(eval (numeric ({0})))";

    private ScaledEvaluation(string baseSExpression, string scalingSExpression)
    {
        _builder = UnitMonomialBuilder.Instance;
        ScalingSExpression = scalingSExpression;
        BaseSExpression = baseSExpression;
        _rl = ConstructCurrentRl();
    }

    internal static ScaledEvaluation Create(string baseSExpression, string scalingSExpression)
    {
        if (baseSExpression == null)
        {
            return null;
        }
        ScaledEvaluation evaluation = new ScaledEvaluation(baseSExpression, scalingSExpression);
        if (!MathNodeToRlTransformer.IsValidRl(evaluation._rl))
        {
            return null;
        }
        return evaluation;
    }

    private SimpleResultNode ExtractUnits(out IList<Unit> units)
    {
        SimpleResultNode result;
        UnitedValueSimpleResultNode unitedResult = Result as UnitedValueSimpleResultNode;
        if (unitedResult == null)
        {
            result = (SimpleResultNode)Result;
            units = null;
        }
        else
        {
            result = unitedResult.Value;
            units = unitedResult.Units;
        }
        return result;
    }

    public ScaledEvaluationResult GetComputedValue(out double value, out string units)
    {
        ScaledEvaluationResult result;
        IList<Unit> resultUnits;
        SimpleResultNode simpleResult = ExtractUnits(out resultUnits);
        if (simpleResult is RealSimpleResultNode)
        {
            value = (simpleResult as RealSimpleResultNode).Value;
            units = _builder.ConstructUnitMonomial(resultUnits);
            result = ScaledEvaluationResult.NoError;
        }
        else
        {
            value = double.NaN;
            units = string.Empty;
            result = ScaledEvaluationResult.ResultTypeIncompatible;
        }
        return result;
    }

    public ScaledEvaluationResult GetComputedValue(out double[][] value, out string units)
    {
        ScaledEvaluationResult result;
        IList<Unit> resultUnits;
        LazyMatrixSimpleResultNode lazyMatrix = ExtractUnits(out resultUnits) as LazyMatrixSimpleResultNode;
        if (lazyMatrix != null)
        {
            double[][] matrix = new double[lazyMatrix.Rows][];
            for (int i = 0; i < lazyMatrix.Rows; i++)
            {
                matrix[i] = new double[lazyMatrix.Columns];
                for (int j = 0; j < lazyMatrix.Columns; j++)
                {
                    RealSimpleResultNode cell = lazyMatrix[i, j] as RealSimpleResultNode;
                    if (cell != null)
                    {
                        matrix[i][j] = cell.Value;
                    }
                    else
                    {
                        value = null;
                        units = string.Empty;
                        return ScaledEvaluationResult.MatrixResultInvalid;
                    }
                }
            }
            value = matrix;
            units = _builder.ConstructUnitMonomial(resultUnits);
            result = ScaledEvaluationResult.NoError;
        }
        else
        {
            value = null;
            units = string.Empty;
            result = ScaledEvaluationResult.ResultTypeIncompatible;
        }
        return result;
    }

    public ScaledEvaluationResult GetComputedValue(out string value)
    {
        ScaledEvaluationResult result;
        IList<Unit> resultUnits;
        StringSimpleResultNode stringResult = ExtractUnits(out resultUnits) as StringSimpleResultNode;
        if (stringResult != null)
        {
            value = stringResult.Value;
            result = ScaledEvaluationResult.NoError;
        }
        else
        {
            value = null;
            result = ScaledEvaluationResult.ResultTypeIncompatible;
        }
        return result;
    }

    private string ConstructCurrentRl()
    {
        string baseRl = ShadowCalculatable.ConvertSExpressionToRl(BaseSExpression);
        if (!MathNodeToRlTransformer.IsValidRl(baseRl))
        {
            return "nil";
        }
        string text;
        if (!string.IsNullOrEmpty(ScalingSExpression))
        {
            string scalingRl = ShadowCalculatable.ConvertSExpressionToRl(ScalingSExpression);
            string scalingUnitRl = scalingRl;
            if (DebugTools.DevAssert(!scalingRl.Contains("*"), null).Failed)
            {
                scalingUnitRl = scalingRl.Replace("*", "UNIT");
            }
            text = string.Format(EvalScaledExpressionRlFormat, baseRl, scalingUnitRl);
        }
        else
        {
            text = string.Format(EvalExpressionRlFormat, baseRl);
        }
        return text;
    }

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public override bool IsErrorOnScalingUnits() => false;

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public string BaseSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOScaledEvaluationMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }
}
