using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public static partial class ParsedSExpressionToComboBoxInputControlData
{
    private static bool a(ParsedSExpression parsed, int index, out string value)
    {
        ParsedSExpression item = parsed.SExpList[index];
        value = item.Value;
        return item.HasValue;
    }

    private static bool a(ParsedSExpression parsed, int index, out int value)
    {
        value = 0;
        ParsedSExpression item = parsed.SExpList[index];
        if (item.HasValue)
        {
            return int.TryParse(item.Value, out value);
        }

        return false;
    }

    partial class b
    {
        [MirrorVerified("Release IL")]
        public b() { }

        [MirrorVerified("Release IL")]
        internal bool d(ParsedSExpression A_0) => A_0.IsValid;

        [MirrorVerified("Release IL")]
        internal MathNode c(ParsedSExpression A_0) => SExpressionToMathNodeTransform.Default.f(A_0);

        [MirrorVerified("Release IL")]
        internal string a(ParsedSExpression A_0) => A_0.Value;
    }
}
