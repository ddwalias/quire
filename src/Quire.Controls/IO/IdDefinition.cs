#nullable disable
using System;
using System.Globalization;
using Quire;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class IdDefinition
{
    internal const string DefinitionSexpFormat = "(:= {0} {1})";

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public string IdSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public double Value { get; private set; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOIdDefinitionMechanicalDifferentialTests")]
    public string ValueSExpression { get; private set; }

    internal IdDefinition(string idSExpression, double value, string scalingSExpression)
    {
        IdSExpression = idSExpression;
        Value = value;
        ScalingSExpression = scalingSExpression;
        _rl = ConstructCurrentRl();
    }

    internal static string ConstructValueSExpression(double value)
    {
        return string.Format(CultureInfo.InvariantCulture, (value >= 0.0) ? "{0}" : "(@NEG {0})", Math.Abs(value));
    }

    private string ConstructCurrentRl()
    {
        string text;
        ValueSExpression = null;
        if (IdSExpression.IsNotEmpty())
        {
            ValueSExpression = ConstructValueSExpression(Value);
            string arg = string.IsNullOrEmpty(ScalingSExpression) ? ValueSExpression : string.Format(MathNodeToSExpressionTransform.ScaleSexpFormat, ValueSExpression, ScalingSExpression);
            string sExpr = string.Format("(:= {0} {1})", IdSExpression, arg);
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
        IdDefinition idDefinition = (IdDefinition)obj;
        if (idDefinition.IdSExpression.Equals(IdSExpression) && idDefinition.Value.Equals(Value))
        {
            return idDefinition.ScalingSExpression.Equals(ScalingSExpression);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return IdSExpression.GetHashCode() ^ Value.GetHashCode() ^ ScalingSExpression.GetHashCode();
    }
}
