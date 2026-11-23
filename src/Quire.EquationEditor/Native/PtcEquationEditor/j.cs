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
        public a_(char A_0, SymbolMargin A_1)
        {
            a = A_0;
            b = A_1;
        }
    }
}
