using System.Runtime.CompilerServices;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Parsing;

namespace Quire.Native.PtcEquationEditor;

internal partial class bk
{
    private readonly MathNode a;
    private readonly int b;
    private readonly int c;
    private readonly MathNode d;
    private readonly Fixity? e;

    [SkipLocalsInit]
    public bk(a5 host, MathNode node, int start, int end, MathNode target, Fixity? fixity)
        : base(host)
    {
        a = node;
        b = start;
        c = end;
        d = target;
        e = fixity;
    }

    private sealed partial class a_
    {
        internal bool a(MathNode node)
        {
            return node.IsPlaceholder;
        }
    }
}
