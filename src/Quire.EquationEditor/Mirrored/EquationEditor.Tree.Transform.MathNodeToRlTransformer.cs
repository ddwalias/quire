using System;
using System.Collections.Generic;
using System.Globalization;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.EquationEditor.Tree.Transform;

public partial class MathNodeToRlTransformer
{
    public static bool IsValidRl(string rl)
    {
        return rl.IsNotEmpty() && rl != "nil";
    }

    public string SetRlidAndTransform(MathNode node)
    {
        node.SetRlidForTree();
        return Transform(node, (IDictionary<string, PositionsToLabel>)null);
    }

    public static string GetLabelRLSuffix(MathNode node, string labelForNoLabel)
    {
        var identifier = node.NodeInfo as IdentifierNodeInfo;
        if (identifier == null)
        {
            return string.Empty;
        }

        return identifier.IdLabel.GetRl(labelForNoLabel);
    }

    public static string UnescapeLiteral(string str)
    {
        str = str.Replace("\\\\", "\\");
        str = str.Replace("\\\"", "\"");
        return str;
    }

    public static bool IsNumeric(object expression)
    {
        if (expression == null || expression is string)
        {
            return false;
        }

        return double.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _);
    }

    public static int GetRlIndexForEvaluationNode(MathNode evalNode)
    {
        var evaluationNodes = MathNode.Utilities.GetEvaluationNodes(evalNode.EquationRootNode);
        if (evaluationNodes == null || !evaluationNodes.Contains(evalNode))
        {
            return -1;
        }

        return a(evaluationNodes)[evalNode];
    }

    public string TransformLhsOfDef(MathNode node)
    {
        return d(node, j__);
    }

    public string SetRlidAndTransformWithResultScalingFactor(MathNode node, MathNode resultScalingFactor)
    {
        node.SetRlidForTree();
        var previous = k__;
        k__ = resultScalingFactor;
        var result = Transform(node, (IDictionary<string, PositionsToLabel>)null);
        k__ = previous;
        return result;
    }
}
