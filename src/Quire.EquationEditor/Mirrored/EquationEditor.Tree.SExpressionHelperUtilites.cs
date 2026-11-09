using System.Collections.Generic;
using System.Linq;
using Quire.EquationEditor.Tree.Transform;

namespace Quire.EquationEditor.Tree;

public static partial class SExpressionHelperUtilites
{
    internal static MathNode a(string sExpression)
    {
        MathNode mathNode = SExpressionToMathNodeTransform.Default.SafeTransform(sExpression);
        if (DebugTools.DevAssert(mathNode != null && MathNode.Utilities.a(mathNode), null).Failed)
        {
            return null;
        }
        if (!(mathNode.DescriptorName == "FUNCTION_DEFINITION"))
        {
            return mathNode;
        }
        return mathNode[0];
    }

    public static string ExtractIdOrFunctionDefinitionName(string idSExp)
    {
        MathNode mathNode = a(idSExp);
        if (mathNode == null)
        {
            return null;
        }
        IdentifierNodeInfo identifierNodeInfo = mathNode.NodeInfo as IdentifierNodeInfo;
        if (!DebugTools.AssertNotNull(identifierNodeInfo, null).Failed)
        {
            return identifierNodeInfo.Value.Text;
        }
        return null;
    }

    public static bool IsIdentifierSExpr(string idSExp)
    {
        return IsValidType(idSExp, new[] { "IDENTIFIER" }, null);
    }

    public static bool IsValidType(string idSExp, IEnumerable<string> allowedDescriptors, IEnumerable<string> disallowedDescriptors)
    {
        MathNode mathNode = SExpressionToMathNodeTransform.Default.SafeTransform(idSExp);
        if (DebugTools.AssertNotNull(mathNode, null).Passed && (allowedDescriptors == null || allowedDescriptors.Contains(mathNode.DescriptorName)))
        {
            if (disallowedDescriptors != null)
            {
                return !disallowedDescriptors.Contains(mathNode.DescriptorName);
            }
            return true;
        }
        return false;
    }

    public static void SplitScalingAndValue(ref string valueSExpr, out string scalingSExpr)
    {
        MathNode mathNode = SExpressionToMathNodeTransform.Default.SafeTransform(valueSExpr);
        scalingSExpr = null;
        if (mathNode == null)
        {
            valueSExpr = null;
        }
        else if (mathNode.DescriptorName == "SCALE")
        {
            valueSExpr = mathNode.Children[0].SExpression;
            scalingSExpr = mathNode.Children[1].SExpression;
        }
    }
}
