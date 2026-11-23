#nullable disable
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public static partial class MathNodeToStringTransformer
{
    [MirrorVerified("Release IL")]
    public static string TransformToViewStringByType(this MathNode mathNode, Nullable<ComboBoxCellDataType> dataType) => a(mathNode, dataType, false, false);
}
