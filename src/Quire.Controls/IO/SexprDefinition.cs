#nullable disable
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class SexprDefinition
{
    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public override void SetResult(IResultNode result) { }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public override bool IsErrorOnScalingUnits() => false;

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    private string _rl;

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public override string Rl { get => _rl; set => _rl = value; }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public string IdSExpression { get; private set; }

    public bool IsValid => MathNodeToRlTransformer.IsValidRl(Rl);

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public string ValueSExpression { get; private set; }

    [MirrorVerified("PtcControlsIOSexprDefinitionMechanicalDifferentialTests")]
    public string ScalingSExpression { get; private set; }

    internal SexprDefinition(string idSexpr, string valueSexpr, string scalingSexpr = null)
    {
        IdSExpression = idSexpr;
        ValueSExpression = valueSexpr;
        ScalingSExpression = scalingSexpr;
        _rl = ConstructRl();
    }

    private string ConstructRl()
    {
        string result = "nil";
        if (ValueSExpression.IsNotEmpty() && IdSExpression.IsNotEmpty())
        {
            string text = SExpressionToRlTransform.Default.TransformIdSubNode(IdSExpression);
            if (DebugTools.DevAssert(MathNodeToRlTransformer.IsValidRl(text), null).Passed)
            {
                result = string.Format("(eval (numeric ({0})))", text);
            }
        }
        return result;
    }
}
