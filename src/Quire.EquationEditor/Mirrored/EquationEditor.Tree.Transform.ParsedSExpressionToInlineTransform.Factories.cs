namespace Quire.EquationEditor.Tree.Transform;

public partial class ParsedSExpressionToInlineTransform
{
    private sealed partial class a_
    {
        internal global::System.Windows.Documents.Inline e() => new global::System.Windows.Documents.Bold();

        internal global::System.Windows.Documents.Inline d() => new global::System.Windows.Documents.Italic();

        internal global::System.Windows.Documents.Inline c() => new global::System.Windows.Documents.Underline();

        internal global::System.Windows.Documents.Inline b() => new global::Quire.Wpf.Subscript();

        internal global::System.Windows.Documents.Inline a() => new global::System.Windows.Documents.Run();
    }
}
