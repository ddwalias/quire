#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct TextRange
{
    [MirrorVerified("PtcTextRangeMechanicalDifferentialTests")]
    public TextRange(int caretPosition) : this(caretPosition, caretPosition) { }
}
