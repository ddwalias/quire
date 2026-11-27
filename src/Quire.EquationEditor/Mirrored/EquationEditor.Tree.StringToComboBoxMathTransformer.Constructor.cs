using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Tree;

public partial class StringToComboBoxMathTransformer
{
    public StringToComboBoxMathTransformer()
    {
    }

    [MirrorVerified("Release IL")]
    public virtual MathNode TransformStringMathNode(string value) => r.a_.a(value);
}
