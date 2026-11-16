using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;

namespace Quire.Native.PtcEquationEditor;

internal partial struct au
{
    public readonly MathNode a;
    public readonly Panacea b;

    public au(MathNode A_0, Panacea A_1)
    {
        a = A_0;
        b = A_1;
    }
}
