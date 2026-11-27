#nullable disable
using System.Globalization;
using System.Text;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class MatrixDefinition
{
    private const string MatrixSexpFormat = "(@MATRIX {0} {1} {2})";

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public string IdSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public double[,] Value { get; private set; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOMatrixDefinitionMechanicalDifferentialTests")]
    public string ValueSExpression { get; private set; }

    internal MatrixDefinition(string idSExpression, double[,] value, string scalingSExpression)
    {
        IdSExpression = idSExpression;
        Value = value;
        ScalingSExpression = scalingSExpression;
        _rl = ConstructCurrentRl();
    }

    private string BuildListOfValues()
    {
        if (Value == null)
        {
            return string.Empty;
        }
        StringBuilder stringBuilder = new StringBuilder();
        int length = Value.GetLength(0);
        int columnCount = Value.GetLength(1);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                string arg = IdDefinition.ConstructValueSExpression(Value[i, j]);
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}", arg);
            }
        }
        return stringBuilder.ToString(1, stringBuilder.Length - 1);
    }

    private string ConstructCurrentRl()
    {
        string text;
        ValueSExpression = null;
        if (IdSExpression.IsNotEmpty() && Value != null)
        {
            string arg = BuildListOfValues();
            ValueSExpression = string.Format(CultureInfo.InvariantCulture, "(@MATRIX {0} {1} {2})", Value.GetLength(0), Value.GetLength(1), arg);
            string scaledValueSExpression = (string.IsNullOrEmpty(ScalingSExpression) ? ValueSExpression : string.Format(CultureInfo.InvariantCulture, MathNodeToSExpressionTransform.ScaleSexpFormat, ValueSExpression, ScalingSExpression));
            string sExpr = string.Format(CultureInfo.InvariantCulture, "(:= {0} {1})", IdSExpression, scaledValueSExpression);
            try
            {
                text = ShadowCalculatable.ConvertSExpressionToRl(sExpr);
            }
            catch (InvalidSExpression invalidSExpression)
            {
                DebugTools.FailAssertion(invalidSExpression.Message);
                text = "nil";
            }
        }
        else
        {
            text = "nil";
        }
        return text;
    }

    public override void SetResult(IResultNode result)
    {
        DebugTools.DevAssert(result is EmptySimpleResultNode, "Logic Error: Definitions should never have results");
    }

    public override bool IsErrorOnScalingUnits()
    {
        if (!string.IsNullOrEmpty(ScalingSExpression) && Error != null)
        {
            return true;
        }
        return false;
    }

    private static bool IsMatrixEqual(double[,] first, double[,] second)
    {
        int length = first.GetLength(0);
        int firstColumnCount = first.GetLength(1);
        int secondRowCount = second.GetLength(0);
        int secondColumnCount = second.GetLength(1);
        if (length == secondRowCount && firstColumnCount == secondColumnCount)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < firstColumnCount; j++)
                {
                    if (first[i, j] != second[i, j])
                    {
                        return false;
                    }
                }
            }
            // IL requires goto IL_MATCH over false return so false return precedes true return
            goto IL_MATCH;
        }
        return false;
    IL_MATCH:
        return true;
    }

    public override bool Equals(object obj)
    {
        if (this == obj)
        {
            return true;
        }
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        MatrixDefinition matrixDefinition = (MatrixDefinition)obj;
        if (matrixDefinition.IdSExpression.Equals(IdSExpression) && IsMatrixEqual(matrixDefinition.Value, Value))
        {
            return matrixDefinition.ScalingSExpression.Equals(ScalingSExpression);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return IdSExpression.GetHashCode() ^ Value.GetHashCode() ^ ScalingSExpression.GetHashCode();
    }
}
