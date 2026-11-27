using System.Collections.Generic;
using Quire.EquationEditor;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.View;

namespace Quire.Native.PtcEquationEditor;

internal partial class d : b
{
    public NavigatorState a(IMathNodeToVisualNodeMap A_0, IVisualMathNode A_1, Direction A_2, Direction A_3, int? A_4, double A_5, double A_6)
    {
        MathNode mathNode = A_0.AssociateNode(A_1);
        if (mathNode == null)
        {
            return NavigatorState.Empty;
        }

        if (mathNode.RestrictFromNavigation && !mathNode.AllowCaretOn(A_2))
        {
            return a(mathNode);
        }

        if (mathNode.CarriesValue && A_2 == Direction.Indeterminate && A_3 == Direction.Indeterminate)
        {
            if (A_1 is IVisualIdNode idNode)
            {
                return a(idNode, mathNode, A_5);
            }

            if (A_1 is IVisualTextNode textNode)
            {
                return a(textNode, mathNode, A_5);
            }
        }

        if (A_2 == Direction.Left && !mathNode.AllowCaretOnLeft)
        {
            A_2 = Direction.Indeterminate;
        }

        if (A_2 == Direction.Right && !mathNode.AllowCaretOnRight)
        {
            A_2 = Direction.Indeterminate;
        }

        if (A_2 != Direction.Indeterminate)
        {
            A_3 = Direction.Indeterminate;
            A_4 = null;
        }

        if (mathNode.NavigablePartCount <= 1)
        {
            A_3 = Direction.Indeterminate;
        }

        if (A_4.HasValue)
        {
            return new NavigatorState(mathNode, A_2, A_4);
        }

        return new NavigatorState(mathNode, A_2, A_3);
    }

    private static NavigatorState a(MathNode A_0)
    {
        for (MathNode parent = A_0.Parent; parent != null; parent = parent.Parent)
        {
            if (!parent.RestrictFromNavigation)
            {
                return new NavigatorState(parent);
            }
        }

        return NavigatorState.Empty;
    }

    private NavigatorState a(IVisualTextNode A_0, MathNode A_1, double A_2)
    {
        int num = a(A_0.CharacterWidths, A_2);
        if (A_1.DistinctNodeEdges)
        {
            if (num == 0)
            {
                return new NavigatorState(A_1, Direction.Left);
            }

            if (num > A_1.NodeInfoValueLength + 1)
            {
                return new NavigatorState(A_1, Direction.Right);
            }

            return new NavigatorState(A_1, num - 1);
        }

        return new NavigatorState(A_1, num);
    }

    private NavigatorState a(IVisualIdNode A_0, MathNode A_1, double A_2)
    {
        int caretPosition = a(A_0.CharacterWidths, A_2);
        return new NavigatorState(A_1, caretPosition, NavigatorState.DefaultCaretInsertionType(A_1, caretPosition));
    }

    private static int a(IList<double> A_0, double A_1)
    {
        double num = 0.0;
        int characterIndex;
        for (characterIndex = 0; characterIndex < A_0.Count; characterIndex++)
        {
            double characterWidth = A_0[characterIndex];
            if (A_1 < num + characterWidth / 2.0)
            {
                break;
            }

            num += characterWidth;
        }

        return characterIndex;
    }

    public d()
    {
    }
}
