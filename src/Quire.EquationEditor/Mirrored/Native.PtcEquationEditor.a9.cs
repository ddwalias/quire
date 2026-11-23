using System;

using System.Runtime.CompilerServices;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class a9
{
    private readonly MathNode a;
    private readonly Direction b;
    private readonly int? c;

    [SkipLocalsInit]
    public a9(a5 host, MathNode currentNode, Direction direction, int? offset)
        : base(host)
    {
        if (currentNode == null)
        {
            throw new ArgumentNullException("currentNode");
        }

        a = currentNode;
        b = direction;
        c = offset;
    }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }
}
