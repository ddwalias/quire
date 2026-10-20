#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial struct TextRange
{
    [MirrorVerified("PtcTextRangeMechanicalDifferentialTests")]
    public TextRange(int caretPosition) : this(caretPosition, caretPosition) { }

    [MirrorVerified("Release IL")]
    public static TextRange CreateRange(int caretPosition, int offsetFromCaret) => new TextRange(caretPosition + offsetFromCaret, caretPosition);

    [MirrorVerified("Release IL")]
    public readonly int CaretPosition;

    [MirrorVerified("Release IL")]
    public readonly int SelectionStart;

    [MirrorVerified("Release IL")]
    public readonly int SelectionEnd;

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("Caret Position: {0}; Selection: {1},{2}", CaretPosition, SelectionStart, SelectionEnd);

    [MirrorVerified("Release IL")]
    public bool HasSelection => SelectionStart != SelectionEnd;

    [MirrorVerified("Release IL")]
    public int SelectionLength => SelectionEnd - SelectionStart;
}
