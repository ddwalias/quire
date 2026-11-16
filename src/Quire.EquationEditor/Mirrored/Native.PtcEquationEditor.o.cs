using System;
using System.Linq;
using Quire.EquationEditor.Tree;

namespace Quire.Native.PtcEquationEditor;

internal static partial class o
{
    public static void a(this AlgorithmOptions A_0, params string[] A_1)
    {
        if (A_1 == null || A_1.Length == 0)
        {
            return;
        }

        foreach (string text in A_1)
        {
            switch (text)
            {
                case "multiStart":
                    A_0.IsMultiStart = true;
                    break;
                case "optimize":
                    A_0.IsOptimize = true;
                    break;
            }
        }
    }

    public static void a(this MathNode A_0, string A_1)
    {
        if (string.IsNullOrEmpty(A_1) || A_0 == null)
        {
            return;
        }

        if (!A_1.Contains('-'))
        {
            A_0.Algorithm = A_1;
            return;
        }

        string[] array = A_1.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length == 0)
        {
            return;
        }

        A_0.Algorithm = array[0];
        if (array.Length > 1)
        {
            A_0.AlgorithmOptions.a(array.Skip(1).ToArray());
        }
    }
}
