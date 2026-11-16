using System;
using System.Collections.Generic;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;

namespace Quire.Native.PtcEquationEditor;

internal static partial class @as
{
    public static bool a(Direction A_0, Fixity A_1)
    {
        if (A_0 == Direction.Left)
        {
            return a(A_1);
        }
        if (A_0 == Direction.Right)
        {
            return b(A_1);
        }
        throw new ArgumentException("Pick a direction, left or right", "direction");
    }

    public static bool a(Direction A_0, LinkedListNode<MathNodeAnalysisHandle> A_1)
    {
        return a(A_0, A_1.Value.EffectiveFixity);
    }

    public static bool b(LinkedListNode<MathNodeAnalysisHandle> A_0)
    {
        if (A_0 == null)
        {
            return true;
        }
        return b(A_0.Value.EffectiveFixity);
    }

    public static bool b(Fixity A_0)
    {
        if (A_0 == Fixity.Prefix || A_0 == Fixity.Infix)
        {
            return true;
        }
        return false;
    }

    public static bool b(IMathNodeDescriptor A_0)
    {
        return !a(A_0);
    }

    public static bool a(IMathNodeDescriptor A_0)
    {
        return a(A_0.Fixity);
    }

    public static bool a(LinkedListNode<MathNodeAnalysisHandle> A_0)
    {
        if (A_0 == null)
        {
            return true;
        }
        return a(A_0.Value.EffectiveFixity);
    }

    public static bool a(Fixity A_0)
    {
        if ((uint)(A_0 - 2) <= 1u)
        {
            return true;
        }
        return false;
    }
}
