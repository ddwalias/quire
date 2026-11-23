#nullable disable
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class j
{
    partial struct a_
    {
        [MirrorVerified("Release IL")]
        public readonly char a;

        [MirrorVerified("Release IL")]
        public readonly SymbolMargin b;

        [MirrorVerified("Release IL")]
        public a_(char symbol, SymbolMargin symbolMargin)
        {
            a = symbol;
            b = symbolMargin;
        }
    }
}
